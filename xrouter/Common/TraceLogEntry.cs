using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using DaemonNT.Logging;

namespace XRouter.Common
{
    [DataContract]
    public class TraceLogEntry
    {
        [DataMember]
        public DateTime Created { get; private set; }

        [DataMember]
        public LogType LogLevel { get; private set; }

        [DataMember]
        public int ThreadID { get; private set; }

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
