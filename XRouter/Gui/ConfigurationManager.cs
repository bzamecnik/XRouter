using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using System.Xml.Linq;
using System.Xml;
using XRouter.Manager;

namespace XRouter.Gui
{
	class ConfigurationManager
	{
        // TODO: this should be configurable!
        private static readonly string ConsoleServerUri = "http://localhost:9090/XRouter.ConsoleService/ConsoleServer";

        public IConsoleServer ConsoleServer { get; private set; }
        public ApplicationConfiguration Configuration { get; private set; }

        public ConfigurationManager()
        {
            ConsoleServer = ConsoleServerProxyProvider.GetConsoleServerProxy(ConsoleServerUri);
        }

        public void LoadConfigurationFromServer()
        {
            Configuration = ConsoleServer.GetConfiguration();

            //TODO: remove
            Configuration.AddAdapterType(new AdapterType(
                "Directory I/O adapter",
                "XRouter.Adapters.DirectoryAdapter,XRouter.Adapters.dll",
                new ObjectConfigurator.ClassMetadata(typeof(XRouter.Adapters.DirectoryAdapter))));
            Configuration.AddAdapterType(new AdapterType(
                "HTTP client adapter",
                "XRouter.Adapters.HttpClientAdapter,XRouter.Adapters.dll",
                new ObjectConfigurator.ClassMetadata(typeof(XRouter.Adapters.HttpClientAdapter))));
        }

		public ConfigurationTreeItem CreateConfigurationTreeRoot()
        {
            ConfigurationTreeItem rootItem = new ConfigurationTreeItem("Configuration", null, null, this);

            ConfigurationTreeItem gatewayItem = new ConfigurationTreeItem("Gateway", () => new ConfigurationControls.Gateway.GatewayConfigurationControl("gateway"), rootItem, this);
            ConfigurationTreeItem processorItem = new ConfigurationTreeItem("Processor", () => new ConfigurationControls.Processor.ProcessorConfigurationControl("processor"), rootItem, this);
            ConfigurationTreeItem mesageflowItem = new ConfigurationTreeItem("Messageflow", () => new ConfigurationControls.Messageflow.MessageflowConfigurationControl(), rootItem, this);

            return rootItem;	
		}

        public void SaveConfiguration()
        {
            ConsoleServer.ChangeConfiguration(Configuration);
        }
	}
}
