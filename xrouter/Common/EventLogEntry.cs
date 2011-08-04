using System;
using System.Runtime.Serialization;
using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Represents a single serializable event entry of the event log.
    /// The message can be an unstructured text.
    /// </summary>
    [DataContract]
    public class EventLogEntry
    {
        /// <summary>
        /// Date and time when the log entry was created.
        /// </summary>
        [DataMember]
        public DateTime Created { get; private set; }

        /// <summary>
        /// Type or importance of the entry. See <see
        /// cref="DaemonNT.Logging.LogType"/> for possible log types.
        /// </summary>
        [DataMember]
        public LogType LogLevel { get; private set; }

        /// <summary>
        /// Text contents of the log entry.
        /// </summary>
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
