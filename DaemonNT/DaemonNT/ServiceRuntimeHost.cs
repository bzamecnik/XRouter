namespace DaemonNT
{
    using System.ServiceProcess;
    using DaemonNT.Configuration;
    using DaemonNT.Logging;

    /// <summary>
    /// Provides a full service host which runs the service within the NT
    /// service framework. It is intended for production use.
    /// </summary>
    internal sealed class ServiceRuntimeHost : ServiceBase
    {
        private Service service;

        private Logger logger;

        private ServiceSettings serviceSettings;

        public ServiceRuntimeHost(Service service, string serviceName, ServiceSettings settings, Logger logger)
        {
            this.service = service;
            this.serviceSettings = settings;
            this.logger = logger;
            this.ServiceName = serviceName;

            // service base properties
            this.AutoLog = false;
            this.CanHandlePowerEvent = false;
            this.CanHandleSessionChangeEvent = false;
            this.CanPauseAndContinue = false;
            this.CanStop = true;
            this.CanShutdown = true;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: why the 'args' parameter is ignored?
            this.service.Start(this.ServiceName, false, this.logger, this.serviceSettings.Settings);
        }

        protected override void OnStop()
        {
            this.service.Stop(false);
        }

        protected override void OnShutdown()
        {
            this.service.Stop(true);
        }
    }
}
