using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Collections.Concurrent;
using System.Xml.Linq;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using System.Threading.Tasks;

namespace XRouter.Broker
{
    public class BrokerService : IBrokerService, IBrokerServiceForDispatcher
    {
        private PersistentStorage storage;
        private ConcurrentDictionary<string, ComponentAccessor> componentsAccessors = new ConcurrentDictionary<string, ComponentAccessor>();

        private Dispatching.Dispatcher dispatcher;
        private XmlResourceManager xmlResourceManager;

        public BrokerService()
        {
            storage = new PersistentStorage();
            UpdateComponentsAccessorsAccordingToConfig();

            xmlResourceManager = new XmlResourceManager(storage, GetConfiguration);
            dispatcher = new Dispatching.Dispatcher(this);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void StartComponents()
        {
            ComponentAccessor[] components = componentsAccessors.Values.ToArray();
            foreach (ComponentAccessor component in components) {
                component.Start();
            }
        }

        public void StopComponents()
        {
            ComponentAccessor[] components = componentsAccessors.Values.ToArray();
            foreach (ComponentAccessor component in components) {
                component.Stop();
            }
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

            UpdateComponentsAccessorsAccordingToConfig();
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
            token.State = TokenState.Received;
            storage.SaveToken(token);
            dispatcher.NotifyAboutNewToken(token);
        }

        public void UpdateTokenMessageFlowState(string updatingProcessorName, Guid tokenGuid, MessageFlowState messageFlowState)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                if (token.MessageFlowState.AssignedProcessor == updatingProcessorName) {
                    token.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                    token.SaveMessageFlowState(messageFlowState);
                }
            });
        }

        public void AddMessageToToken(string updatingProcessorName, Guid targetTokenGuid, string messageName, SerializableXDocument message)
        {
            storage.UpdateToken(targetTokenGuid, delegate(Token token) {
                if (token.MessageFlowState.AssignedProcessor == updatingProcessorName) {
                    token.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                    token.AddMessage(messageName, message);
                }
            });
        }

        public void FinishToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument resultMessage)
        {
            Token token = storage.GetToken(tokenGuid);
            if (token.MessageFlowState.AssignedProcessor == updatingProcessorName) {
                storage.UpdateToken(tokenGuid, delegate(Token t) {
                    t.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                    t.SaveMessageFlowState();
                });

                XDocument sourceMetadata = token.GetSourceMetadata();
                GatewayAccessor sourceGateway = componentsAccessors.OfType<GatewayAccessor>().SingleOrDefault(gwa => gwa.ComponentName == token.SourceAddress.GatewayName);
                sourceGateway.ReceiveReturn(tokenGuid, resultMessage, new SerializableXDocument(sourceMetadata));

                storage.UpdateToken(tokenGuid, delegate(Token t) {
                    t.State = TokenState.Finished;
                });
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

        public SerializableXDocument GetXmlResource(XrmTarget target)
        {
            XDocument xmlResource = xmlResourceManager.GetXmlResource(target);
            return new SerializableXDocument(xmlResource);
        }

        public void UpdateComponentControllerAddress(string componentName, Uri controllerAddress)
        {
            ComponentAccessor component;
            if (componentsAccessors.TryGetValue(componentName, out component)) {
                component.ControllerAddress = controllerAddress;
            } else {
                throw new ArgumentException("Cannot find component with given name.");
            }
        }

        public void UpdateComponentInfo(string componentName, Uri componentAddress, XmlReduction configReduction)
        {
            ComponentAccessor component;
            if (componentsAccessors.TryGetValue(componentName, out component)) {
                component.ComponentAddress = componentAddress;
                component.ConfigurationReduction = configReduction;
            } else {
                throw new ArgumentException("Cannot find component with given name.");
            }
        }

        private void UpdateComponentsAccessorsAccordingToConfig()
        {
            ApplicationConfiguration config = GetConfiguration();

            ComponentAccessor[] oldComponents = componentsAccessors.Values.ToArray();
            List<ComponentAccessor> newComponents = new List<ComponentAccessor>();
            var componentsNames = config.GetComponentNames();
            foreach (string componentName in componentsNames) {
                ComponentAccessor component = oldComponents.FirstOrDefault(c => c.ComponentName == componentName);
                if (component == null) {
                    component = ComponentAccessor.Create(componentName, config);
                }
                newComponents.Add(component);
            }

            componentsAccessors.Clear();
            foreach (var componentInfo in newComponents) {
                componentsAccessors.AddOrUpdate(componentInfo.ComponentName, componentInfo, (key, oldValue) => componentInfo);
            }
        }
    }
}
