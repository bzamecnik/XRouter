using System;

namespace DaemonNT.Logging
{
    public sealed class OnStopTraceLoggerStorageArgs
    {
        public Boolean Shutdown { internal set; get; }

        internal OnStopTraceLoggerStorageArgs()
        { }
    }
}
