using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using XRouter.Common;
using XRouter.Broker;
using XRouter.Common.Utils;
using System.Collections.Concurrent;

namespace XRouter.Gateway.Implementation
{
    public class Gateway : IGatewayService
    {
        public string Name { get; private set; }

        public XmlReduction ConfigurationReduction { get; private set; }

        internal ApplicationConfiguration Configuration { get; private set; }

        private IBrokerServiceForGateway Broker { get; set; }

        private Dictionary<string, AdapterService> AdapterServicesByName { get; set; }

        private ConcurrentDictionary<Guid, MessageResultHandler> waintingResultMessageHandlers;

        public Gateway()
        {
            ConfigurationReduction = new XmlReduction();
            waintingResultMessageHandlers = new ConcurrentDictionary<Guid, MessageResultHandler>();
        }

        public void UpdateConfig(ApplicationConfiguration config)
        {
            Configuration = config;
        }

        public void Start(string name, IBrokerServiceForGateway broker)
        {
            Broker = broker;
            Name = name;

            Configuration = Broker.GetConfiguration(ConfigurationReduction);

            #region Create adapters and AdapterServices
            AdapterServicesByName = new Dictionary<string, AdapterService>();
            var adaptersConfig = Configuration.GetComponentConfiguration(Name).Element("adapters").Elements("adapter");
            foreach (var adapterConfig in adaptersConfig) {
                string adapterName = adapterConfig.Attribute(XName.Get("name")).Value;
                string typeFullNameAndAssembly = adapterConfig.Attribute(XName.Get("type")).Value;
                AdapterService adapterService = CreateServiceForAdapter(typeFullNameAndAssembly, adapterConfig, adapterName);
                AdapterServicesByName.Add(adapterName, adapterService);
            }
            #endregion

            #region Start adapters
            foreach (var adapterService in AdapterServicesByName.Values) {
                Task.Factory.StartNew(delegate {
                    adapterService.Adapter.Start(adapterService);
                }, TaskCreationOptions.LongRunning);
            }
            #endregion
        }

        public void Stop()
        {
            foreach (var adapterService in AdapterServicesByName.Values) {
                adapterService.Adapter.Stop();
            }
        }

        private AdapterService CreateServiceForAdapter(string typeFullNameAndAssembly, XElement adapterConfig, string adapterName)
        {
            var plugin = TypeUtils.CreateTypeInstance<IAdapter>(typeFullNameAndAssembly);
            var adapterService = new AdapterService(this, adapterName);
            adapterService.Adapter = plugin;
            return adapterService;
        }

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata)
        {
            if (address.GatewayName != Name) {
                throw new ArgumentException("Incorrect gateway name.", "address");
            }
            if (!AdapterServicesByName.ContainsKey(address.AdapterName)) {
                throw new ArgumentException("Incorrect adapter name.", "address");
            }

            AdapterService adapterService = AdapterServicesByName[address.AdapterName];
            XDocument result = adapterService.SendMessage(address, message, metadata);
            return new SerializableXDocument(result);
        }

        internal void ReceiveToken(Token token, MessageResultHandler resultHandler = null)
        {
            if (resultHandler != null) {
                waintingResultMessageHandlers.AddOrUpdate(token.Guid, resultHandler, (key, oldValue) => resultHandler);
            }
            Task.Factory.StartNew(delegate {
                Broker.ReceiveToken(token);
            });
        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata)
        {
            MessageResultHandler resultHandler;
            if (waintingResultMessageHandlers.TryRemove(tokenGuid, out resultHandler)) {
                resultHandler(tokenGuid, resultMessage.XDocument, sourceMetadata.XDocument);
            }
        }
    }
}

