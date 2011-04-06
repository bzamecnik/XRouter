using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    public sealed class Logger 
    {       
        public EventLogger Event { private set; get; }

        public TraceLogger Trace { private set; get; }
        
        internal static Logger Start(String serviceName)
        {
            Logger instance = new Logger();
            instance.Event = EventLogger.Start(serviceName);  
            instance.Trace = TraceLogger.Start(serviceName);

            return instance;
        }

        public void Stop()
        {
            this.Event.Stop();
            this.Trace.Stop();
        }
    }
}
