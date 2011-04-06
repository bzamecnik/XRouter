using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace DaemonNT.Installation
{
    /// <summary>
    /// Implementace zabudovaneho instalatoru. 
    /// 
    /// Poznamka SB: Trida by mela mit viditelnost internal. Diky pozadovane reflexi se mi 
    /// to zatim nepodarilo zmenit. 
    /// </summary>
    [System.ComponentModel.RunInstaller(true)]
    public sealed class ProjectInstaller : System.Configuration.Install.Installer
    {
        private static String servicename = null;

        private static DaemonNT.Configuration.InstallerSetting setting = null;

        private ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();

        private ServiceInstaller serviceInstaller =  new ServiceInstaller();
                 
        /// <summary>
        /// Metoda musi byt zavolana pred vytvorenim instance teto tridy. Pro proces instalace je 
        /// nutne dodat parametr installerSetting. 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="installerSetting">Parametr muze byt null.</param>      
        internal static void Initialize(String serviceName, DaemonNT.Configuration.InstallerSetting installerSetting)
        {
            servicename = serviceName;
            setting = installerSetting;
        }
                      
        public ProjectInstaller()
        {          
            // ServiceName
            this.serviceInstaller.ServiceName = servicename;
            
            if (setting != null)
            {
                // DisplayName
                this.serviceInstaller.DisplayName = String.Format("DaemonNT {0}", servicename);

                // Description
                this.serviceInstaller.Description = setting.Description;

                // StartType
                this.serviceInstaller.StartType = GetServiceStartModeFromString(setting.StartMode);
                
                // DependedOn
                this.serviceInstaller.ServicesDependedOn = setting.DependentOn;

                // Account
                this.serviceProcessInstaller.Account = GetServiceAccountFromString(setting.Account);
                if (this.serviceProcessInstaller.Account == ServiceAccount.User)
                {
                    this.serviceProcessInstaller.Username = setting.User;
                    this.serviceProcessInstaller.Username = setting.Pwd;
                }
            }
                                                 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] { this.serviceProcessInstaller,
                    this.serviceInstaller});
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            // prida parametry prikazove radky za assemblypath tak, aby se pomoci DaemonNT.exe spustila sprava sluzba
            String assemblyPath = Context.Parameters["assemblypath"];
            assemblyPath = String.Concat("\"", assemblyPath, "\"", " run ", this.serviceInstaller.ServiceName);
            Context.Parameters["assemblypath"] = assemblyPath;

            base.Install(stateSaver);
        }

        private ServiceStartMode GetServiceStartModeFromString(String startMode)
        {
            switch (startMode)
            {
                case "Automatic":
                    return ServiceStartMode.Automatic;
                case "Manual":
                    return ServiceStartMode.Manual;
                case "Disabled":
                    return ServiceStartMode.Disabled;
            }
            return ServiceStartMode.Manual;
        }

        private ServiceAccount GetServiceAccountFromString(String account)
        {
            switch (account)
            {
                case "LocalSystem":
                    return ServiceAccount.LocalSystem;
                case "LocalService":
                    return ServiceAccount.LocalService;
                case "NetworkService":
                    return ServiceAccount.NetworkService;
                case "User":
                    return ServiceAccount.User;
            }
            return ServiceAccount.LocalSystem;
        }
    }
}
