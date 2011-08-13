using System;
using System.ServiceProcess;
using System.Threading;
using DaemonNT.Logging;

namespace XRouter.Manager
{
    /// <summary>
    /// Reprezentuje jednoduchy service watcher, ktery pravidelne sleduje status sluzby. 
    /// Posledne ziskany status lze ziskat z vlastnosti ServiceStatus. Pokud je hodnota 
    /// throwUpService nastavena na true a sluzba je po urcity cas ce stavu stopped, pak 
    /// ji nahodi. Nahazovani sluzby je mozno docasne zakazat metodou DisableServiceThrowUp()
    /// a v takovem pripade se nahazovani opet povoli, jakmile sluzba prejde do stavu running. 
    /// </summary>
    internal sealed class Watcher
    {
        /// <summary>
        /// Nazev instance asociovane XRouterService.
        /// </summary>
        private string managedServiceName = null;

        /// <summary>
        /// Urcuje, jestli je ServiceWatcher pouzit pouze v ladicim prostredi (ma vliv na jeho chovani).
        /// </summary>
        private bool isDebugMode = false;

        /// <summary>
        /// Odkaz na Daemon trace logger.
        /// </summary>
        private TraceLogger logger = null;

        /// <summary>
        /// Urcuje, jestli se ma sluzba nahazovat (urceno konfiguraci), pokud je po urcity cas ve stavu stopped.
        /// </summary>
        private bool configAutoRestartEnabled = false;

        /// <summary>
        /// Urcuje, jestli se ma sluzna nahazovat (meni se za behu).       
        /// </summary>
        private volatile bool runtimeAutoRestartEnabled = false;

        /// <summary>
        /// Thread, ktery hostuje implementaci sledovani. 
        /// </summary>
        private Thread worker = null;

        /// <summary>
        /// Urcuje, jestli stale sledovani bezi. 
        /// </summary>
        private volatile bool isWorkerRunning = true;
        
        /// <summary>
        /// Pocet jednotek casu, po kterych je sluzba ve stavu stopped. 
        /// </summary>
        private int stoppedTimes = 0;

        /// <summary>
        /// Obsahuje aktualni (posledne zjisteny) status sluzby. 
        /// </summary>
        private volatile ServiceControllerStatus lastServiceStatus = ServiceControllerStatus.Stopped;

        /// <summary>
        /// Perioda sledovani (polling) sluzby v ms. 
        /// Hodnota je urcena implementaci a nebude parametrizovana.
        /// </summary>
        private static readonly int Interval = 1000;

        /// <summary>
        /// Pocet jenotek casu, po kterych muze byt sluzba nahozena. 
        /// Hodnota je urcena implementaci a nebude parametrizovana. 
        /// </summary>
        private static readonly int MaxStoppedTimes = 10;

        /// <summary>
        /// Odkaz na odesilac emailu. 
        /// </summary>
        private EMailSender emailSender = null;

        /// <summary>
        /// Vraci aktualni (posledne zjisteny) status sluzby.
        /// </summary>
        public ServiceControllerStatus ServiceStatus
        {
            get { return this.lastServiceStatus; }
        }

        /// <summary>
        /// Umoznuje zakazat nahazovani sluzby. 
        /// </summary>
        public void DisableServiceAutoRestart()
        {
            this.runtimeAutoRestartEnabled = false;
        }

        public Watcher(string serviceName, bool isDebugMode, bool autoRestartEnabled, TraceLogger logger, EMailSender sender)
        {
            this.managedServiceName = serviceName;
            this.isDebugMode = isDebugMode;
            this.configAutoRestartEnabled = autoRestartEnabled;
            this.runtimeAutoRestartEnabled = autoRestartEnabled;
            this.logger = logger;
            this.emailSender = sender;

            if (isDebugMode)
            {
                // TODO: this is quite meaningless
                // watcher should be disabled if the watched XRouter service
                // runs in debug mode, not this XRouter Manager service!!!
                logger.LogWarning("Watcher is disabled on debug mode.");
            }
        }

        public void Start()
        {
            this.worker = new Thread(RunWatcher);
            this.worker.Start(null);
        }

        /// <summary>
        /// Stops the watcher thread.
        /// </summary>
        public void Stop()
        {
            this.isWorkerRunning = false;
            this.worker.Join();
        }

        private void RunWatcher(object data)
        {
            while (this.isWorkerRunning)
            {
                Thread.Sleep(Interval);

                if (this.isDebugMode)
                {
                    continue;
                }
                try
                {
                    // get status
                    ServiceController sc = new ServiceController(this.managedServiceName);
                    this.lastServiceStatus = sc.Status;

                    // manage stopped times
                    if (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        this.stoppedTimes++;
                    }
                    else
                    {
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            this.runtimeAutoRestartEnabled = true;
                        }
                        this.stoppedTimes = 0;
                    }

                    // automatically restart the managed service
                    if (this.configAutoRestartEnabled && this.runtimeAutoRestartEnabled)
                    {
                        RestartAutomatically(sc);
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogException(e);
                }
            }
        }

        private void RestartAutomatically(ServiceController sc)
        {
            if (this.stoppedTimes == MaxStoppedTimes)
            {
                this.stoppedTimes = 0;

                // log info
                string logMessage = (string.Format(
                    "Watcher automatically restarted managed service '{0}'.",
                    this.managedServiceName));
                this.logger.LogInfo(logMessage);

                // send e-mail
                if (this.emailSender != null)
                {
                    this.emailSender.Send("Automatic restart", logMessage);
                }

                // start service
                sc.Start();
            }
        }
    }
}
