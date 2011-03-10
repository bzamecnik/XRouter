using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using XRouter.Management;

namespace XRouter.Scheduler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {            
            var xrouterManager = XRouterManagerProvider.GetManager();

            string schedulerName = System.Configuration.ConfigurationManager.AppSettings["schedulerName"];
            var scheduler = new Implementation.Scheduler(xrouterManager, schedulerName);
        }
    }
}
