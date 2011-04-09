namespace DaemonNT.Installation
{
    using System.Linq;
    using System.ServiceProcess;

    /// <summary>
    /// Implementace zabudovaneho instalatoru. 
    /// 
    /// Poznamka SB: Trida by mela mit viditelnost internal. Diky pozadovane reflexi se mi 
    /// to zatim nepodarilo zmenit. 
    /// </summary>
    [System.ComponentModel.RunInstaller(true)]
    public sealed class ProjectInstaller : System.Configuration.Install.Installer
    {
        private static string servicename = null;

        private static DaemonNT.Configuration.InstallerSetting setting = null;

        private ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();

        private ServiceInstaller serviceInstaller = new ServiceInstaller();

        public ProjectInstaller()
        {
            // ServiceName
            this.serviceInstaller.ServiceName = servicename;

            if (setting != null)
            {
                // DisplayName
                this.serviceInstaller.DisplayName = string.Format("DaemonNT {0}", servicename);

                // Description
                this.serviceInstaller.Description = setting.Description;

                // StartType
                this.serviceInstaller.StartType = this.GetServiceStartModeFromString(setting.StartMode);

                // DependedOn
                this.serviceInstaller.ServicesDependedOn = setting.RequiredServices.ToArray();

                // Account
                this.serviceProcessInstaller.Account = this.GetServiceAccountFromString(setting.Account);
                if (this.serviceProcessInstaller.Account == ServiceAccount.User)
                {
                    this.serviceProcessInstaller.Username = setting.User;
                    this.serviceProcessInstaller.Password = setting.Password;
                }
            }

            this.Installers.AddRange(
                new System.Configuration.Install.Installer[]
                {
                    this.serviceProcessInstaller,
                    this.serviceInstaller
                });
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            // prida parametry prikazove radky za assemblypath tak, aby se pomoci DaemonNT.exe spustila sprava sluzba
            string assemblyPath = Context.Parameters["assemblypath"];
            assemblyPath = string.Concat("\"", assemblyPath, "\"", " run ", this.serviceInstaller.ServiceName);
            Context.Parameters["assemblypath"] = assemblyPath;

            base.Install(stateSaver);
        }

        /// <summary>
        /// Metoda musi byt zavolana pred vytvorenim instance teto tridy. Pro proces instalace je 
        /// nutne dodat parametr installerSetting. 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="installerSetting">Parametr muze byt null.</param>      
        internal static void Initialize(string serviceName, DaemonNT.Configuration.InstallerSetting installerSetting)
        {
            servicename = serviceName;
            setting = installerSetting;
        }

        private ServiceStartMode GetServiceStartModeFromString(string startMode)
        {
            switch (startMode)
            {
                case "Automatic":
                    return ServiceStartMode.Automatic;
                case "Manual":
                    return ServiceStartMode.Manual;
                case "Disabled":
                    return ServiceStartMode.Disabled;
                // TODO: default
            }

            return ServiceStartMode.Manual;
        }

        private ServiceAccount GetServiceAccountFromString(string account)
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
                // TODO: default
            }

            return ServiceAccount.LocalSystem;
        }
    }
}
