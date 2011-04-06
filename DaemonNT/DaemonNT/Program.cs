using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DaemonNT.Logging;
using DaemonNT.Configuration;

namespace DaemonNT
{    
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
        static void CommandDebug(String serviceName)
        {
            Logger logger = Logger.Start(serviceName);
            try
            {               
                ServiceSetting serviceSetting = ConfigProvider.LoadServiceSetting(serviceName);
                Service service = TypesProvider.CreateService(serviceSetting.TypeClass, serviceSetting.TypeAssembly);
                ServiceDebugHost serviceHost = new ServiceDebugHost(serviceName, serviceSetting, logger, service);
                
                serviceHost.Start();
                Console.WriteLine(String.Format("Press enter to stop service {0}...", serviceName));
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
        static void CommandRun(String serviceName)
        {
            Logger logger = Logger.Start(serviceName);
            try
            {
                ServiceSetting serviceSetting = ConfigProvider.LoadServiceSetting(serviceName);
                Service service = TypesProvider.CreateService(serviceSetting.TypeClass, serviceSetting.TypeAssembly);
                ServiceRuntimeHost serviceHost = new ServiceRuntimeHost(serviceName, serviceSetting, logger, service);

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
        static void CommandInstall(String serviceName)
        {
            // load installer setting
            ServiceSetting serviceSetting = null;
            try
            {
                serviceSetting = ConfigProvider.LoadServiceSetting(serviceName);                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e.Message);
            }

            // install
            try
            {               
                Installation.ProjectInstaller.Initialize(serviceName, serviceSetting.InstallerSetting);
                Installation.InstallerServices.Install(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", String.Concat(e.Message, String.Format(" The log file is located at {0}.Installer.log.", serviceName)));
            }        
        }

        /// <summary>
        /// Provede odinstalovani sluzby. 
        /// TODO: Pozdeji lepe zdokumentovat dle GoogleDocu. 
        /// </summary>
        /// <param name="serviceName"></param>
        static void CommandUninstall(String serviceName)
        {                 
            try
            {
                Installation.ProjectInstaller.Initialize(serviceName, null);
                Installation.InstallerServices.Uninstall(serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", String.Concat(e.Message, String.Format(" The log file is located at {0}.Installer.log.", serviceName)));
            }                  
        }
        
        static void Main(string[] args)
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
                }
            }    
           
        }       
    }
}
