using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace XRouter.Broker
{
    class BrokerService : IBrokerService
    {
        private PersistentStorage storage;
        private ConcurrentDictionary<string, ComponentAccessor> componentsAccessors = new ConcurrentDictionary<string, ComponentAccessor>();
        private Dispatching.Dispatcher dispatcher;

        public BrokerService()
        {
            storage = new PersistentStorage();
            UpdateComponentsAccessorsAccordingToConfig();

            dispatcher = new Dispatching.Dispatcher();
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

        public void ReceiveToken(Token token)
        {
            token.State = TokenState.Received;
            storage.SaveToken(token);
            dispatcher.Dispatch(token);
        }

        public void UpdateTokenWorkflowState(Guid tokenGuid, WorkflowState workflowState)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) { 
                token.WorkflowState = workflowState; 
            });
        }

        public void AddMessageToToken(Guid tokenGuid, SerializableXDocument message)
        {
            storage.UpdateToken(tokenGuid, delegate(Token token) {
                token.AddMessage(message);
            });
        }

        public void FinishToken(Guid tokenGuid, SerializableXDocument resultMessage)
        {
            Token token = storage.GetToken(tokenGuid);
            GatewayAccessor sourceGateway = componentsAccessors.OfType<GatewayAccessor>().SingleOrDefault(gwa => gwa.ComponentName == token.GatewayName);
            sourceGateway.ReceiveReturn(tokenGuid, resultMessage);

            storage.UpdateToken(tokenGuid, delegate(Token t) {
                t.State = TokenState.Finished;
            });
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
