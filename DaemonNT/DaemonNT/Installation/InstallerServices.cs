using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DaemonNT.Installation
{
    /// <summary>
    /// Poskytuje implementaci operaci pro instalaci/uninstalaci služeb. 
    /// </summary>
    internal static class InstallerServices
    {
        public static void Install(String serviceName)
        {
            // specifikuje command line arguments pro InstallUtil
            List<String> args = new List<String>();
            args.Add("/LogToConsole=false");       
            args.Add(String.Format("/LogFile={0}.Installer.log", serviceName));            
            args.Add(Assembly.GetExecutingAssembly().Location); 
          
            // provede proces instalaci, vygeneruje log file
            System.Configuration.Install.ManagedInstallerClass.InstallHelper(args.ToArray());

            // odstrani soubor InstallState, ktery generuje InstallUtil
            String filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DaemonNT.InstallState");
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
        }

        public static void Uninstall(String serviceName)
        {
            // specifikuje command line arguments pro InstallUtil
            List<String> args = new List<String>();
            args.Add("/u");
            args.Add("/LogToConsole=false");
            args.Add(String.Format("/LogFile={0}.Installer.log", serviceName));
            args.Add(Assembly.GetExecutingAssembly().Location);        
    
            // provede proces uninstalace, vygeneruje log file
            System.Configuration.Install.ManagedInstallerClass.InstallHelper(args.ToArray());
        }       
    }
}
