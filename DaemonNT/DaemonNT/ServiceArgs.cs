using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT
{
    public sealed class ServiceArgs
    {
        public String ServiceName { private set; get; }

        public Boolean IsDebugMode { private set; get; }

        public DaemonNT.Logging.Logger Logger { private set; get; }

        public DaemonNT.Configuration.Setting Setting { private set; get; }

        internal ServiceArgs(String serviceName, Boolean isDebugMode, DaemonNT.Logging.Logger logger, 
            DaemonNT.Configuration.Setting setting)
        {
            this.ServiceName = serviceName;
            this.IsDebugMode = isDebugMode;
            this.Logger = logger;
            this.Setting = setting;
        }
    }
}
