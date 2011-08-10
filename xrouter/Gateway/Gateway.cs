using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.Utils;

namespace XRouter.Gateway
{
    /// <summary>
    /// Gateway is a component which manages communication of XRouter with
    /// external systems via various communication means.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A gateway can manage several adapters, each specialized in one
    /// communication protocol (such as file exchange, web services, email,
    /// etc.). It works as a mediator between adapters and the broker.
    /// Incoming messages are passed from adapter to the broker for being
    /// processed and outgoing processed messages go the other way.
    /// </para>
    /// <para>
    /// The code in a gateway run from broker or component hosting thread.
    /// Each adapter runs in its own thread.
    /// </para>
    /// </remarks>
    public class Gateway : IGatewayService
    {
        /// <summary>
        /// Identifier of the gateway component.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Configuration reduction to get parts of configuration specific
        /// to this gateway.
        /// </summary>
        public XmlReduction ConfigurationReduction { get; private set; }

        internal ApplicationConfiguration Configuration { get; private set; }

        private IBrokerServiceForGateway Broker { get; set; }

        private Dictionary<string, Adapter> AdaptersByName { get; set; }

        private ConcurrentDictionary<Guid, ResultHandlingInfo> waitingResultMessageHandlers;

        public Gateway()
        {
            ConfigurationReduction = new XmlReduction();
            waitingResultMessageHandlers = new ConcurrentDictionary<Guid, ResultHandlingInfo>();
        }

        #region IComponentService Members

        public void UpdateConfig(ApplicationConfiguration config)
        {
            Configuration = config;
        }

        #endregion

        #region IGatewayService Members

        public void Start(string name, IBrokerServiceForGateway broker)
        {
            Broker = broker;
            Name = name;

            Configuration = Broker.GetConfiguration(ConfigurationReduction);

            #region Create adapters and AdapterServices
            AdaptersByName = new Dictionary<string, Adapter>();
            var xAdaptersConfig = Configuration.GetAdapterConfigurations(Name);
            foreach (var xAdapterConfig in xAdaptersConfig) {
                string adapterName = xAdapterConfig.Attribute(XName.Get("name")).Value;
                string adapterTypeName = xAdapterConfig.Attribute(XName.Get("type")).Value;
                AdapterType adapterType = Configuration.GetAdapterType(adapterTypeName);
                Adapter adapter = CreateAdapter(adapterType.AssemblyAndClrType, xAdapterConfig, adapterName);
                AdaptersByName.Add(adapterName, adapter);
            }
            #endregion

            #region Start adapters
            foreach (var adapter in AdaptersByName.Values) {
                adapter.Start();
            }
            #endregion
        }

        public void Stop()
        {
            Parallel.ForEach(AdaptersByName.Values, delegate(Adapter adapter)
            {
                adapter.Stop();
            });
        }

        public SerializableXDocument SendMessage(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata)
        {
            if (!AdaptersByName.ContainsKey(address.AdapterName)) {
                throw new ArgumentException(string.Format(
                    "There is no adapter named '{0}' in gateway '{1}'.",
                    address.AdapterName, Name), "address");
            }

            Adapter adapter = AdaptersByName[address.AdapterName];
            XDocument result = adapter.SendMessage(address.EndpointName, message.XDocument, metadata.XDocument);
            return new SerializableXDocument(result);
        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata)
        {
            ResultHandlingInfo resultHandlingInfo;
            if (waitingResultMessageHandlers.TryRemove(tokenGuid, out resultHandlingInfo))
            {
                resultHandlingInfo.ResultHandler(tokenGuid, resultMessage.XDocument, sourceMetadata.XDocument, resultHandlingInfo.Context);
            }
        }

        #endregion

        internal void ReceiveToken(Token token, object context, MessageResultHandler resultHandler = null)
        {
            if (resultHandler != null)
            {
                ResultHandlingInfo resultHandlingInfo = new ResultHandlingInfo(token.Guid, resultHandler, context);
                waitingResultMessageHandlers.AddOrUpdate(token.Guid, resultHandlingInfo, (key, oldValue) => resultHandlingInfo);
            }
            Broker.ReceiveToken(token);
        }

        private Adapter CreateAdapter(string typeFullNameAndAssembly, XElement adapterConfig, string adapterName)
        {
            var adapter = TypeUtils.CreateTypeInstance<Adapter>(typeFullNameAndAssembly);
            adapter.Gateway = this;
            adapter.AdapterName = adapterName;
            adapter.Config = new XDocument(adapterConfig.XPathSelectElement("objectConfig"));
            return adapter;
        }

        private class ResultHandlingInfo
        {
            public Guid TokenGuid { get; private set; }
            public MessageResultHandler ResultHandler { get; private set; }
            public object Context { get; private set; }

            public ResultHandlingInfo(Guid tokenGuid, MessageResultHandler resultHandler, object context)
            {
                TokenGuid = tokenGuid;
                ResultHandler = resultHandler;
                Context = context;
            }
        }
    }
}

