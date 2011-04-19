using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    public sealed class OnStartTraceLoggerStorageArgs
    {
        public String StorageName { internal set; get; }

        public Boolean IsDebugMode { internal set; get; }

        public DaemonNT.Configuration.Settings Settings { internal set; get; }

        internal OnStartTraceLoggerStorageArgs()
        { }
    }
}
