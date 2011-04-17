using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;  
using System.Threading;

namespace DaemonNT.Logging
{   
    public delegate void LogSaved(Log log);

    /// <summary>
    /// Poskytuje obecnou implementaci efektivního, více-vláknového logování. 
    /// </summary>
    internal sealed class LoggerImplementation
    {       
        private LoggerFileStorage storage;

        private BlockingCollection<Log> buffer;

        private ManualResetEvent mreStopPending;

        private Thread flushingWorker;
    
        private LoggerImplementation()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static LoggerImplementation Start(LoggerFileStorage storage, Int32 bufferSize)
        {
            LoggerImplementation instance = new LoggerImplementation();      
            instance.buffer = new BlockingCollection<Log>(bufferSize);
            instance.storage = storage;     
            instance.mreStopPending = new ManualResetEvent(false);
            instance.flushingWorker = new Thread(new ParameterizedThreadStart(instance.Flushing));
            instance.flushingWorker.Start();

            return instance;
        }
                                    
        public void PushLog(Log log)
        {
            this.buffer.Add(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void Flushing(object data)
        {
            foreach (Log log in this.buffer.GetConsumingEnumerable())
            {
                storage.SaveLog(log);              
            }

            this.mreStopPending.Set();
        }

        public void Stop()
        {
            this.buffer.CompleteAdding();
            this.mreStopPending.WaitOne();
        }
    }
}
