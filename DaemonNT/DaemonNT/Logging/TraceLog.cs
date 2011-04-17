namespace DaemonNT.Logging
{
    using System;
    using System.Text;
  
    public class TraceLog : Log
    {                
        public int ThreadId { get; private set; }

        public string ThreadName { get; private set; }

        public string Content { get; private set; }

        private string logStr;

        private TraceLog()
        {
        }

        public static TraceLog Create(string content)
        {
            TraceLog traceLog = new TraceLog();
            traceLog.DateTime = DateTime.Now;
            traceLog.Content = content;
            traceLog.ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            traceLog.ThreadName = System.Threading.Thread.CurrentThread.Name;
            traceLog.logStr = traceLog.SerializeToStr();

            return traceLog;
        }

        private string SerializeToStr()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<log ");
            sb.Append(string.Format("date-time=\"{0}\" ", this.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.ff")));
            sb.Append(string.Format("thread-id=\"{0}\" ", this.ThreadId));
            if (!string.IsNullOrEmpty(this.ThreadName))
            {
                sb.Append(string.Format("thread-name=\"{0}\" ", this.ThreadName));
            }

            sb.Append(">");
            sb.Append(this.Content);
            sb.Append("</log>");

            return sb.ToString();
        }

        public override string ToString()
        {
            return this.logStr;
        }
    }
}
