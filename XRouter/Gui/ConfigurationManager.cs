using XRouter.Common;
using XRouter.Gui.ConfigurationControls.Gateway;
using XRouter.Gui.ConfigurationControls.Messageflow;
using XRouter.Gui.ConfigurationControls.Processor;
using XRouter.Manager;

namespace XRouter.Gui
{
    class ConfigurationManager
    {
        public static readonly string DefaultConsoleServerUri =
            "http://localhost:9090/XRouter.ConsoleService/ConsoleServer";
        public string ConsoleServerUri { get; set; }

        public IConsoleServer ConsoleServer { get; private set; }
        public ApplicationConfiguration Configuration { get; set; }

        public ConfigurationManager()
        {
            string uri = System.Configuration.ConfigurationManager.AppSettings.Get("consoleServerUri");
            if (uri == null)
            {
                uri = DefaultConsoleServerUri;
            }
            ConsoleServerUri = uri;
        }

        public void Connect()
        {
            ConsoleServer = ConsoleServerProxyProvider.GetConsoleServerProxy(ConsoleServerUri);
        }

        public void LoadConfigurationFromServer()
        {
            Configuration = ConsoleServer.GetConfiguration();

            ////TODO: remove
            //Configuration.AddAdapterType(new AdapterType(
            //    "Directory I/O adapter",
            //    "XRouter.Adapters.DirectoryAdapter,XRouter.Adapters.dll",
            //    new ObjectConfigurator.ClassMetadata(typeof(XRouter.Adapters.DirectoryAdapter))));
            //Configuration.AddAdapterType(new AdapterType(
            //    "HTTP client adapter",
            //    "XRouter.Adapters.HttpClientAdapter,XRouter.Adapters.dll",
            //    new ObjectConfigurator.ClassMetadata(typeof(XRouter.Adapters.HttpClientAdapter))));
        }

        public void SaveConfigurationToServer()
        {
            ConsoleServer.ChangeConfiguration(Configuration);
        }

        public ConfigurationTreeItem CreateConfigurationTreeRoot()
        {
            ConfigurationTreeItem rootItem = new ConfigurationTreeItem("Configuration", null, null, this);

            // NOTE: get names of the first gateway and processor component
            // as we assume there is only one gateway and one processor
            string gatewayName = Configuration.GetComponentElements(ComponentType.Gateway)
                .Element("gateway").Attribute("name").Value;
            string processorName = Configuration.GetComponentElements(ComponentType.Processor)
                .Element("processor").Attribute("name").Value;

            ConfigurationTreeItem gatewayItem = new ConfigurationTreeItem("Gateway",
                () => new GatewayConfigurationControl(gatewayName), rootItem, this);
            ConfigurationTreeItem processorItem = new ConfigurationTreeItem("Processor",
                () => new ProcessorConfigurationControl(processorName), rootItem, this);
            ConfigurationTreeItem mesageflowItem = new ConfigurationTreeItem("Message flow",
                () => new MessageflowConfigurationControl(), rootItem, this);

            return rootItem;
        }
    }
}
