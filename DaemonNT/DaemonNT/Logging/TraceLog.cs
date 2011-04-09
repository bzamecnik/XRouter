namespace DaemonNT.Logging
{
    using System;

    internal class TraceLog
    {
        private TraceLog()
        {
        }

        public DateTime DateTime { get; private set; }

        public int ThreadId { get; private set; }

        public string ThreadName { get; private set; }

        public string Content { get; private set; }

        public static TraceLog Create(string xmlContent)
        {
            TraceLog traceLog = new TraceLog();
            traceLog.DateTime = DateTime.Now;
            traceLog.Content = xmlContent;
            traceLog.ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            traceLog.ThreadName = System.Threading.Thread.CurrentThread.Name;

            return traceLog;
        }
    }
}
