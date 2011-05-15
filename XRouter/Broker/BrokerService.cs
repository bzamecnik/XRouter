using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace XRouter.Broker
{
    class BrokerService : IBrokerService, IBrokerServiceForDispatcher
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

        public IEnumerable<ProcessorAccessor> GetProcessors()
        {
            ComponentAccessor[] components = componentsAccessors.Values.ToArray();
            var result = components.OfType<ProcessorAccessor>();
            return result;
        }

        public IEnumerable<Token> GetActiveTokensAssignedToProcessor(string processorName)
        {
            var result = storage.GetActiveTokensAssignedToProcessor(processorName);
            return result;
        }

        public IEnumerable<Token> GetUndispatchedTokens()
        {
            var result = storage.GetUndispatchedTokens();
            return result;
        }

        public void UpdateTokenAssignedProcessor(Guid tokenGuid, string assignedProcessor)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.MessageFlowState.AssignedProcessor = assignedProcessor;
            });
        }

        public void UpdateTokenMessageFlow(Guid tokenGuid, Guid messageFlowGuid)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.MessageFlowState.MessageFlowGuid = messageFlowGuid;
            });
        }

        public void UpdateTokenLastResponseFromProcessor(Guid tokenGuid, DateTime lastResponse)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.MessageFlowState.LastResponseFromProcessor = lastResponse;
            });
        }

        public Token GetToken(Guid tokenGuid)
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
                    token.MessageFlowState = messageFlowState;
                }
            });
        }

        public void AddMessageToToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument message)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                if (token.MessageFlowState.AssignedProcessor == updatingProcessorName) {
                    token.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                    token.AddMessage(message);
                }
            });
        }

        public void FinishToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument resultMessage)
        {
            Token token = storage.GetToken(tokenGuid);
            if (token.MessageFlowState.AssignedProcessor == updatingProcessorName) {
                storage.UpdateToken(tokenGuid, delegate(Token t) {
                    t.MessageFlowState.LastResponseFromProcessor = DateTime.Now;
                });

                GatewayAccessor sourceGateway = componentsAccessors.OfType<GatewayAccessor>().SingleOrDefault(gwa => gwa.ComponentName == token.GatewayName);
                sourceGateway.ReceiveReturn(tokenGuid, resultMessage);

                storage.UpdateToken(tokenGuid, delegate(Token t) {
                    t.State = TokenState.Finished;
                });
            }
        }

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
        {
            ComponentAccessor component;
            if (componentsAccessors.TryGetValue(address.GatewayName, out component)) {
                if (component is GatewayAccessor) {
                    GatewayAccessor gateway = (GatewayAccessor)component;
                    SerializableXDocument result = gateway.SendMessageToOutputEndPoint(address, message);
                    return result;
                }
            }
            throw new ArgumentException("Cannot find target gateway.");
        }

        public MessageFlow[] GetActiveMessageFlows()
        {
            var config = GetConfiguration();
            var activeMessageFlowGuids = storage.GetActiveMessageFlowsGuids();

            List<MessageFlow> result = new List<MessageFlow>();
            foreach (var guid in activeMessageFlowGuids) {
                MessageFlow messageFlow = config.GetMessageFlow(guid);
                result.Add(messageFlow);
            }
            return result.ToArray();
        }

        public SerializableXDocument GetXmlResource(Uri resourceUri)
        {
            XDocument xmlResource = xmlResourceManager.GetXmlResource(resourceUri);
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
