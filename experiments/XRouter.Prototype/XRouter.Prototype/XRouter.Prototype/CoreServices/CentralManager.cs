using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Prototype.CoreTypes;
using System.Threading.Tasks;
using System.Threading;

namespace XRouter.Prototype.CoreServices
{
    class CentralManager : ICentralManager, IConfigurationManagement
    {
        private PersistentStorage storage;

        private object stateLock = new object();
        private Dictionary<string, ComponentInfo> knownComponentsInfo = new Dictionary<string, ComponentInfo>();
        private Dictionary<EndPointAddress, IOutputEndpoint> registeredOutputEndpoints = new Dictionary<EndPointAddress, IOutputEndpoint>();

        public bool IsCentralManagerRunning { get; private set; }

        private Dispatcher dispatcher;

        public CentralManager()
        {
            storage = new PersistentStorage();
            UpdateKnownComponentsAccordingToConfig();

            dispatcher = new Dispatcher(GetConfiguration, storage.GetTokensAssignedToProcessor);

            // todo: checking for dead output endpoints
        }

        #region ICentralManager

        public ConnectComponentResult ConnectComponent(string name, IComponentHost componentHost, ComponentState state, XmlReduction configReduction)
        {
            var componentInfo = GetComponentInfo(name);
            componentInfo.OnComponentConnected(componentHost, configReduction, state);

            ApplicationConfiguration config = GetConfiguration();
            var reducedConfig = config.GetReducedConfiguration(configReduction);
            var result = new ConnectComponentResult(reducedConfig);
            return result;
        }

        public void ReceiveToken(Token token)
        {
            if (!IsCentralManagerRunning) {
                throw new InvalidOperationException("Central manager is not runnig.");
            }
            token.State = TokenState.Received;
            storage.SaveToken(token);
            dispatcher.Dispatch(token);
        }

        public void RegisterOutputEndPoint(EndPointAddress address, IOutputEndpoint outputEndPoint)
        {
            lock (stateLock) {
                if (registeredOutputEndpoints.ContainsKey(address)) {
                    registeredOutputEndpoints[address] = outputEndPoint;
                } else {
                    registeredOutputEndpoints.Add(address, outputEndPoint);
                }
            }
        }

        public void UnregisterOutputEndPoint(EndPointAddress address)
        {
            lock (stateLock) {
                if (registeredOutputEndpoints.ContainsKey(address)) {
                    registeredOutputEndpoints.Remove(address);
                }
            }
        }

        public void SendMessageToOutputEndPoint(EndPointAddress address, Token messageToken, Action<Token> resultCallback)
        {
            IOutputEndpoint outputEndpointProxy = null;
            lock (stateLock) {
                if (registeredOutputEndpoints.ContainsKey(address)) {
                    outputEndpointProxy = registeredOutputEndpoints[address];
                }
            }
            if (outputEndpointProxy == null) {
                throw new ArgumentException("Cannot find output endpoint with give name.");
            }

            outputEndpointProxy.SendMessage(messageToken, resultCallback);
        }

        public void UpdateTokenWorkflowState(Guid tokenGuid, Token token)
        {
            //todo
        }

        public void AddMessageToToken(Guid tokenGuid, XDocument message)
        {
            //todo
        }

        #endregion

        #region ISystemManagement

        public ApplicationConfiguration GetConfiguration()
        {
            XDocument configXml = storage.GetConfigXml();
            var result = new ApplicationConfiguration(configXml);
            return result;
        }

        public void ChangeConfiguration(ApplicationConfiguration configuration)
        {
            storage.SaveConfigXml(configuration.Content);

            UpdateKnownComponentsAccordingToConfig();
            DoForConnectedComponents(delegate(ComponentInfo componentInfo) {
                var reducedConfig = configuration.GetReducedConfiguration(componentInfo.ConfigurationReduction);
                componentInfo.ComponentHostProxy.ChangeConfig(reducedConfig);
            });
        }

        public void StartCentralManager()
        {
            lock (stateLock) {
                if (IsCentralManagerRunning) { return; }
                IsCentralManagerRunning = true;
            }

            DoForConnectedComponents(delegate(ComponentInfo componentInfo) {
                componentInfo.ComponentHostProxy.OnCentralManagerStarted();
            });

            dispatcher.Start();
            DispatchReceivedMessages();
        }

        public void StopCentralManager()
        {
            lock (stateLock) {
                if (!IsCentralManagerRunning) { return; }
                IsCentralManagerRunning = false;
            }

            DoForConnectedComponents(delegate(ComponentInfo componentInfo) {
                componentInfo.ComponentHostProxy.OnCentralManagerStopped();
            });

            dispatcher.Stop();
        }

        public ComponentInfo[] GetComponentsInfo()
        {
            ComponentInfo[] result;
            lock (stateLock) {
                result = knownComponentsInfo.Values.ToArray();
            }
            return result;
        }

        #endregion

        internal void OnComponentStarted(ComponentInfo componentInfo)
        {
            if (componentInfo.ComponentType == ComponentType.Processor) {
                DispatchReceivedMessages();
            }
        }

        internal void OnComponentStopped(ComponentInfo componentInfo)
        {
        }

        private void DispatchReceivedMessages()
        {
            Token[] tokens = storage.GetReceivedTokens();
            foreach (var token in tokens) {
                dispatcher.Dispatch(token);
            }
        }

        private void DoForConnectedComponents(Action<ComponentInfo> action)
        {
            ComponentInfo[] connectedComponentsInfo;
            lock (stateLock) {
                connectedComponentsInfo = knownComponentsInfo.Values.Where(c => c.State.Status != ComponentStatus.Disconnected).ToArray();
            }
            foreach (ComponentInfo componentInfo in connectedComponentsInfo) {
                action(componentInfo);
            }
        }

        private ComponentInfo GetComponentInfo(string name)
        {
            ComponentInfo componentInfo = null;
            lock (stateLock) {
                if (knownComponentsInfo.ContainsKey(name)) {
                    componentInfo = knownComponentsInfo[name];
                }
            }
            if (componentInfo == null) {
                throw new ArgumentException("Do not know component with given name.");
            }
            return componentInfo;
        }

        private void UpdateKnownComponentsAccordingToConfig()
        {
            ComponentInfo[] oldComponentsInfo;
            lock (stateLock) {
                oldComponentsInfo = knownComponentsInfo.Values.ToArray();
            }

            List<ComponentInfo> newComponentsInfo = new List<ComponentInfo>();
            ApplicationConfiguration config = GetConfiguration();
            var componentsNames = config.GetComponentNames();
            foreach (string componentName in componentsNames) {
                ComponentInfo componentInfo = oldComponentsInfo.FirstOrDefault(c => c.Name == componentName);
                if (componentInfo == null) {
                    componentInfo = new ComponentInfo(this, componentName, config.GetComponentType(componentName));
                }
                newComponentsInfo.Add(componentInfo);
            }

            lock (stateLock) {
                knownComponentsInfo.Clear();
                foreach (var componentInfo in newComponentsInfo) {
                    knownComponentsInfo.Add(componentInfo.Name, componentInfo);
                }
            }
        }
    }
}
