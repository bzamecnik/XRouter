using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT.Configuration;
using DaemonNT.Installation;
using DaemonNT.Logging;

namespace DaemonNT
{
    internal class ServiceCommands
    {
        /// <summary>
        /// Configuration file with service definitions and settings.
        /// If null or empty string is specified, a default
        /// configuration file is used.
        /// </summary>
        public string ConfigFile { get; set; }

        /// <summary>
        /// Starts the service in debug mode - inside a console application
        /// rather than a real NT service.
        /// </summary>
        /// <remarks>
        /// The service is stopped after the user presses any key in the
        /// console.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        public void Debug(string serviceName)
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
                    ServiceSettings serviceSettings = ConfigProvider.LoadServiceSettings(serviceName, ConfigFile);
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
        public void Run(string serviceName)
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
                    ServiceSettings serviceSettings = ConfigProvider.LoadServiceSettings(serviceName, ConfigFile);
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
        public void Install(string serviceName)
        {
            // load installer settings
            ServiceSettings serviceSettings = null;
            try
            {
                serviceSettings = ConfigProvider.LoadServiceSettings(serviceName, ConfigFile);
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
        public void Uninstall(string serviceName)
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
    }
}
