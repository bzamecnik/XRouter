﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Collections.Concurrent;
using System.Xml.Linq;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using System.Threading.Tasks;
using XRouter.Common.ComponentInterfaces;
using System.Threading;

namespace XRouter.Broker
{
    public class BrokerService : IBrokerService, IBrokerServiceForDispatcher
    {
        private PersistentStorage storage;
        private ConcurrentDictionary<string, ComponentAccessor> componentsAccessors = new ConcurrentDictionary<string, ComponentAccessor>();

        private Dispatching.Dispatcher dispatcher;
        private XmlResourceManager xmlResourceManager;

        private object syncLock = new object();

        public BrokerService()
        {
        }

        public void Start(IEnumerable<GatewayProvider> gatewayProviders, IEnumerable<ProcessorProvider> processorProviders)
        {
            storage = new PersistentStorage();
            ApplicationConfiguration config = GetConfiguration();

            #region Prepare componentsAccessors
            foreach (GatewayProvider gatewayProvider in gatewayProviders) {
                GatewayAccessor accessor = new GatewayAccessor(gatewayProvider.Name, gatewayProvider.Gateway, config);
                componentsAccessors.AddOrUpdate(accessor.ComponentName, accessor, (key, oldValue) => accessor);
            }
            foreach (ProcessorProvider processorProvider in processorProviders) {
                ProcessorAccessor accessor = new ProcessorAccessor(processorProvider.Name, processorProvider.Processor, config);
                componentsAccessors.AddOrUpdate(accessor.ComponentName, accessor, (key, oldValue) => accessor);
            }
            #endregion

            xmlResourceManager = new XmlResourceManager(storage);
            dispatcher = new Dispatching.Dispatcher(this);
        }

        public void Stop()
        {
            // Make sure that all operations are completed and no one will be running after this call
            Monitor.Enter(syncLock);
            dispatcher.Stop();
        }

        public ApplicationConfiguration GetConfiguration(XmlReduction reduction)
        {
            ApplicationConfiguration config = GetConfiguration();
            ApplicationConfiguration result = config.GetReducedConfiguration(reduction);
            return result;
        }

        public ApplicationConfiguration GetConfiguration()
        {
            XDocument configXml = storage.GetConfigXml();
            var result = new ApplicationConfiguration(configXml);
            return result;
        }

        public void ChangeConfiguration(ApplicationConfiguration config)
        {
            storage.SaveConfigXml(config.Content);

            ComponentAccessor[] components = componentsAccessors.Values.ToArray();
            foreach (var component in components) {
                var reducedConfig = config.GetReducedConfiguration(component.ConfigurationReduction);
                component.UpdateConfig(reducedConfig);
            }
        }

        IEnumerable<ProcessorAccessor> IBrokerServiceForDispatcher.GetProcessors()
        {
            ComponentAccessor[] components = componentsAccessors.Values.ToArray();
            var result = components.OfType<ProcessorAccessor>();
            return result;
        }

        IEnumerable<Token> IBrokerServiceForDispatcher.GetActiveTokensAssignedToProcessor(string processorName)
        {
            var result = storage.GetActiveTokensAssignedToProcessor(processorName);
            return result;
        }

        IEnumerable<Token> IBrokerServiceForDispatcher.GetUndispatchedTokens()
        {
            var result = storage.GetUndispatchedTokens();
            return result;
        }

        void IBrokerServiceForDispatcher.UpdateTokenAssignedProcessor(Guid tokenGuid, string assignedProcessor)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.MessageFlowState.AssignedProcessor = assignedProcessor;
            });
        }

        void IBrokerServiceForDispatcher.UpdateTokenMessageFlow(Guid tokenGuid, Guid messageFlowGuid)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.MessageFlowState.MessageFlowGuid = messageFlowGuid;
                token.SaveMessageFlowState();
            });
        }

        void IBrokerServiceForDispatcher.UpdateTokenLastResponseFromProcessor(Guid tokenGuid, DateTime lastResponse)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.MessageFlowState.LastResponseFromProcessor = lastResponse;
            });
        }

        Token IBrokerServiceForDispatcher.GetToken(Guid tokenGuid)
        {
            Token token = storage.GetToken(tokenGuid);
            return token;
        }

        public void ReceiveToken(Token token)
        {
            lock (syncLock)
            {
                token.State = TokenState.Received;
                storage.SaveToken(token);
                dispatcher.NotifyAboutNewToken(token);
            }
        }

        public void UpdateTokenMessageFlowState(string updatingProcessorName, Guid tokenGuid, MessageFlowState messageFlowState)
        {
            lock (syncLock)
            {
                storage.UpdateToken(tokenGuid, delegate(Token token)
                {
                    if (token.MessageFlowState.AssignedProcessor == updatingProcessorName)
                    {
                        token.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                        token.SaveMessageFlowState(messageFlowState);
                    }
                });
            }
        }

        public void AddMessageToToken(string updatingProcessorName, Guid targetTokenGuid, string messageName, SerializableXDocument message)
        {
            lock (syncLock)
            {
                storage.UpdateToken(targetTokenGuid, delegate(Token token)
                {
                    if (token.MessageFlowState.AssignedProcessor == updatingProcessorName)
                    {
                        token.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                        token.AddMessage(messageName, message);
                    }
                });
            }
        }

        public void FinishToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument resultMessage)
        {
            lock (syncLock)
            {
                Token token = storage.GetToken(tokenGuid);
                if (token.MessageFlowState.AssignedProcessor == updatingProcessorName)
                {
                    storage.UpdateToken(tokenGuid, delegate(Token t)
                    {
                        t.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                        t.SaveMessageFlowState();
                    });

                    XDocument sourceMetadata = token.GetSourceMetadata();
                    GatewayAccessor sourceGateway = componentsAccessors.Values.OfType<GatewayAccessor>().SingleOrDefault(gwa => gwa.ComponentName == token.SourceAddress.GatewayName);
                    sourceGateway.ReceiveReturn(tokenGuid, resultMessage, new SerializableXDocument(sourceMetadata));

                    storage.UpdateToken(tokenGuid, delegate(Token t)
                    {
                        t.State = TokenState.Finished;
                    });
                }
            }
        }

        public SerializableXDocument SendMessage(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata)
        {
            ComponentAccessor component;
            if (componentsAccessors.TryGetValue(address.GatewayName, out component)) {
                if (component is GatewayAccessor) {
                    GatewayAccessor gateway = (GatewayAccessor)component;
                    SerializableXDocument result = gateway.SendMessageToOutputEndPoint(address, message, metadata);
                    return result;
                }
            }
            throw new ArgumentException("Cannot find target gateway.");
        }

        public MessageFlowConfiguration[] GetActiveMessageFlows()
        {
            var config = GetConfiguration();
            var activeMessageFlowGuids = storage.GetActiveMessageFlowsGuids();

            List<MessageFlowConfiguration> result = new List<MessageFlowConfiguration>();
            foreach (var guid in activeMessageFlowGuids) {
                MessageFlowConfiguration messageFlow = config.GetMessageFlow(guid);
                result.Add(messageFlow);
            }
            return result.ToArray();
        }

        public SerializableXDocument GetXmlResource(XrmUri target)
        {
            XDocument xmlResource = xmlResourceManager.GetXmlResource(target);
            return new SerializableXDocument(xmlResource);
        }
    }
}


