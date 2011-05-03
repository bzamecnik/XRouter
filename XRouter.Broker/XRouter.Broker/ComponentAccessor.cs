using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using ObjectRemoter;

namespace XRouter.Broker
{
    abstract class ComponentAccessor
    {
        public string ComponentName { get; private set; }

        public ComponentType ComponentType { get; private set; }

        public XmlReduction ConfigurationReduction { get; set; }

        public Uri ComponentAddress { get; set; }

        public Uri ControllerAddress { get; set; }

        protected ComponentAccessor(string componentName, ApplicationConfiguration configuration)
        {
            ComponentName = componentName;
            ComponentType = configuration.GetComponentType(componentName);
            ConfigurationReduction = new XmlReduction();
            ComponentAddress = configuration.GetComponentAddress(componentName);
            ControllerAddress = configuration.GetComponentControllerAddress(componentName);
        }

        public static ComponentAccessor Create(string componentName, ApplicationConfiguration configuration)
        {
            ComponentType componentType = configuration.GetComponentType(componentName);
            switch (componentType) {
                case ComponentType.Gateway:
                    return new GatewayAccessor(componentName, configuration);
                case ComponentType.Processor:
                    return new ProcessorAccessor(componentName, configuration);
                default:
                    throw new InvalidOperationException("Unknown component type.");
            }
        }

        public void Start()
        {
            IComponentController controller = GetController();
            controller.StartComponent();
        }

        public void Stop()
        {
            IComponentController controller = GetController();
            controller.StopComponent();
        }

        public bool IsRunning()
        {
            IComponentController controller = GetController();
            bool isRunning = controller.IsComponentRunning();
            return isRunning;
        }

        public void UpdateConfig(ApplicationConfiguration config)
        {
            IComponentService component = GetComponent<IComponentService>();
            component.UpdateConfig(config);
        }

        private IComponentController GetController()
        {
            IComponentController controller = ServiceRemoter.GetServiceProxy<IComponentController>(ControllerAddress);
            return controller;
        }

        protected T GetComponent<T>()
            where T : IComponentService
        {
            T component = ServiceRemoter.GetServiceProxy<T>(ComponentAddress);
            return component;
        }
    }
}
