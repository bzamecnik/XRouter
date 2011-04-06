using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT
{
    internal sealed class ServiceDebugHost
    {
         private String serviceName;

         private DaemonNT.Configuration.ServiceSetting serviceSetting = null;

         private DaemonNT.Logging.Logger logger = null;

         private Service service = null;

         public ServiceDebugHost(String serviceName, DaemonNT.Configuration.ServiceSetting serviceSetting,
             DaemonNT.Logging.Logger logger, Service service)
         {
             this.serviceName = serviceName;
             this.serviceSetting = serviceSetting;
             this.logger = logger;
             this.service = service;
         }

         internal void Start()
         {
             this.service.StartCommand(serviceName, true, this.logger, this.serviceSetting.Setting);
         }

         internal void Stop()
         {
             this.service.StopCommand(false);
         }
    }
}
