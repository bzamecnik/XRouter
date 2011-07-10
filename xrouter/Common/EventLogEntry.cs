using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT.Logging;
using System.Runtime.Serialization;

namespace XRouter.Common
{
    [DataContract]
    public class EventLogEntry
    {
        [DataMember]
        public DateTime Created { get; private set; }

        [DataMember]
        public LogType LogLevel { get; private set; }

        [DataMember]
        public string Message { get; private set; }

        internal EventLogEntry(string logFileLine, DateTime createdDate)
        {
            string line = logFileLine;

            string createdTimeText = ExtractStartUntilWhiteSpace(ref line);
            TimeSpan createdTime = TimeSpan.Parse(createdTimeText);
            Created = createdDate + createdTime;

            string logLevelText = ExtractStartUntilWhiteSpace(ref line);
            LogLevel = ParseLogLevel(logLevelText);

            Message = line;
        }

        private string ExtractStartUntilWhiteSpace(ref string text)
        {
            int firstWhiteSpacePos = 0;
            while ((firstWhiteSpacePos < text.Length) && (!char.IsWhiteSpace(text[firstWhiteSpacePos]))) {
                firstWhiteSpacePos++;
            }

            string result = text.Substring(0, firstWhiteSpacePos);

            if (firstWhiteSpacePos < text.Length) {
                text = text.Substring(firstWhiteSpacePos).TrimStart();
            } else {
                text = string.Empty;
            }

            return result;
        }

        internal static LogType ParseLogLevel(string logLevelText)
        {
            if (logLevelText == "I") {
                return DaemonNT.Logging.LogType.Info;
            } else if (logLevelText == "W") {
                return DaemonNT.Logging.LogType.Warning;
            } else if (logLevelText == "E") {
                return DaemonNT.Logging.LogType.Error;
            }
            throw new ArgumentException("Cannot parse log type.", "logLevelText");
        }
    }
}
