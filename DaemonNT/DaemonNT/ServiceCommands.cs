using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Xml.Linq;
using System.Xml.XPath;
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
                    Console.WriteLine("The service '{0}' is running in debug mode.", serviceName);
                    Console.WriteLine("Press CTRL+C or type 'exit' to stop the service ...", serviceName);
                    Console.CancelKeyPress += (sender, e) => serviceHost.Stop();
                    while (true)
                    {
                        string request = Console.ReadLine();
                        if (request == "exit")
                        {
                            serviceHost.Stop();
                            return;
                        }
                    }
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
        /// This method is intended to be called by the operating system's
        /// service runner, not by user!
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
        /// <returns>True if the service was successfully installed.</returns>
        public bool Install(string serviceName)
        {
            if (InstallerServices.IsInstalled(serviceName))
            {
                Console.WriteLine("Error: Service '{0}' is already installed.", serviceName);
                return false;
            }

            // load installer settings
            ServiceSettings serviceSettings = null;
            try
            {
                serviceSettings = ConfigProvider.LoadServiceSettings(serviceName, ConfigFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e.Message);
                return false;
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
                return false;
            }
            Console.WriteLine("Service '{0}' has been successfully installed.", serviceName);
            return true;
        }

        /// <summary>
        /// Uninstalls the service which is installed as an NT service.
        /// </summary>
        /// <param name="serviceName">Service name</param>
        /// <returns>True if the service was successfully uninstalled.</returns>
        public bool Uninstall(string serviceName)
        {
            if (!InstallerServices.IsInstalled(serviceName))
            {
                Console.WriteLine("Error: Service '{0}' is not installed.", serviceName);
                return false;
            }

            try
            {
                ProjectInstaller.Initialize(serviceName, null);
                InstallerServices.Uninstall(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0} The log file is located at {1}.Installer.log.", e.Message, serviceName);
                return false;
            }
            Console.WriteLine("Service '{0}' has been successfully uninstalled.", serviceName);
            return true;
        }

        /// <summary>
        /// Starts an installed service.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called by a user process (unlike the
        /// Run() method.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        /// <returns>True if the service was successfully started.</returns>
        public bool Start(string serviceName)
        {
            if (!InstallerServices.IsInstalled(serviceName))
            {
                Console.WriteLine("Error: Service '{0}' is not installed.", serviceName);
                return false;
            }

            bool? started = (bool?)WorkWithServiceController(serviceName,
                (sc) =>
                {
                    sc.Start();
                    Console.WriteLine("Service '{0}' has been started.", serviceName);
                    return true;
                },
                "Cannot start service {0}: {1}");
            return started.HasValue && started.Value;
        }

        /// <summary>
        /// Restarts an installed service.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called by a user process (unlike the
        /// Run() method.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        /// <returns>True if the service was successfully restarted.</returns>
        public bool Restart(string serviceName)
        {
            if (!InstallerServices.IsInstalled(serviceName))
            {
                Console.WriteLine("Error: Service '{0}' is not installed.", serviceName);
                return false;
            }

            bool? restarted = (bool?)WorkWithServiceController(serviceName,
                (sc) =>
                {
                    if (GetStatus(serviceName) == ServiceControllerStatus.Running.ToString())
                    {
                        sc.Stop();
                    }
                    // TODO: there could be a timeout
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    sc.Start();
                    // TODO: wait for the Running status
                    // - the whole method should accept a timeout parameter
                    Console.WriteLine("Service '{0}' has been restarted.", serviceName);
                    return true;
                },
                "Cannot restart service '{0}': {1}");
            return restarted.HasValue && restarted.Value;
        }

        /// <summary>
        /// Stops an installed service.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called by a user process (unlike the
        /// Run() method.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        /// <returns>True if the service was successfully stopped.</returns>
        public bool Stop(string serviceName)
        {
            if (!InstallerServices.IsInstalled(serviceName))
            {
                Console.WriteLine("Error: Service '{0}' is not installed.", serviceName);
                return false;
            }

            bool? stopped = (bool?)WorkWithServiceController(serviceName,
                (sc) =>
                {
                    sc.Stop();
                    Console.WriteLine("Service '{0}' has been stopped.", serviceName);
                    return true;
                },
                "Cannot stop service {0}: {1}");
            return stopped.HasValue && stopped.Value;
        }

        /// <summary>
        /// Queries for the service status (running, stopped, etc.).
        /// </summary>
        /// <param name="serviceName">Service name</param>
        public string GetStatus(string serviceName)
        {
            if (!InstallerServices.IsInstalled(serviceName))
            {
                return string.Empty;
            }
            string status = (string)WorkWithServiceController(serviceName,
                (sc) =>
                {
                    return sc.Status.ToString();
                },
                "Cannot determine status of service '{0}': {1}");
            if (status == null)
            {
                return string.Empty;
            }
            return status;
        }

        /// <summary>
        /// Gets the list of names of all configured services.
        /// </summary>
        /// <returns>List of service names.</returns>
        public IEnumerable<string> ListServices()
        {
            XDocument config = ConfigProvider.LoadRawConfiguration(ConfigFile);
            var services = config.XPathSelectElements("/config/service");
            List<string> serviceNames = new List<string>();
            foreach (var service in services)
            {
                serviceNames.Add(service.Attribute(XName.Get("name")).Value);
            }
            return serviceNames;
        }

        /// <summary>
        /// Perform an action with a new service controller, taking care
        /// of the exceptions.
        /// </summary>
        /// <param name="serviceName">Service name</param>
        /// <param name="action">Action to be performed</param>
        /// <param name="errorMessage">Error message template (formattable by
        /// strinf.Format(). {0} is replaced by serviceName, {1} by the error
        /// message.</param>
        /// <returns>The result of the action, can be null for void action.
        /// </returns>
        private object WorkWithServiceController(
            string serviceName,
            Func<ServiceController, object> action,
            string errorMessage)
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                return WorkWithServiceController(sc, serviceName, action, errorMessage);
            }
        }

        /// <summary>
        /// Perform an action with an existing service controller, taking care
        /// of the exceptions.
        /// </summary>
        /// <remarks>
        /// Disposing of the provided service controller is the responsibility
        /// of the called of this method.
        /// </remarks>
        /// <param name="sc">Existing ServiceController instance</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="action">Action to be performed</param>
        /// <param name="errorMessage">Error message template (formattable by
        /// strinf.Format(). {0} is replaced by serviceName, {1} by the error
        /// message.</param>
        /// <returns>The result of the action, can be null for void action.
        /// </returns>
        private object WorkWithServiceController(
            ServiceController sc,
            string serviceName,
            Func<ServiceController, object> action,
            string errorMessage)
        {
            try
            {
                return action(sc);
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine(errorMessage, serviceName, ex.Message);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.Error.WriteLine(errorMessage, serviceName, ex.Message);
            }
            return null;
        }
    }
}
