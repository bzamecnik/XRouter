using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace DaemonNT.Logging
{
    /// <summary>
    /// Implementation of a multi-channel producer-consumer logger.
    /// </summary>
    internal sealed class LoggerImplementation
    {
        private ILoggerStorage[] storages;

        private BlockingCollection<Log> buffer;

        private ManualResetEvent mreStopPending;

        private Thread flushingWorker;

        private LoggerImplementation()
        { }

        public static LoggerImplementation Start(ILoggerStorage[] storages, Int32 bufferSize)
        {
            LoggerImplementation instance = new LoggerImplementation();
            instance.buffer = new BlockingCollection<Log>(bufferSize);
            instance.storages = storages;
            instance.mreStopPending = new ManualResetEvent(false);
            instance.flushingWorker = new Thread(new ParameterizedThreadStart(instance.Flushing));
            instance.flushingWorker.Start();

            return instance;
        }

        public void PushLog(Log log)
        {
            this.buffer.Add(log);
        }

        private void Flushing(object data)
        {
            foreach (Log log in this.buffer.GetConsumingEnumerable())
            {
                foreach (ILoggerStorage storage in this.storages)
                {
                    storage.SaveLog(log);
                }
            }

            // notifies the thread hosting the Stop() method that it can
            // continue running
            this.mreStopPending.Set();
        }

        public void Stop()
        {
            this.buffer.CompleteAdding();

            // waits for the consumer to finish its work
            this.mreStopPending.WaitOne();
        }
    }
}
