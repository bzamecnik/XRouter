namespace DaemonNT.Logging
{
    using DaemonNT.Configuration;

    /// <summary>
    /// Provides a logging facility.
    /// </summary>
    /// <remarks>
    /// Two loggers are available: event logger and trace logger.   
    /// </remarks>
    public sealed class Logger
    {
        private string serviceName;

        /// <summary>
        /// Poskytuje efektivní, tread-safe logování významných událostí, které jsou čitelné 
        /// pro správce služby.
        /// </summary>
        public EventLogger Event { get; private set; }

        /// <summary>
        /// Poskytuje efektivní, thread-safe logování low-level informací, které jsou čitelné 
        /// vývojáři či odborníky dané problémové domény. 
        /// </summary>
        public TraceLogger Trace { get; private set; }

        internal static Logger Create(string serviceName)
        {
            Logger logger = new Logger();
            logger.serviceName = serviceName;

            return logger;
        }

        internal void CreateEventLogger()
        {
            this.Event = EventLogger.Create(serviceName, 1000);          
        }

        internal void CloseEventLogger()
        {
            if (this.Event != null)
            {
                this.Event.Close();
            }
        }

        internal void CreateTraceLogger()
        {
            this.Trace = TraceLogger.Create(serviceName, 1000);
        }

        internal void CloseTraceLogger()
        {
            if (this.Trace != null)
            {
                this.Trace.Close();
            }
        }       
    }
}
