namespace DaemonNT.Logging
{
    using System;
    using System.Text;
  
    /// <summary>
    ///  Reprezentuje jeden trace log záznam vytvořený TraceLoggerem.
    /// </summary>
    public class TraceLog : Log
    {
        public LogType LogType { get; private set; }

        public int ThreadId { get; private set; }

        public string ThreadName { get; private set; }

        public string Content { get; private set; }

        private string logStr;

        private TraceLog()
        {
        }

        public static TraceLog Create(LogType logType, string content)
        {
            TraceLog traceLog = new TraceLog();
            traceLog.LogType = logType;
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
            
            String logTypeStr = null;
            switch (this.LogType)
            {
                case LogType.Info:
                    logTypeStr = "I";
                    break;
                case LogType.Warning:
                    logTypeStr = "W";
                    break;
                case LogType.Error:
                    logTypeStr = "E";
                    break;         
                default:
                    throw new ArgumentException(String.Format("Unsupported log type: {0}", logTypeStr));
            }

            sb.Append("<log");           
            sb.Append(string.Format(" date-time=\"{0}\"", this.DateTime.ToString("yyyy-MM-ddTHH:mm:ss.ff")));
            sb.Append(string.Format(" type=\"{0}\"", logTypeStr));
            sb.Append(string.Format(" thread-id=\"{0}\"", this.ThreadId));
            if (!string.IsNullOrEmpty(this.ThreadName))
            {
                sb.Append(string.Format(" thread-name=\"{0}\"", this.ThreadName));
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
