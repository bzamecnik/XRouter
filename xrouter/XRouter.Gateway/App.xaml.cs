using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using XRouter.Management;
using System.Xml.Linq;

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

            string messageProcessorName = System.Configuration.ConfigurationManager.AppSettings["messageProcessorName"];
            var messageProcessor = new MessageProcessor.Implementation.BroadcastingMessageProcessor(xrouterManager, messageProcessorName);
        }
    }
}
