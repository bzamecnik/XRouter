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

namespace XRouter.Gateway
{
    public class Gateway : IGatewayService
    {
        public string Name { get; private set; }

        public XmlReduction ConfigurationReduction { get; private set; }

        internal ApplicationConfiguration Configuration { get; private set; }

        private IBrokerServiceForGateway Broker { get; set; }

        private Dictionary<string, Adapter> AdaptersByName { get; set; }

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
            AdaptersByName = new Dictionary<string, Adapter>();
            var adaptersConfig = Configuration.GetComponentConfiguration(Name).Element("adapters").Elements("adapter");
            foreach (var adapterConfig in adaptersConfig) {
                string adapterName = adapterConfig.Attribute(XName.Get("name")).Value;
                string typeFullNameAndAssembly = adapterConfig.Attribute(XName.Get("type")).Value;
                Adapter adapter = CreateAdapter(typeFullNameAndAssembly, adapterConfig, adapterName);
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

