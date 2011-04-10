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
            Logger logger = Logger.Start(serviceName);
            try
            {
                ServiceSettings serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
                Service service = TypesProvider.CreateService(serviceSettings.TypeClass, serviceSettings.TypeAssembly);
                ServiceDebugHost serviceHost = new ServiceDebugHost(service, serviceName, serviceSettings, logger);

                serviceHost.Start();
                Console.WriteLine(string.Format("Press enter to stop service {0}...", serviceName));
                Console.ReadLine();
                serviceHost.Stop();
            }
            catch (Exception e)
            {
                logger.Event.LogError(e.Message);
            }

            logger.Stop();
        }

        /// <summary>
        ///  Spusti nainstalovanou sluzbu. Spousti to Service Control Manager (nikoliv primo uzivatel).
        ///  TODO: Pozdeji lepe zdokumentovat dle GoogleDocu.
        /// </summary>
        /// <param name="serviceName"></param>
        public static void CommandRun(string serviceName)
        {
            Logger logger = Logger.Start(serviceName);
            try
            {
                ServiceSettings serviceSettings = ConfigProvider.LoadServiceSetting(serviceName);
                Service service = TypesProvider.CreateService(serviceSettings.TypeClass, serviceSettings.TypeAssembly);
                ServiceRuntimeHost serviceHost = new ServiceRuntimeHost(service, serviceName, serviceSettings, logger);

                ServiceRuntimeHost.Run(serviceHost);
            }
            catch (Exception e)
            {
                logger.Event.LogError(e.Message);
            }

            logger.Stop();
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
