namespace DaemonNT
{
    using DaemonNT.Configuration;
    using DaemonNT.Logging;

    /// <summary>
    /// Provides a simple host for services outside the NT service framework
    /// in order to facilitate debugging.
    /// </summary>
    /// <remarks>
    /// The service can be run in a regular console application without the
    /// need to install it and running as an NT service. This can help save
    /// some time in the debugging cycle.
    /// </remarks>
    internal sealed class ServiceDebugHost
    {
        private string serviceName;

        private ServiceSettings serviceSettings;

        private Logger logger;

        private Service service;

        public ServiceDebugHost(Service service, string serviceName, ServiceSettings settings, Logger logger)
        {
            this.serviceName = serviceName;
            this.serviceSettings = settings;
            this.logger = logger;
            this.service = service;
        }

        internal void Start()
        {
            this.service.Start(this.serviceName, true, this.logger, this.serviceSettings.Settings);
        }

        internal void Stop()
        {
            this.service.Stop(false);
        }
    }
}
