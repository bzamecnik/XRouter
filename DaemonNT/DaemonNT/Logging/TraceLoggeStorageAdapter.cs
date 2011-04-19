using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    internal class TraceLoggerStorageAdapter : ILoggerStorage
    {
        private TraceLoggerStorage storage;

        public TraceLoggerStorageAdapter(TraceLoggerStorage storage)
        {
            this.storage = storage;
        }

        public void Start(String storageName, Boolean isDebugMode, DaemonNT.Configuration.Settings settings)
        {
            OnStartTraceLoggerStorageArgs args = new OnStartTraceLoggerStorageArgs();
            args.StorageName = storageName;
            args.IsDebugMode = isDebugMode;
            args.Settings = settings;

            storage.Start(args);
        }

        public void SaveLog(Log log)
        {
            this.storage.SaveLog((TraceLog)log);
        }

        public void Stop(Boolean shutdown)
        {
            OnStopTraceLoggerStorageArgs args = new OnStopTraceLoggerStorageArgs();
            args.Shutdown = shutdown;

            storage.Stop(args);
        }
    }
}
