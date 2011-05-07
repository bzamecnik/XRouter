using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT.Logging;

namespace DaemonNT.Test
{
    /// <summary>
    /// An example trivial trace logger storage implementation.
    /// It shows how to save traces into debug mode console.
    /// </summary>
    public class TestTraceLogger : TraceLoggerStorage
    {
        private Boolean isDebugMode;

        private String storageName;

        protected override void OnStart(OnStartTraceLoggerStorageArgs args)
        {
            this.isDebugMode = args.IsDebugMode;
            this.storageName = args.StorageName;

            if (this.isDebugMode)
            {
                Console.WriteLine(String.Format("Initialized trace-logger-storage Name={0}.",
                    this.storageName));
                Console.WriteLine(args.Settings.Parameter["x"]);
            }
        }

        protected override void OnSaveLog(TraceLog log)
        {
            if (this.isDebugMode)
            {
                Console.WriteLine(String.Format("{0}: {1}",
                    log.DateTime.ToString("HH-mm-ss"), log.Content));
            }
        }

        protected override void OnStop(OnStopTraceLoggerStorageArgs args)
        {
            if (this.isDebugMode)
            {
                Console.WriteLine(String.Format("Destroyed trace-logger-storage Name={0}.",
                    this.storageName));
            }
        }
    }
}
