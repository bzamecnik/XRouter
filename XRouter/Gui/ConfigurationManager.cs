using XRouter.Common;
using XRouter.Gui.ConfigurationControls.Gateway;
using XRouter.Gui.ConfigurationControls.Messageflow;
using XRouter.Gui.ConfigurationControls.Processor;
using XRouter.Manager;
using System.Xml.Linq;
using System;
using System.Windows;

namespace XRouter.Gui
{
    class ConfigurationManager
    {
        private static readonly string DefaultConsoleServerUri =
            "http://localhost:9090/XRouter.ConsoleService/ConsoleServer";

        public string DefaultConsoleServerAddress {
            get {
                string result= System.Configuration.ConfigurationManager.AppSettings.Get("consoleServerUri");
                if (result == null) {
                    result = DefaultConsoleServerUri;
                }
                return result;
            }
        }

        public IConsoleServer ConsoleServer { get; private set; }
        public ApplicationConfiguration Configuration { get; private set; }

        public ConfigurationManager()
        {
        }

        public bool Connect(string consoleServerUri, bool throwOnError)
        {
            try {
                ConsoleServer = ConsoleServerProxyProvider.GetConsoleServerProxy(consoleServerUri);
                ConsoleServer.GetXRouterServiceStatus();
                return true;
            } catch (Exception ex) {
                if (throwOnError) {
                    throw;
                } else {
                    string message = string.Format(
@"It is not possible to connect to XRouter Manager server at:
{0}.
Details:
{1}", consoleServerUri, ex.Message);
                    MessageBox.Show(ex.Message, "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public bool DownloadConfiguration(bool throwOnError)
        {
            try {
                Configuration = ConsoleServer.GetConfiguration();
                return true;
            } catch (Exception ex) {
                if (throwOnError) {
                    throw;
                } else {
                    string message = string.Format("Cannot download configuration from XRouter manager: " + ex.Message);
                    MessageBox.Show(ex.Message, "Operation failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public bool UploadConfiguration(bool throwOnError)
        {
            try {
                ConsoleServer.ChangeConfiguration(Configuration);
                return true;
            } catch (Exception ex) {
                if (throwOnError) {
                    throw;
                } else {
                    string message = string.Format("Cannot upload configuration to XRouter manager: " + ex.Message);
                    MessageBox.Show(ex.Message, "Operation failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public void ClearConfiguration()
        {
            XDocument xConfig = XDocument.Parse(ApplicationConfiguration.InitialContent);
            Configuration = new ApplicationConfiguration(xConfig);
        }

        public bool ReadConfiguration(XDocument xConfig, bool throwOnError)
        {
            try {
                Configuration = new ApplicationConfiguration(xConfig);
                return true;
            } catch (Exception ex) {
                if (throwOnError) {
                    throw;
                } else {
                    string message = string.Format("Cannot read configuration: " + ex.Message);
                    MessageBox.Show(ex.Message, "Operation failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
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
