using System;

namespace DaemonNT.Logging
{
    /// <summary>
    /// Represents a single event log record (created by the EventLogger).
    /// </summary>
    internal sealed class EventLog : Log
    {
        public LogType LogType { private set; get; }

        public String Message { private set; get; }

        private String logStr = null;

        private EventLog()
        { }

        internal static EventLog Create(LogType logType, String message)
        {
            EventLog eventLog = new EventLog();
            eventLog.DateTime = DateTime.Now;
            eventLog.LogType = logType;
            eventLog.Message = message;
            eventLog.logStr = eventLog.SerializeToStr();

            return eventLog;
        }

        /// <summary>
        /// Prevede informace dane instance do Stringu.
        /// </summary>
        /// <returns></returns>
        private string SerializeToStr()
        {
            String dateTimeStr = this.DateTime.ToString("HH:mm:ss.ff");

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
                    throw new ArgumentException(String.Format("Unsupported log type '{0}'", logTypeStr));
            }

            // the event log record must not span multiple lines!
            string message = this.Message.Replace('\n', ' ').Replace('\r', ' ');
            return string.Format("{0}\t{1}\t{2}", dateTimeStr, logTypeStr, message);
        }

        public override string ToString()
        {
            return this.logStr;
        }
    }
}
