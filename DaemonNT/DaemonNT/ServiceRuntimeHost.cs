namespace DaemonNT
{
    using System.ServiceProcess;

    internal sealed class ServiceRuntimeHost : ServiceBase
    {
        private Service service = null;

        private DaemonNT.Logging.Logger logger = null;

        private DaemonNT.Configuration.ServiceSetting serviceSetting = null;

        public ServiceRuntimeHost(
            string serviceName,
            DaemonNT.Configuration.ServiceSetting serviceSetting,
            DaemonNT.Logging.Logger logger,
            Service service)
        {
            this.service = service;
            this.serviceSetting = serviceSetting;
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
            // TODO: why the 'args' parameter is ignored?!
            this.service.StartCommand(this.ServiceName, false, this.logger, this.serviceSetting.Setting);
        }

        protected override void OnStop()
        {
            this.service.StopCommand(false);
        }

        protected override void OnShutdown()
        {
            this.service.StopCommand(true);
        }
    }
}
