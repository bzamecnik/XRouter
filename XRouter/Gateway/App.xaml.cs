using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using XRouter.Common;
using ObjectRemoter;
using XRouter.Broker;

namespace XRouter.Gateway
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Uri brokerUri = new Uri(System.Configuration.ConfigurationManager.AppSettings["brokerUrl"]);
            var broker = ServiceRemoter.GetServiceProxy<IBrokerServiceForGateway>(brokerUri);

            string gatewayName = System.Configuration.ConfigurationManager.AppSettings["gatewayName"];
            var gatewayService = new Implementation.Gateway(broker, gatewayName);
            gatewayService.Start();

            Uri gatewayAddress = ObjectRemoter.ServiceRemoter.PublishService<IGatewayService>(gatewayService);
            broker.UpdateComponentInfo(gatewayService.Name, gatewayAddress, gatewayService.ConfigurationReduction);
        }
    }
}
