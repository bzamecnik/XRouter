namespace DaemonNT
{
    using DaemonNT.Logging;
    using DaemonNT.Configuration;

    public sealed class ServiceArgs
    {
        internal ServiceArgs(
            string serviceName,
            bool debugModeEnabled,
            Logger logger,
            Settings settings)
        {
            this.ServiceName = serviceName;
            this.DebugModeEnabled = debugModeEnabled;
            this.Logger = logger;
            this.Settings = settings;
        }

        public string ServiceName { get; private set; }

        public bool DebugModeEnabled { get; private set; }

        public Logger Logger { get; private set; }

        public Settings Settings { get; private set; }
    }
}
