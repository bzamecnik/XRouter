using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XRouter.Test.Common
{
    class ThreadXRouterRunner : DaemonNtDebugXRouterRunner
    {
        public ThreadXRouterRunner(string daemonNtConfigFile, string serviceName)
            : base(daemonNtConfigFile, serviceName)
        {
        }

        #region IXRouterRunner Members

        public override void Start()
        {
            // start the debug host in a new thread because it blocks while
            // reading from the console
            Task.Factory.StartNew(() => DaemonNt.DebugStart(ServiceName));

            // TODO: let the service start
            Thread.Sleep(1000);
        }

        public override void Stop()
        {
            DaemonNt.DebugStop(ServiceName);
        }

        #endregion
    }
}
