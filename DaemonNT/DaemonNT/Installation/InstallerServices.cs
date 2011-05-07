namespace DaemonNT.Installation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Poskytuje implementaci operaci pro instalaci/uninstalaci služeb.
    /// </summary>
    internal static class InstallerServices
    {
        public static void Install(string serviceName)
        {
            // specifikuje command line arguments pro InstallUtil
            List<string> args = new List<string>();
            args.Add("/LogToConsole=false");
            args.Add(string.Format("/LogFile={0}.Installer.log", serviceName));
            args.Add(Assembly.GetExecutingAssembly().Location);

            // provede proces instalaci, vygeneruje log file
            System.Configuration.Install.ManagedInstallerClass.InstallHelper(args.ToArray());

            // odstrani soubor InstallState, ktery generuje InstallUtil
            string filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DaemonNT.InstallState");
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
        }

        public static void Uninstall(string serviceName)
        {
            // specifikuje command line arguments pro InstallUtil
            List<string> args = new List<string>();
            args.Add("/u");
            args.Add("/LogToConsole=false");
            args.Add(string.Format("/LogFile={0}.Installer.log", serviceName));
            args.Add(Assembly.GetExecutingAssembly().Location);

            // provede proces uninstalace, vygeneruje log file
            System.Configuration.Install.ManagedInstallerClass.InstallHelper(args.ToArray());
        }
    }
}
