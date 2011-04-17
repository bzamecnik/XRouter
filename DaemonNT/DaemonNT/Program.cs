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
        /// TODO: Pozdeji lepe zdokumentovat dle GoogleDocu. 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandDebug(string serviceName)
        {
            Logger logger = null;
            try
            {
                // create event logger
                logger = Logger.Create(serviceName);
                logger.CreateEventLogger();

                // load settings and initialize constructs                
                ServiceDebugHost serviceHost = null;
                try
                {
                    ServiceSettings serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
                    Service service = TypesProvider.CreateService(serviceSettings.TypeClass, serviceSettings.TypeAssembly);
                    serviceHost = new ServiceDebugHost(service, serviceName, serviceSettings, logger);

                    // TODO: vytvorit instance trace-loggeru, ktere jsou definovany v konfiguraci
                    logger.CreateTraceLogger();
                }
                catch (Exception e)
                {                   
                    logger.Event.LogError(e.Message);
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
                    logger.Event.LogError(e.Message);
                    throw e;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error: {0}", e.Message));               
            }
            finally
            {
                if (logger != null)
                {
                    logger.CloseTraceLogger();
                    logger.CloseEventLogger();
                }
            }
        }

        /// <summary>
        ///  Spusti nainstalovanou sluzbu. Spousti to Service Control Manager (nikoliv primo uzivatel).
        ///  TODO: Pozdeji lepe zdokumentovat dle GoogleDocu.
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandRun(string serviceName)
        {
            Logger logger = null;
            try
            {
                // create logger
                logger = Logger.Create(serviceName);
                logger.CreateEventLogger();

                // load settings and initialize constructs 
                ServiceRuntimeHost serviceHost = null;
                try
                {
                    ServiceSettings serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
                    Service service = TypesProvider.CreateService(serviceSettings.TypeClass, serviceSettings.TypeAssembly);
                    serviceHost = new ServiceRuntimeHost(service, serviceName, serviceSettings, logger);

                    // TODO: vytvorit instance trace-loggeru, ktere jsou definovany v konfiguraci
                    logger.CreateTraceLogger();                   
                }
                catch (Exception e)
                {
                    logger.Event.LogError(e.Message);
                    throw e;
                }

                // service hosting
                try
                {
                    ServiceRuntimeHost.Run(serviceHost);
                }
                catch (Exception e)
                {
                    logger.Event.LogError(e.Message);
                    throw e;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (logger != null)
                {
                    logger.CloseTraceLogger();
                    logger.CloseEventLogger();
                }
            }
        }

        /// <summary>
        /// Provede instalaci sluzby. 
        /// TODO: Pozdeji lepe zdokumentovat dle GoogleDocu. 
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
        /// TODO: Pozdeji lepe zdokumentovat dle GoogleDocu. 
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
            // co vsechno bude v sobe DaemonNT obsahovat. 

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
                    // TODO: default
                }
            }
        }
    }
}
