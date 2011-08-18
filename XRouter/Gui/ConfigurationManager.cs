using XRouter.Common;
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

            // TODO: the component names should NOT be hard-wired!
            ConfigurationTreeItem gatewayItem = new ConfigurationTreeItem("Gateway", () => new ConfigurationControls.Gateway.GatewayConfigurationControl("gateway"), rootItem, this);
            ConfigurationTreeItem processorItem = new ConfigurationTreeItem("Processor", () => new ConfigurationControls.Processor.ProcessorConfigurationControl("processor"), rootItem, this);
            ConfigurationTreeItem mesageflowItem = new ConfigurationTreeItem("Messageflow", () => new ConfigurationControls.Messageflow.MessageflowConfigurationControl(), rootItem, this);

            return rootItem;
        }
    }
}
