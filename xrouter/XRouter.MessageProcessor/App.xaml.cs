using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using XRouter.Management;

namespace XRouter.MessageProcessor
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

            string messageProcessorName = System.Configuration.ConfigurationManager.AppSettings["messageProcessorName"];
            var messageProcessor = new Implementation.BroadcastingMessageProcessor(xrouterManager, messageProcessorName);
        }
    }
}
