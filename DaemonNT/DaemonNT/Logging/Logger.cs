namespace DaemonNT.Logging
{
    /// <summary>
    /// Provides a logging facility.
    /// </summary>
    /// <remarks>
    /// Two loggers are available: event logger and trace logger.
    /// A new logger of logger can be created and started via the Start()
    /// factory method. A running logger can be stopped by calling
    /// the Stop() method.
    /// </remarks>
    public sealed class Logger
    {
        public EventLogger Event { get; private set; }

        public TraceLogger Trace { get; private set; }

        public void Stop()
        {
            this.Event.Stop();
            this.Trace.Stop();
        }

        // TODO: It is good to join the two operations, creating a logger and
        // starting it, into a single method?

        internal static Logger Start(string serviceName)
        {
            Logger logger = new Logger();
            logger.Event = EventLogger.Start(serviceName);
            logger.Trace = TraceLogger.Start(serviceName);
            return logger;
        }
    }
}
