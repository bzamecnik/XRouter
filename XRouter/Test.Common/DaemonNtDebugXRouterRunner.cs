using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT;

namespace XRouter.Test.Common
{
    abstract class DaemonNtDebugXRouterRunner :IXRouterRunner 
    {        
        /// <summary>
        /// DaemonNT configuration file containing the XRouter service.
        /// </summary>
        public string DaemonNtConfigFile { get; private set; }

        /// <summary>
        /// XRouter service name as configured in the DaemonNT configuration
        /// file.
        /// </summary>
        public string ServiceName { get; private set; }

        protected ServiceCommands DaemonNt { get; set; }

        public DaemonNtDebugXRouterRunner(string daemonNtConfigFile, string serviceName)
        {
            ServiceName = serviceName;
            DaemonNtConfigFile = daemonNtConfigFile;
            DaemonNt = new ServiceCommands() { ConfigFile = daemonNtConfigFile };
        }

        #region IXRouterRunner Members

        public abstract void Start();

        public abstract void Stop();

        #endregion
    }
}
