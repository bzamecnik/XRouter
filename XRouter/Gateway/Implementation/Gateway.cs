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

namespace XRouter.Gateway.Implementation
{
    class Gateway : IGatewayService, IHostableComponent
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string Name { get; private set; }

        public XmlReduction ConfigurationReduction { get; private set; }

        private IBrokerServiceForGateway broker { get; set; }

        private Dictionary<string, AdapterService> AdapterServices { get; set; }

        private Dictionary<ManualResetEvent, ResultMessage> dictSyncToken = new Dictionary<ManualResetEvent, ResultMessage>();

        private List<Task> adapterTasks = new List<Task>();

        public Gateway(IBrokerServiceForGateway broker, string name)
        {
            this.broker = broker;
            Name = name;
            ConfigurationReduction = new XmlReduction();
        }

        public void UpdateConfig(ApplicationConfiguration config)
        {
        }

        public void Start()
        {
            var configuration = broker.GetConfiguration(ConfigurationReduction);

            AdapterServices = new Dictionary<string, AdapterService>();
            var adaptersConfig = configuration.GetComponentConfiguration(Name).Element("adapters").Elements("adapter");
            foreach (var adapterConfig in adaptersConfig)
            {
                string adapterName = adapterConfig.Attribute(XName.Get("name")).Value;
                string typeAddress = adapterConfig.Attribute(XName.Get("type")).Value;
                AdapterService pluginService = GetEndpointsPluginInstance(typeAddress, adapterConfig, adapterName);
                AdapterServices.Add(adapterName, pluginService);
            }

            #region Start endpoints
            foreach (var AdapterService in AdapterServices.Values) {
                Task task = Task.Factory.StartNew(() => AdapterService.Client.Start());
                adapterTasks.Add(task);
            }
            #endregion
        }

        private AdapterService GetEndpointsPluginInstance(string typeAddress, XElement pluginConfig, string adapterName)
        {
            string[] addressParts = typeAddress.Split(';');
            string assemblyFile = addressParts[0].Trim();
            string typeFullName = addressParts[1].Trim();

            string assemblyFullPath = Path.Combine(BinPath, assemblyFile);
            Assembly assembly = Assembly.LoadFile(assemblyFullPath);
            Type type = assembly.GetType(typeFullName, true);

            var adapterService = new AdapterService(this, adapterName);
            var constructor = type.GetConstructor(new Type[] { typeof(XElement), typeof(IAdapterService) });
            var adapterObject = constructor.Invoke(new object[] { pluginConfig, adapterService });
            var plugin = (IAdapter)adapterObject;
            adapterService.Client = plugin;
            return adapterService;
        }

        public void Stop()
        {
            #region Stop endpoints
            foreach (var AdapterService in AdapterServices.Values)
            {
                AdapterService.Client.Stop();
            }
            #endregion

            adapterTasks.Clear();
        }

        public XDocument ReceiveMessage(Token token)
        {            
            ManualResetEvent e = new ManualResetEvent(false);

            dictSyncToken.Add(e, new ResultMessage(token.Guid));

            broker.ReceiveToken(token);
            e.WaitOne(); // Nastavit cas timeoute tokenu, pokud token vyprsi vraci null

            var content = dictSyncToken[e].Content;
            dictSyncToken.Remove(e);
            return content;

        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage)
        {
            var syncPair = dictSyncToken.SingleOrDefault(pair => pair.Value.TokenGuid == tokenGuid);
            if (syncPair.Key == null) {
                throw new Exception("Token guid does not exist.");
            }
            syncPair.Value.Content = resultMessage;
            syncPair.Key.Set();
        }

        public void ReceiveMessageAsync(Token token)
        {
            //! doplnit info do tokeku
            broker.ReceiveToken(token);
        }

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
        {
            if (address.GatewayName != Name) {
                throw new ArgumentException("Incorrect gateway name.", "address");
            }

            AdapterService adapterService = AdapterServices.Values.SingleOrDefault(a => a.AdapterName == address.AdapterName);
            if (adapterService == null) {
                throw new ArgumentException("Incorrect adapter name.", "address");
            }

            return adapterService.SendMessageToOutputEndPoint(address, message);
        }

        private class ResultMessage
        {
            public Guid TokenGuid { get; private set; }

            public XDocument Content { get; set; }

            public ResultMessage(Guid tokenGuid)
            {
                TokenGuid = tokenGuid;
            }
        }
    }
}

