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
        /// Vraci DaemoNT logger, ktery umoznuje zaznamenavat udalosti a trasovani
        /// sluzby. 
        /// </summary>
        /// <remarks>
        /// Instanci je mozno zacit pouzivat v metode OnStart().
        /// </remarks>
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
            // prepare start args
            OnStartServiceArgs args = new OnStartServiceArgs();
            args.ServiceName = serviceName;
            args.IsDebugMode = isDebugMode;                      
            args.Settings = settings;

            // start service
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
            // create stop args
            OnStopServiceArgs args = new OnStopServiceArgs();
            args.Shutdown = shutdown;

            // stop service
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
        /// V odvozene tride neni nutne volat base.OnStart().
        /// </remarks>
        protected virtual void OnStart(OnStartServiceArgs args)
        {
        }

        /// <summary>
        /// A hook method which is called before the service is stopped
        /// (manually or due to a system shutdown). It is intended to be
        /// implemented in a derived class.
        /// </summary>      
        /// <remarks>
        /// V odvozene tride neni nutne volat base.OnSop().
        /// </remarks>
        protected virtual void OnStop(OnStopServiceArgs args)
        {
        }
    }
}
