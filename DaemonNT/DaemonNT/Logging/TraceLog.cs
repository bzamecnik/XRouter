using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DaemonNT.Logging
{
    internal class TraceLog
    {
        public DateTime DateTime { private set; get; }

        public Int32 ThreadId { private set; get; }

        public String ThreadName { private set; get; }

        public String Content { private set; get; }

        private TraceLog()
        { }

        public static TraceLog Create(String xmlContent)
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
