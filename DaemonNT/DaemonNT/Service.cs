using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT
{
    /// <summary>
    /// Provides an abstract class for a DaemonNT service that will exist as part as 
    /// a service application.
    /// </summary>
    public abstract class Service
    {               
        internal void StartCommand(String serviceName, Boolean isDebugMode, DaemonNT.Logging.Logger logger,
            DaemonNT.Configuration.Setting setting)
        {
            ServiceArgs args = new ServiceArgs(serviceName, isDebugMode, logger, setting);
            this.OnStart(args);            
        }

        protected virtual void OnStart(ServiceArgs args)
        { }

        internal void StopCommand(Boolean shutdown)
        {
            this.OnStop(shutdown);
        }

        protected virtual void OnStop(Boolean shutdown)
        { }              
    }
}
