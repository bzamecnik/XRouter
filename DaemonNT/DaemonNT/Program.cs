namespace DaemonNT
{
    using System;
    using DaemonNT.Configuration;
    using DaemonNT.Installation;
    using DaemonNT.Logging;

    /// <summary>
    /// Provides a console program to run various DaemonNT commands.
    /// </summary>
    /// <remarks>
    /// The available commands are: <c>run</c>, <c>debug</c>, <c>install</c>,
    /// <c>uninstall</c>.
    /// </remarks>
    internal class Program
    {
        /// <summary>
        /// Starts the service in debug mode - inside a console application
        /// rather than a real NT service.
        /// </summary>
        /// <remarks>
        /// The service is stopped after the user presses any key in the
        /// console.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
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
                    logger.Event.LogError(String.Format("An error occurred during DaemonNT initialization: {0}", e.Message));
                    throw e;
                }

                // service hosting
                try
                {
                    serviceHost.Start();
                    Console.WriteLine("Press ENTER or CRTL+C to stop the '{0}' service ...", serviceName);
                    Console.CancelKeyPress += (sender, e) => serviceHost.Stop();
                    Console.ReadLine();
                    serviceHost.Stop();
                }
                catch (Exception e)
                {
                    logger.Event.LogError(String.Format("An unexpected error occurred while running DaemonNT: {0}", e.Message));
                    throw e;
                }
            }
            catch (Exception e)
            {
                // log to console
                Console.WriteLine("Error: {0}", e.Message);
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
        /// Starts the installed service as an NT service.
        /// </summary>
        /// <remarks>
        /// It calls the operating system to start the service.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
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
                    logger.Event.LogError(String.Format("An error occurred during DaemonNT initialization: {0}", e.Message));
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
                    logger.Event.LogError(String.Format("An unexpected error occurred while running DaemonNT: {0}", e.Message));
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
        /// Installs the service as an NT service.
        /// </summary>
        /// <param name="serviceName">Service name</param>
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
                Console.WriteLine("Error: {0} The log file is located at {1}.Installer.log.", e.Message, serviceName);
            }
        }

        /// <summary>
        /// Uninstalls the service which is installed as an NT service.
        /// </summary>
        /// <param name="serviceName">Service name</param>
        public static void CommandUninstall(string serviceName)
        {
            try
            {
                ProjectInstaller.Initialize(serviceName, null);
                InstallerServices.Uninstall(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0} The log file is located at {1}.Installer.log.", e.Message, serviceName);
            }
        }

        public static void Main(string[] args)
        {
            // TODO: Lepe poresit parametry prikazove radky (az bude definitivne jiste,
            // co vsechno bude v sobe DaemonNT obsahovat - pokud budeme napr. implementovat
            // watchdog ci konfiguracni GUI)

            if (args.Length != 2)
            {
                PrintUsage();
                return;
            }

            string command = args[0];
            string serviceName = args[1];
            switch (command)
            {
                case "run":
                    CommandRun(serviceName);
                    break;
                case "debug":
                    CommandDebug(serviceName);
                    break;
                case "install":
                    CommandInstall(serviceName);
                    break;
                case "uninstall":
                    CommandUninstall(serviceName);
                    break;
                default:
                    PrintUsage();
                    return;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: DaemonNT.exe COMMAND SERVICE_NAME");
            Console.WriteLine("Available commands are: run, debug, install, uninstall.");
        }
    }
}
