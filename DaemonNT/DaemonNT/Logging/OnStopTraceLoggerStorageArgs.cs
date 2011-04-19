using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    public sealed class OnStopTraceLoggerStorageArgs
    {
        public Boolean Shutdown { internal set; get; }

        internal OnStopTraceLoggerStorageArgs()
        { }
    }
}
