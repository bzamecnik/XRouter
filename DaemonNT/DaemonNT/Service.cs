using System;

namespace DaemonNT
{
    /// <summary>
    /// Provides an abstract class for a DaemonNT service that will exist as
    /// part of a service application.
    /// </summary>
    public abstract class Service
    {
        public DaemonNT.Logging.Logger Logger { internal set; get; }           
  
        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <remarks>
        /// OnStart() hook methods are called upon starting the service.
        /// </remarks>    
        internal void Start(string serviceName, bool isDebugMode, 
            DaemonNT.Logging.Logger logger, DaemonNT.Configuration.Settings settings)
        {
            OnStartArgs args = new OnStartArgs();
            args.ServiceName = serviceName;
            args.IsDebugMode = isDebugMode;                      
            args.Settings = settings;

            this.Logger.Event.LogInfo("Sluzba se spousti...");
            try
            {
                this.OnStart(args);
                this.Logger.Event.LogInfo("Sluzba byla uspesne spustena!");    
            }
            catch (Exception e)
            {
                this.Logger.Event.LogError("Pri startu sluzby doslo k neocekavane chybe.");
                throw e;
            }                   
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
            OnStopArgs args = new OnStopArgs();
            args.Shutdown = shutdown;

            this.Logger.Event.LogInfo("Sluzba se zastavuje...");
            try
            {
                this.OnStop(args);
                this.Logger.Event.LogInfo("Sluzba byla uspesne zastavena!");
            }
            catch (Exception e)
            {
                this.Logger.Event.LogError("Pri zastaveni sluzby doslo k neocekavane chybe.");
                throw e;
            }
        }

        /// <summary>
        /// A hook method which is called just when the Service has been
        /// started. It is intended to be implemented in a derived class.
        /// </summary>
        /// <param name="args">Service start-up arguments</param>
        protected virtual void OnStart(OnStartArgs args)
        {
        }

        /// <summary>
        /// A hook method which is called before the service is stopped
        /// (manually or due to a system shutdown). It is intended to be
        /// implemented in a derived class.
        /// </summary>
        /// <param name="shutdown">Indicates that the system is shutting down
        /// </param>
        protected virtual void OnStop(OnStopArgs args)
        {
        }
    }
}
