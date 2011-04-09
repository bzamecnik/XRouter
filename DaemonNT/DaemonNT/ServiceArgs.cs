namespace DaemonNT
{
    public sealed class ServiceArgs
    {
        internal ServiceArgs(
            string serviceName,
            bool isDebugMode,
            DaemonNT.Logging.Logger logger,
            DaemonNT.Configuration.Setting setting)
        {
            this.ServiceName = serviceName;
            this.IsDebugMode = isDebugMode;
            this.Logger = logger;
            this.Setting = setting;
        }

        public string ServiceName { get; private set; }

        public bool IsDebugMode { get; private set; }

        public DaemonNT.Logging.Logger Logger { get; private set; }

        public DaemonNT.Configuration.Setting Setting { get; private set; }
    }
}
