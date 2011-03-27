using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;

namespace XRouter.Prototype.CoreServices
{
    class ComponentHost : IComponentHost
    {
        private bool IsCentralManagerRunning { get; set; }
        private bool IsComponentRunning { get; set; }
        private bool ShouldBeComponentRunning { get; set; }

        private string ComponentName { get; set; }

        private IComponent component;
        private ApplicationConfiguration configuration;

        private ICentralManager centralManagerProxy;

        public ComponentHost(ComponentType componentType, string componentName, ICentralManager centralManager)
        {
            ComponentName = componentName;
            if (componentType == ComponentType.Gateway) {
                component = new Gateway.GatewayImplementation();
            }
            if (componentType == ComponentType.Processor) {
                component = new Processor.ProcessorImplementation();
            }

            centralManagerProxy = centralManager;

            var connectionResult = centralManager.ConnectComponent(componentName, this, GetState(), component.ConfigReduction);
            IsCentralManagerRunning = true;

            ICentralComponentServices services = centralManager;
            component.Initalize(connectionResult.Configuration, services);
        }

        public void Start()
        {
            ShouldBeComponentRunning = true;
            if (IsCentralManagerRunning) {
                IsComponentRunning = true;
                component.Start();
            }
        }

        public void Stop()
        {
            ShouldBeComponentRunning = false;
            IsComponentRunning = false;
            component.Stop();
        }

        public void OnCentralManagerStarted()
        {
            IsCentralManagerRunning = true;
            if (ShouldBeComponentRunning && (!IsComponentRunning)) {
                IsComponentRunning = true;
                component.Start();
            }
        }

        public void OnCentralManagerStopped()
        {
            IsCentralManagerRunning = false;
            if (IsComponentRunning) {
                IsComponentRunning = false;
                component.Stop();
            }
        }

        public ComponentState GetState()
        {
            if (IsComponentRunning) {
                return new ComponentState(ComponentName, ComponentStatus.Running);
            } else {
                return new ComponentState(ComponentName, ComponentStatus.Stopped);
            }
        }

        public void ChangeConfig(ApplicationConfiguration config)
        {
            configuration = config;
        }

        public IProcessor GetComponentAsProcessor()
        {
            return (IProcessor)component;
        }
    }
}
