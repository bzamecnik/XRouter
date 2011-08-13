using System.Windows;
using XRouter.Management;

namespace XRouter.Gateway
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var xrouterManager = XRouterManagerProvider.GetManager();

            string gatewayName = System.Configuration.ConfigurationManager.AppSettings["gatewayName"];
            var gatewayService = new Implementation.Gateway(xrouterManager, gatewayName);

            string dispatcherName = System.Configuration.ConfigurationManager.AppSettings["dispatcherName"];
            var dispatcher = new Dispatcher.Implementation.Dispatcher(xrouterManager, dispatcherName);

            string processorName = System.Configuration.ConfigurationManager.AppSettings["processorName"];
            var messageProcessor = new Processor.Implementation.Processor(xrouterManager, processorName);

            string processingProviderName = System.Configuration.ConfigurationManager.AppSettings["processingProviderName"];
            var processingProvider = new Processor.Implementation.Broadcaster(xrouterManager, processingProviderName);
        }
    }
}
