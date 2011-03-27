using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.Processor;

namespace XRouter.Prototype.CoreServices
{
    class ComponentInfo
    {
        public string Name { get; private set; }

        internal XmlReduction ConfigurationReduction { get; private set; }

        internal IComponentHost ComponentHostProxy { get; private set; }

        public ComponentType ComponentType { get; private set; }

        public ComponentState State { get; private set; }

        private CentralManager centralManager;

        public ComponentInfo(CentralManager centralManager, string name, ComponentType componentType)
        {
            this.centralManager = centralManager;
            Name = name;
            ComponentType = componentType;
            State = new ComponentState(name, ComponentStatus.Disconnected);
        }

        public void OnComponentConnected(IComponentHost componentHostProxy, XmlReduction configReduction, ComponentState state)
        {
            ConfigurationReduction = configReduction;
            ComponentHostProxy = componentHostProxy;
            State = state;
        }

        public void UpdateState()
        {
            if (ComponentHostProxy == null) {
                State = new ComponentState(Name, ComponentStatus.Disconnected);
            } else {
                State = ComponentHostProxy.GetState();
            }
        }

        public void Start()
        {
            if (ComponentHostProxy == null) {
                throw new InvalidOperationException("Cannot start disconnected component.");
            }
            ComponentHostProxy.Start();
            centralManager.OnComponentStarted(this);
        }

        public void Stop()
        {
            if (ComponentHostProxy == null) {
                throw new InvalidOperationException("Cannot stop disconnected component.");
            }
            ComponentHostProxy.Stop();
            centralManager.OnComponentStopped(this);
        }

        private IProcessor processorProxyCache = null;

        internal IProcessor GetProcessorProxy()
        {
            if (ComponentType != ComponentType.Processor) {
                throw new InvalidOperationException("Cannot get processor proxy from a component which has not a processor type.");
            }
            if (ComponentHostProxy == null) {
                throw new InvalidOperationException("Cannot access disconnected component.");
            }

            if (processorProxyCache != null) {
                return processorProxyCache;
            }
            IProcessor result = ComponentHostProxy.GetComponentAsProcessor();
            processorProxyCache = result;
            return result;
        }
    }
}
