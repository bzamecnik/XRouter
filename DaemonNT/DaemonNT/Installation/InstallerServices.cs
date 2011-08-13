namespace DaemonNT.Installation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;

    /// <summary>
    /// Provides an implementation of installing and uninstalling NT services.
    /// </summary>
    internal static class InstallerServices
    {
        /// <summary>
        /// Installs an NT service.
        /// </summary>
        /// <remarks>
        /// The service must not be installed yet.
        /// </remarks>
        /// <param name="serviceName">Service name.</param>
        public static void Install(string serviceName)
        {
            // specify command-line arguments for InstallUtil
            List<string> args = new List<string>();
            args.Add("/LogToConsole=false");
            args.Add(string.Format("/LogFile={0}.Installer.log", serviceName));
            args.Add(Assembly.GetExecutingAssembly().Location);

            // perform the installation process generating a log file
            System.Configuration.Install.ManagedInstallerClass.InstallHelper(args.ToArray());

            // remove the InstalState file produced by InstallUtil
            // TODO: should we use AppDomain.CurrentDomain.BaseDirectory or
            // Directory.GetCurrentDirectory()
            string filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DaemonNT.InstallState");
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
        }

        /// <summary>
        /// Uninstalls an NT service.
        /// </summary>
        /// <remarks>
        /// The service must be installed.
        /// </remarks>
        /// <param name="serviceName">Service name.</param>
        public static void Uninstall(string serviceName)
        {
            // specify command-line arguments for InstallUtil
            List<string> args = new List<string>();
            args.Add("/u");
            args.Add("/LogToConsole=false");
            args.Add(string.Format("/LogFile={0}.Installer.log", serviceName));
            args.Add(Assembly.GetExecutingAssembly().Location);

            // perform the uninstallation process generating a log file
            System.Configuration.Install.ManagedInstallerClass.InstallHelper(args.ToArray());
        }

        /// <summary>
        /// Gets the list of all currently installed NT services.
        /// </summary>
        /// <returns>The list of service controllers.</returns>
        public static ServiceController[] GetInstalledServices()
        {
            return ServiceController.GetServices();
        }

        /// <summary>
        /// Checks if the service is installed.
        /// </summary>
        /// <param name="serviceName">Service name</param>
        public static bool IsInstalled(string serviceName)
        {
            return GetInstalledServices().Select((service) => service.ServiceName).Contains(serviceName);
        }
    }
}
