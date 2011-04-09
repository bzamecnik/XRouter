namespace DaemonNT
{
    /// <summary>
    /// Provides an abstract class for a DaemonNT service that will exist as part as 
    /// a service application.
    /// </summary>
    public abstract class Service
    {
        internal void StartCommand(
            string serviceName,
            bool isDebugMode,
            DaemonNT.Logging.Logger logger,
            DaemonNT.Configuration.Setting setting)
        {
            ServiceArgs args = new ServiceArgs(serviceName, isDebugMode, logger, setting);
            this.OnStart(args);
        }

        internal void StopCommand(bool shutdown)
        {
            this.OnStop(shutdown);
        }

        protected virtual void OnStart(ServiceArgs args)
        {
        }

        protected virtual void OnStop(bool shutdown)
        {
        }
    }
}
