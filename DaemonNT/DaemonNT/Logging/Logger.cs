namespace DaemonNT.Logging
{
    public sealed class Logger
    {
        public EventLogger Event { get; private set; }

        public TraceLogger Trace { get; private set; }

        public void Stop()
        {
            this.Event.Stop();
            this.Trace.Stop();
        }

        internal static Logger Start(string serviceName)
        {
            Logger instance = new Logger();
            instance.Event = EventLogger.Start(serviceName);
            instance.Trace = TraceLogger.Start(serviceName);

            return instance;
        }
    }
}
