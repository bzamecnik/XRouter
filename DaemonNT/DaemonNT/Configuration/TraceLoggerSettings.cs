using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Configuration
{
    internal sealed class TraceLoggerSettings
    {
        public int BufferSize { set; get; }

        public List<TraceLoggerStorageSettings> Storages { private set; get; }

        public TraceLoggerSettings()
        {
            this.BufferSize = 1000;
            this.Storages = new List<TraceLoggerStorageSettings>();
        }
    }
}
