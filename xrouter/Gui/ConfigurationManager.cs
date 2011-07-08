using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using System.Xml.Linq;

namespace XRouter.Gui
{
	

	public class ConfigurationTree
	{
		public string Name { get; private set; }
		public IConfigurationControl Control { get; private set; }
		public List<ConfigurationTree> Children { get; private set; }
        public ConfigurationTree Parent { get; private set; }
        public XElement XContent { get; private set; }

        public ConfigurationTree(string Name, IConfigurationControl Control, ConfigurationTree parent, XElement xContent)
		{
			this.Name = Name;
			this.Control = Control;
            this.Parent = parent;
            this.XContent = xContent;
			Children = new List<ConfigurationTree>();
		}
	}

	public static class ConfigurationManager
	{
        public static ApplicationConfiguration ApplicationConfiguration { get; private set; }
        public static IBrokerServiceForManagement BrokerService { get; private set; }

		public static ConfigurationTree LoadConfigurationTree()
		{
            BrokerService = new XRouter.Gui.Service.BrokerServiceForManagementClient();
            ApplicationConfiguration = BrokerService.GetConfiguration();
            var gws = ApplicationConfiguration.GetComponentsElement(Common.ComponentType.Gateway);

			IConfigurationControl adaptercontrol = (IConfigurationControl)ConfigurationControlManager.LoadUserControlFormFile("XRouter.Gui.exe", "XRouter.Gui.ConfigurationControls.Adapter.AdapterConfigurationControl");
            IConfigurationControl gatewaycontrol = (IConfigurationControl)ConfigurationControlManager.LoadUserControlFormFile("XRouter.Gui.exe", "XRouter.Gui.ConfigurationControls.Gateway.GatewayConfigurationControl");
            IConfigurationControl dispatchercontrol = (IConfigurationControl)ConfigurationControlManager.LoadUserControlFormFile("XRouter.Gui.exe", "XRouter.Gui.ConfigurationControls.Dispatcher.DispatcherConfigurationControl");
            IConfigurationControl messageflowcontrol = (IConfigurationControl)ConfigurationControlManager.LoadUserControlFormFile("XRouter.Gui.exe", "XRouter.Gui.ConfigurationControls.Messageflow.MessageflowConfigurationControl");
			//ConfigurationControlManager.LoadUserControlFormFile("DirectoryAdapterConfigurationControl.dll", "DirectoryAdapterConfigurationControl.ConfigurationControl")


			ConfigurationTree node_Root = new ConfigurationTree("Konfigurace", null, null, null);

            // Load Gateways
            ConfigurationTree node_Gateways = new ConfigurationTree("Gateways", null, node_Root, null);
            node_Root.Children.Add(node_Gateways);

            foreach (XElement xGateway in ApplicationConfiguration.GetComponentsElement(ComponentType.Gateway).Elements()) {
                string gatewayName = xGateway.Attribute(XName.Get("name")).Value;
                ConfigurationTree node_Gateway = new ConfigurationTree(gatewayName, gatewaycontrol, node_Gateways, xGateway);
                node_Gateways.Children.Add(node_Gateway);

                var xAdapters = xGateway.Element(XName.Get("adapters")).Elements();
                foreach (var xAdapter in xAdapters) {
                    string adapterName = xGateway.Attribute(XName.Get("name")).Value;
                    ConfigurationTree node_Adapter = new ConfigurationTree(adapterName, adaptercontrol, node_Gateway, xAdapter);
                    node_Gateway.Children.Add(node_Adapter);
                }
            }
          

            ConfigurationTree dispatcher = new ConfigurationTree("Dispatcher 1", dispatchercontrol, node_Root, null);
			
			node_Root.Children.Add(dispatcher);

            node_Root.Children.Add(new ConfigurationTree("MessageFlow", messageflowcontrol, node_Root, null));
            node_Root.Children.Add(new ConfigurationTree("Seznam dostupných adaptérů", null, node_Root, null));


			// Nacteni Processor

			// Nacteni Dispatcher




			return node_Root;		
		}

		public static void GetAllowedAdapters()
		{
		// Nacteni dostupnych adapteru
		}
	}
}
