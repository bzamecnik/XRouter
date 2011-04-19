using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Configuration
{
    internal sealed class TraceLoggerStorageSettings
    {
        public TraceLoggerStorageSettings()
        { }
        
        public String Name { set; get; }

        public string TypeClass { get; set; }

        public string TypeAssembly { get; set; }

        public Settings Settings { get; set; }
    }
}
