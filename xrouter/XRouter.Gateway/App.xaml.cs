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
            string xrouterManagerAddress = System.Configuration.ConfigurationManager.AppSettings["xrouterManagerAddress"];
            var xrouterManager = XRouterManagerProvider.GetManager(xrouterManagerAddress);

            string gatewayName = System.Configuration.ConfigurationManager.AppSettings["gatewayName"];
            var gatewayService = new Implementation.Gateway(xrouterManager, gatewayName);

            //string schedulerName = System.Configuration.ConfigurationManager.AppSettings["schedulerName"];
            //var scheduler = new Scheduler.Implementation.Scheduler(xrouterManager, schedulerName);

            string messageProcessorName = System.Configuration.ConfigurationManager.AppSettings["messageProcessorName"];
            var messageProcessor = new MessageProcessor.Implementation.BroadcastingMessageProcessor(xrouterManager, messageProcessorName);
        }
    }
}
