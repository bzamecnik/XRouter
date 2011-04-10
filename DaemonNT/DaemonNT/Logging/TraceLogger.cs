namespace DaemonNT.Logging
{
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Poskytuje efektivní bufferované thread-safe logování low-level informací, které bude 
    /// potenciálně zkoumat vývojář či technicky pokročilá osoba. Logování může být přizpůsobeno 
    /// problémové doméně. Výsledné záznamy je možno zkoumat programově. 
    /// </summary>
    public sealed class TraceLogger
    {
        private LoggerFileStorage storage;

        private BlockingCollection<TraceLog> buffer;

        private ManualResetEvent mreStopPending;

        private Thread flushingWorker;

        private TraceLogger()
        {
        }

        public void Log(string xmlElement)
        {
            TraceLog log = TraceLog.Create(xmlElement);
            this.PushLog(log);
        }

        internal static TraceLogger Start(string serviceName)
        {
            TraceLogger instance = new TraceLogger();
            instance.buffer = new BlockingCollection<TraceLog>(1000);
            instance.storage = new LoggerFileStorage(string.Format("{0}.Trace", serviceName));
            instance.mreStopPending = new ManualResetEvent(false);
            instance.flushingWorker = new Thread(new ParameterizedThreadStart(instance.Flushing));
            instance.flushingWorker.Start();

            return instance;
        }

        internal void Stop()
        {
            this.buffer.CompleteAdding();
            this.mreStopPending.WaitOne();
        }

        private void PushLog(TraceLog log)
        {
            this.buffer.Add(log);
        }

        private void Flushing(object data)
        {
            foreach (TraceLog log in this.buffer.GetConsumingEnumerable())
            {
                this.storage.Save(log.DateTime, this.SerializeLog(log));
            }

            this.mreStopPending.Set();
        }

        private string SerializeLog(TraceLog log)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<log ");
            sb.Append(string.Format("date-time=\"{0}\" ", log.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.ff")));
            sb.Append(string.Format("thread-id=\"{0}\" ", log.ThreadId));
            if (!string.IsNullOrEmpty(log.ThreadName))
            {
                sb.Append(string.Format("thread-name=\"{0}\" ", log.ThreadName));
            }

            sb.Append(">");
            sb.Append(log.Content);
            sb.Append("</log>");

            return sb.ToString();
        }
    }
}
