using System;

namespace DaemonNT
{
    /// <summary>
    /// Provides an abstract class for a DaemonNT service that will exist as
    /// part of a service application.
    /// </summary>
    public abstract class Service
    {
        /// <summary>
        /// A logger providing a facility to record events and to trace
        /// services.
        /// </summary>
        /// <remarks>
        /// A logger instance can be used after the OnStart() method was
        /// called.
        /// </remarks>
        public DaemonNT.Logging.Logger Logger { internal set; get; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <remarks>
        /// OnStart() hook methods (from this and/or derived classes) are
        /// called upon starting the service.
        /// </remarks>
        /// <param name="serviceName">Service name. Must not be null.</param>
        /// <param name="isDebugMode">Indicates whether the service should run
        /// in debug mode (true) or as a true NT service (false).</param>
        /// <param name="logger">Instance of logger. Must not be null.</param>
        /// <param name="settings">Service settings. Must not be null.</param>
        internal void Start(
            string serviceName,
            bool isDebugMode,
            DaemonNT.Logging.Logger logger,
            DaemonNT.Configuration.Settings settings)
        {
            // prepare start args
            OnStartServiceArgs args = new OnStartServiceArgs()
            {
                ServiceName = serviceName,
                IsDebugMode = isDebugMode,
                Settings = settings
            };

            // start service
            this.Logger.Event.LogInfo(string.Format(
                "The '{0}' service is being started...", args.ServiceName));
            try
            {
                this.OnStart(args);
                this.Logger.Event.LogInfo(string.Format(
                    "The '{0}' service has beed started successfully!",
                    args.ServiceName));
            }
            catch (Exception e)
            {
                this.Logger.Event.LogError(string.Format(
                    "An unexpected error occurred while starting the service: {0}",
                    e.Message));
                throw e;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <remarks>
        /// OnStop() hook methods (from this and/or derived classes) are
        /// called upon stopping the service.
        /// </remarks>
        /// <param name="shutdown">Indicates that the system is being shut down.
        /// </param>
        internal void Stop(bool shutdown)
        {
            // create stop args
            OnStopServiceArgs args = new OnStopServiceArgs()
            {
                Shutdown = shutdown
            };

            // stop service
            this.Logger.Event.LogInfo("The service is being stopped...");
            try
            {
                this.OnStop(args);
                this.Logger.Event.LogInfo("The service has been stopped successfully!");
            }
            catch (Exception e)
            {
                this.Logger.Event.LogError(string.Format(
                    "An unexpected error occurred while stopping the service: {0}",
                    e.Message));
                throw e;
            }
        }

        /// <summary>
        /// A hook method which is called just when the Service has been
        /// started. It is intended to be implemented in a derived class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Typically, in this method a new thread with a service loop should
        /// be started. This method should not run indefinitely long, it is
        /// meant just for initialization.
        /// </para>
        /// <para>
        /// In derived classes it is not necessary to call base.OnStart().
        /// </para>
        /// </remarks>
        /// <param name="args">Arguments for starting the service.</param>
        protected virtual void OnStart(OnStartServiceArgs args)
        {
        }

        /// <summary>
        /// A hook method which is called before the service is stopped
        /// (manually or due to a system shutdown). It is intended to be
        /// implemented in a derived class.
        /// </summary>
        /// <remarks>
        /// In derived classes it is not necessary to call base.OnSop().
        /// </remarks>
        /// <param name="args">Arguments for stopping the service.</param>
        protected virtual void OnStop(OnStopServiceArgs args)
        {
        }
    }
}
