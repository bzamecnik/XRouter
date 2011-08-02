﻿using System;
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
using XRouter.Common.ComponentInterfaces;
using System.Xml.XPath;

namespace XRouter.Gateway
{
    public class Gateway : IGatewayService
    {
        public string Name { get; private set; }

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

        private Adapter CreateAdapter(string typeFullNameAndAssembly, XElement adapterConfig, string adapterName)
        {
            var adapter = TypeUtils.CreateTypeInstance<Adapter>(typeFullNameAndAssembly);
            adapter.Gateway = this;
            adapter.AdapterName = adapterName;
            adapter.Config = new XDocument(adapterConfig.XPathSelectElement("objectConfig"));
            return adapter;
        }

        public SerializableXDocument SendMessage(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata)
        {
            if (!AdaptersByName.ContainsKey(address.AdapterName)) {
                throw new ArgumentException("Incorrect adapter name.", "address");
            }

            Adapter adapter = AdaptersByName[address.AdapterName];
            XDocument result = adapter.SendMessage(address.EndPointName, message.XDocument, metadata.XDocument);
            return new SerializableXDocument(result);
        }

        internal void ReceiveToken(Token token, object context, MessageResultHandler resultHandler = null)
        {
            if (resultHandler != null) {
                ResultHandlingInfo resultHandlingInfo = new ResultHandlingInfo(token.Guid, resultHandler, context);
                waitingResultMessageHandlers.AddOrUpdate(token.Guid, resultHandlingInfo, (key, oldValue) => resultHandlingInfo);
            }
            Task.Factory.StartNew(delegate {
                Broker.ReceiveToken(token);
            });
        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata)
        {
            ResultHandlingInfo resultHandlingInfo;
            if (waitingResultMessageHandlers.TryRemove(tokenGuid, out resultHandlingInfo))
            {
                resultHandlingInfo.ResultHandler(tokenGuid, resultMessage.XDocument, sourceMetadata.XDocument, resultHandlingInfo.Context);
            }
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

