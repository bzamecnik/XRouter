using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using XRouter.Management;

namespace XRouter.Dispatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {            
            var xrouterManager = XRouterManagerProvider.GetManager();

            string dispatcherName = System.Configuration.ConfigurationManager.AppSettings["dispatcherName"];
            var dispatcher = new Implementation.Dispatcher(xrouterManager, dispatcherName);
        }
    }
}
