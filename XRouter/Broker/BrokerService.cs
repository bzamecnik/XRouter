using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.Persistence;
using XRouter.Common.Xrm;
using wcf = System.ServiceModel;

namespace XRouter.Broker
{
    /// <summary>
    /// Implements the broker component which acts as a mediator among all
    /// the other components. It also provides a web service for XRouter
    /// management.
    /// </summary>
    [wcf.ServiceBehavior(InstanceContextMode = wcf.InstanceContextMode.Single)]
    public class BrokerService : IBrokerService, IBrokerServiceForDispatcher
    {
        // TODO: make the log directory/path configurable
        private static readonly string LogDirectory = "Logs";

        private PersistentStorage storage;
        private ConcurrentDictionary<string, ComponentAccessor> componentsAccessors = new ConcurrentDictionary<string, ComponentAccessor>();

        private Dispatching.Dispatcher dispatcher;
        private XmlResourceManager xmlResourceManager;

        private object syncLock = new object();

        public BrokerService()
        {
        }

        public void Start(string dbConnectionString, IEnumerable<GatewayProvider> gatewayProviders, IEnumerable<ProcessorProvider> processorProviders)
        {
            storage = new PersistentStorage(dbConnectionString);
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
            // Make sure that all operations are completed and none of them will be running after this call
            // TODO: this can potentially wait indefinitely, try cutting it
            Monitor.Enter(syncLock);
            //Monitor.TryEnter(syncLock, new TimeSpan(0, 0, 0, 30));
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
            XDocument configXml = storage.GetApplicationConfiguration();
            var result = new ApplicationConfiguration(configXml);
            return result;
        }

        public void UpdateConfiguration(ApplicationConfiguration config)
        {
            storage.SaveApplicationConfiguration(config.Content);

            // TODO: why not just iterate over componentsAccessors.Values?
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
                token.UpdateMessageFlowState(mfs => { mfs.AssignedProcessor = assignedProcessor; });
            });
        }

        void IBrokerServiceForDispatcher.UpdateTokenMessageFlow(Guid tokenGuid, Guid messageFlowGuid)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.UpdateMessageFlowState(mfs => { mfs.MessageFlowGuid = messageFlowGuid; });
            });
        }

        void IBrokerServiceForDispatcher.UpdateTokenLastResponseFromProcessor(Guid tokenGuid, DateTime lastResponse)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.UpdateMessageFlowState(mfs => { mfs.LastResponseFromProcessor = lastResponse; });
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
                storage.UpdateToken(tokenGuid, delegate(Token token) {
                    if (messageFlowState.AssignedProcessor == updatingProcessorName) {
                        messageFlowState.LastResponseFromProcessor = DateTime.Now;
                        token.SetMessageFlowState(messageFlowState);
                    }
                });
            }
        }

        public Token AddMessageToToken(string updatingProcessorName, Guid targetTokenGuid, string messageName, SerializableXDocument message)
        {
            lock (syncLock)
            {
                Token updatedToken = storage.UpdateToken(targetTokenGuid, delegate(Token token)
                {
                    MessageFlowState messageflowState = token.GetMessageFlowState();
                    if (messageflowState.AssignedProcessor == updatingProcessorName)
                    {
                        token.UpdateMessageFlowState(mfs => { mfs.LastResponseFromProcessor = DateTime.Now; });
                        token.AddMessage(messageName, message);
                    }
                });
                return updatedToken;
            }
        }

        public Token AddExceptionToToken(string updatingProcessorName, Guid targetTokenGuid, string sourceNodeName, string message, string stackTrace)
        {
            lock (syncLock) {
                Token updatedToken = storage.UpdateToken(targetTokenGuid, delegate(Token token) {
                    MessageFlowState messageflowState = token.GetMessageFlowState();
                    if (messageflowState.AssignedProcessor == updatingProcessorName) {
                        token.UpdateMessageFlowState(mfs => { mfs.LastResponseFromProcessor = DateTime.Now; });
                        token.AddException(sourceNodeName, message, stackTrace);
                    }
                });
                return updatedToken;
            }
        }

        public void FinishToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument resultMessage)
        {
            lock (syncLock)
            {
                Token token = storage.GetToken(tokenGuid);
                MessageFlowState messageflowState = token.GetMessageFlowState();
                if (messageflowState.AssignedProcessor == updatingProcessorName)
                {
                    storage.UpdateToken(tokenGuid, delegate(Token t) {
                        t.UpdateMessageFlowState(mfs => { mfs.LastResponseFromProcessor = DateTime.Now; });
                    });

                    XDocument sourceMetadata = token.GetSourceMetadata();
                    var sourceAdress = token.GetSourceAddress();
                    if (sourceAdress != null)
                    {
                        string sourceGatewayName = sourceAdress.GatewayName;
                        GatewayAccessor sourceGateway = componentsAccessors.Values.OfType<GatewayAccessor>().SingleOrDefault(gwa => gwa.ComponentName == sourceGatewayName);
                        sourceGateway.ReceiveReturn(tokenGuid, resultMessage, new SerializableXDocument(sourceMetadata));
                    }

                    storage.UpdateToken(tokenGuid, delegate(Token t) {
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
            throw new ArgumentException(string.Format(
                "Cannot find target gateway of endpoint address '{0}'.", address));
        }

        public SerializableXDocument GetXmlResource(XrmUri target)
        {
            XDocument xmlResource = xmlResourceManager.GetXmlResource(target);
            return new SerializableXDocument(xmlResource);
        }
    }
}


