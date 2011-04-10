namespace DaemonNT
{
    /// <summary>
    /// Provides an abstract class for a DaemonNT service that will exist as
    /// part of a service application.
    /// </summary>
    public abstract class Service
    {
        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <remarks>
        /// OnStart() hook methods are called upon starting the service.
        /// </remarks>
        /// <param name="serviceName"></param>
        /// <param name="debugModeEnabled"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        internal void Start(
            string serviceName,
            bool debugModeEnabled,
            DaemonNT.Logging.Logger logger,
            DaemonNT.Configuration.Settings settings)
        {
            ServiceArgs args = new ServiceArgs(serviceName, debugModeEnabled, logger, settings);
            this.OnStart(args);
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <remarks>
        /// OnStop() hook methods are called upon stopping the service.
        /// </remarks>
        /// <param name="shutdown">Indicates that the system is shutting down</param>
        internal void Stop(bool shutdown)
        {
            this.OnStop(shutdown);
        }

        /// <summary>
        /// A hook method which is called just when the Service has been
        /// started. It is intended to be implemented in a derived class.
        /// </summary>
        /// <param name="args">Service start-up arguments</param>
        protected virtual void OnStart(ServiceArgs args)
        {
        }

        /// <summary>
        /// A hook method which is called before the service is stopped
        /// (manually or due to a system shutdown). It is intended to be
        /// implemented in a derived class.
        /// </summary>
        /// <param name="shutdown">Indicates that the system is shutting down
        /// </param>
        protected virtual void OnStop(bool shutdown)
        {
        }
    }
}
