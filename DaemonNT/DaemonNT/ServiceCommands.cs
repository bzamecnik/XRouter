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
                    Console.WriteLine("Press CTRL+C to stop the '{0}' service ...", serviceName);
                    Console.CancelKeyPress += (sender, e) => serviceHost.Stop();
                    while (true)
                    {
                        System.Threading.Thread.Sleep(100);
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

        /// <summary>
        /// Starts an installed service.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called by a user process (unlike the
        /// Run() method.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        public void Start(string serviceName)
        {
            WorkWithServiceController(serviceName,
                (sc) =>
                {
                    sc.Start();
                    return null;
                },
                "Cannot start service {0}: {1}");
        }

        /// <summary>
        /// Restarts an installed service.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called by a user process (unlike the
        /// Run() method.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        public void Restart(string serviceName)
        {
            WorkWithServiceController(serviceName,
                (sc) =>
                {
                    sc.Stop();
                    sc.Start();
                    return null;
                },
                "Cannot restart service {0}: {1}");
        }

        /// <summary>
        /// Stops an installed service.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called by a user process (unlike the
        /// Run() method.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        public void Stop(string serviceName)
        {
            WorkWithServiceController(serviceName,
                (sc) =>
                {
                    sc.Stop();
                    return null;
                },
                "Cannot stop service {0}: {1}");
        }

        /// <summary>
        /// Queries for the service status (running, stopped, etc.).
        /// </summary>
        /// <param name="serviceName">Service name</param>
        public string GetStatus(string serviceName)
        {
            string status = (string)WorkWithServiceController(serviceName,
                (sc) =>
                {
                    return sc.Status.ToString();
                },
                "Cannot determine status of service {0}: {1}");
            if (status == null)
            {
                status = string.Empty;
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
        /// Checks if the service is installed.
        /// </summary>
        /// <param name="serviceName">Service name</param>
        public bool IsInstalled(string serviceName)
        {
            bool isInstalled = false;
            string errorMessage = "Cannot determine wheter service {0} is installed: {1}";

            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    ServiceControllerStatus status = sc.Status;
                    if (!string.IsNullOrEmpty(status.ToString()))
                    {
                        isInstalled = true;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // check if the exception was really due to the service is not
                // instaled or there is another cause
                if (!ex.Message.Contains("was not found on computer") &&
                    !ex.InnerException.Message.Contains(
                    "The specified service does not exist as an installed service"))
                {
                    Console.Error.WriteLine(errorMessage, serviceName, ex.Message);
                    throw ex;
                }

            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.Error.WriteLine(errorMessage, serviceName, ex.Message);
            }

            return isInstalled;
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
