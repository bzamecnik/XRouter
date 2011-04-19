namespace DaemonNT
{
    using System;
    using DaemonNT.Configuration;
    using DaemonNT.Installation;
    using DaemonNT.Logging;

    /// <summary>
    /// Obsahuje implementaci use-cases DaemonNT. 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Spusti sluzbu v ladicim modu.        
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandDebug(string serviceName)
        {
            Logger logger = null;            
            try
            {
                // create event logger
                logger = Logger.Create(serviceName, true);
                
                // load settings and initialize constructs                
                ServiceDebugHost serviceHost = null;
                try
                {
                    ServiceSettings serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
                    logger.CreateTraceLogger(serviceSettings.TraceLoggerSettings);
                    Service service = TypesProvider.CreateService(serviceSettings.TypeClass, serviceSettings.TypeAssembly);
                    serviceHost = new ServiceDebugHost(service, serviceName, serviceSettings, logger);                                                            
                }
                catch (Exception e)
                {                   
                    logger.Event.LogError(String.Format("Pri inicializaci DaemonNT doslo k chybe. {0}", e.Message));
                    throw e;
                }

                // service hosting
                try
                {
                    serviceHost.Start();
                    Console.WriteLine(string.Format("Press enter to stop service {0}...", serviceName));
                    Console.ReadLine();
                    serviceHost.Stop();
                }
                catch (Exception e)
                {                   
                    logger.Event.LogError("Pri behu DaemonNT doslo k neocekavane chybe.");
                    throw e;
                }
            }
            catch (Exception e)
            {
                // log to console
                Console.WriteLine(String.Format("Error: {0}", e.Message));               
            }
            finally
            {
                if (logger != null)
                {
                    logger.Close(false);
                }
            }
        }

        /// <summary>
        /// Spusti nainstalovanou sluzbu (spousti operacni system).        
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandRun(string serviceName)
        {
            Logger logger = null;
            bool shutdown = false;
            try
            {
                // create logger
                logger = Logger.Create(serviceName, false);
           
                // load settings and initialize constructs 
                ServiceRuntimeHost serviceHost = null;
                try
                {
                    ServiceSettings serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
                    logger.CreateTraceLogger(serviceSettings.TraceLoggerSettings);
                    Service service = TypesProvider.CreateService(serviceSettings.TypeClass, serviceSettings.TypeAssembly);
                    serviceHost = new ServiceRuntimeHost(service, serviceName, serviceSettings, logger);                                   
                }
                catch (Exception e)
                {                    
                    logger.Event.LogError(String.Format("Pri inicializaci DaemonNT doslo k chybe. {0}", e.Message));
                    throw e;
                }

                // service hosting
                try
                {
                    ServiceRuntimeHost.Run(serviceHost);
                    shutdown = serviceHost.Shutdown;
                }
                catch (Exception e)
                {
                    logger.Event.LogError("Pri behu DaemonNT doslo k neocekavane chybe.");                   
                    throw e;
                }
            }
            catch (Exception e)
            {
                // log to Windows EventLog
                throw e;
            }
            finally
            {
                if (logger != null)
                {
                    logger.Close(shutdown);
                }
            }
        }

        /// <summary>
        /// Provede instalaci sluzby.        
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandInstall(string serviceName)
        {
            // load installer settings
            ServiceSettings serviceSettings = null;
            try
            {
                serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e.Message);
            }

            // install
            try
            {
                ProjectInstaller.Initialize(serviceName, serviceSettings.InstallerSettings);
                InstallerServices.Install(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", string.Concat(e.Message, string.Format(" The log file is located at {0}.Installer.log.", serviceName)));
            }
        }

        /// <summary>
        /// Provede odinstalovani sluzby.        
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandUninstall(string serviceName)
        {
            try
            {
                ProjectInstaller.Initialize(serviceName, null);
                InstallerServices.Uninstall(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", string.Concat(e.Message, string.Format(" The log file is located at {0}.Installer.log.", serviceName)));
            }
        }
        
        public static void Main(string[] args)
        {
            // TODO: Lepe poresit parametry prikazove radky (az bude definitivne jiste, 
            // co vsechno bude v sobe DaemonNT obsahovat - pokud budeme napr. implementovat
            // watchdog ci konfiguracni GUI)

            if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "run":
                        CommandRun(args[1]);
                        break;
                    case "debug":
                        CommandDebug(args[1]);
                        break;
                    case "install":
                        CommandInstall(args[1]);
                        break;
                    case "uninstall":
                        CommandUninstall(args[1]);
                        break;                  
                }             
            }            
        }
    }
}
