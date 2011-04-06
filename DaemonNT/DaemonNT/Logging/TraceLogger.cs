using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace DaemonNT.Logging
{
    /// <summary>
    /// Poskytuje efektivní bufferované thread-safe logování low-level informací, které bude 
    /// potenciálně zkoumat vývojář či technicky pokročilá osoba. Logování může být přizpůsobeno 
    /// problémové doméně. Výsledné záznamy je možno zkoumat programově. 
    /// </summary>
    public sealed class TraceLogger
    {
        private LoggerFileStorage storage = null;
        
        private BlockingCollection<TraceLog> buffer = null;

        private ManualResetEvent mreStopPending = null;

        private Thread flushingWorker = null;
        
        private TraceLogger()
        { }
       
        internal static TraceLogger Start(String serviceName)
        {
            TraceLogger instance = new TraceLogger();
            instance.buffer = new BlockingCollection<TraceLog>(1000);
            instance.storage = new LoggerFileStorage(String.Format("{0}.Trace", serviceName));
            instance.mreStopPending = new ManualResetEvent(false);
            instance.flushingWorker = new Thread(new ParameterizedThreadStart(instance.Flushing));
            instance.flushingWorker.Start();

            return instance;
        }

        private void PushLog(TraceLog log)
        {
            this.buffer.Add(log);
        }

        private void Flushing(Object data)
        {
            foreach (TraceLog log in this.buffer.GetConsumingEnumerable())
            {
                this.storage.Save(log.DateTime, this.SerializeLog(log));
            }

            this.mreStopPending.Set();
        }
                       
        internal void Stop()
        {
            this.buffer.CompleteAdding();
            this.mreStopPending.WaitOne();
        }

        private String SerializeLog(TraceLog log)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<log ");            
            sb.Append(String.Format("date-time=\"{0}\" ", log.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.ff")));
            sb.Append(String.Format("thread-id=\"{0}\" ", log.ThreadId));
            if (!String.IsNullOrEmpty(log.ThreadName))
            {
                sb.Append(String.Format("thread-name=\"{0}\" ", log.ThreadName));
            }
            sb.Append(">");
            sb.Append(log.Content);
            sb.Append("</log>");

            return sb.ToString();
        }
        
        public void Log(String xmlElement)
        {
            TraceLog log = TraceLog.Create(xmlElement);
         
            this.PushLog(log);
        }             
    }
}
