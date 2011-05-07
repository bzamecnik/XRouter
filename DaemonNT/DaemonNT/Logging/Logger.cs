namespace DaemonNT.Logging
{
    using DaemonNT.Configuration;

    /// <summary>
    /// Provides a logging facility for services.
    /// </summary>
    /// <remarks>
    /// Two loggers are available: event logger and trace logger.
    /// </remarks>
    public sealed class Logger
    {
        /// <summary>
        /// Name of service which logs into this logger.
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Indicates whether the logger is used in debug mode.
        /// </summary>
        private bool isDebugMode;

        // TODO: this should be configurable, not hard-coded

        /// <summary>
        /// Buffer size (number of records) of the event log.
        /// </summary>
        private static readonly int EVENT_BUFFER_SIZE = 1000;

        /// <summary>
        /// Provides a facility for efficient, thread-safe logging of
        /// important events designated for service administrators.
        /// </summary>
        public EventLogger Event { get; private set; }

        /// <summary>
        /// Provides a facility for efficient, thread-safe logging of
        /// potentially low-level information designated for developers or
        /// specialists in the problem domain.
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

        // TODO: Is it necessary to initialize trace logger separately from
        // the Create() method?

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
