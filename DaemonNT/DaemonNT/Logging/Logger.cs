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
        /// <summary>
        /// Obsahuje nazev sluzby, ktera loguje.
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Obsahuje informaci o tom, jestli je logger pouzivan v DaemonNT
        /// ladicim modu. 
        /// </summary>
        private bool isDebugMode;

        /// <summary>
        /// Urcuje velikost bufferu event loggeru. 
        /// </summary>
        private static readonly int EVENT_BUFFER_SIZE = 1000;

        /// <summary>
        /// Poskytuje efektivní, tread-safe logování významných událostí, které jsou čitelné 
        /// pro správce služby.
        /// </summary>
        public EventLogger Event { get; private set; }

        /// <summary>
        /// Poskytuje efektivní, thread-safe logování potenciálně low-level informací, které jsou 
        /// čitelné vývojáři, odborníky dané problémové domény či adekvátním softwarem. 
        /// </summary>
        public TraceLogger Trace { get; private set; }

        internal static Logger Create(string serviceName, bool isDebugMode)
        {
            Logger logger = new Logger();
            logger.serviceName = serviceName;
            logger.isDebugMode = isDebugMode;
            logger.Event = EventLogger.Create(serviceName, EVENT_BUFFER_SIZE);            

            return logger;
        }
        
        internal void CreateTraceLogger(TraceLoggerSettings settings)
        {
            this.Trace = TraceLogger.Create(serviceName, settings.BufferSize, 
                this.isDebugMode, settings, this.Event);
        }

        internal void Close(bool shutdown)
        {           
            if (this.Trace != null)
            {
                this.Trace.Close(shutdown);
            }

            this.Event.Close();
        }       
    }
}
