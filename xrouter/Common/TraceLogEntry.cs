using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Represents a single serializable entry of the trace log.
    /// The message can be structured XML.
    /// </summary>
    [DataContract]
    public class TraceLogEntry
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
        /// Identifier of the thread which the trace log entry originates from.
        /// </summary>
        [DataMember]
        public int ThreadID { get; private set; }

        /// <summary>
        /// XML contents of the log entry.
        /// </summary>
        [DataMember]
        public string XmlContent { get; private set; }

        internal TraceLogEntry(XElement entryElement)
        {
            XmlContent = string.Empty;
            foreach (XNode node in entryElement.Nodes()) {
                XmlContent += node.ToString();
            }

            string createdText = entryElement.Attribute(XName.Get("date-time")).Value;
            Created = DateTime.Parse(createdText);

            string logLevelText = entryElement.Attribute(XName.Get("type")).Value;
            LogLevel = EventLogEntry.ParseLogLevel(logLevelText);

            string threadIDText = entryElement.Attribute(XName.Get("thread-id")).Value;
            ThreadID = int.Parse(threadIDText);
        }
    }
}
