namespace DaemonNT.Installation
{
    using System.Collections;
    using System.Configuration.Install;
    using System.Linq;
    using System.ServiceProcess;
    using DaemonNT.Configuration;

    /// <summary>
    /// The embedded NT service installer.
    /// </summary>
    /// <remarks>
    /// TODO: This class should be marked as internal. Due to the need for
    /// reflection it must be public for now. Find out how to allow it to be
    /// internal. --SB
    /// </remarks>
    [System.ComponentModel.RunInstaller(true)]
    public sealed class ProjectInstaller : Installer
    {
        private static string servicename;

        private static InstallerSettings settings;

        private ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();

        private ServiceInstaller serviceInstaller = new ServiceInstaller();

        public ProjectInstaller()
        {
            // ServiceName
            this.serviceInstaller.ServiceName = servicename;

            if (settings != null)
            {
                // DisplayName
                this.serviceInstaller.DisplayName = string.Format("DaemonNT {0}", servicename);

                // Description
                this.serviceInstaller.Description = settings.Description;

                // StartType
                this.serviceInstaller.StartType = this.GetServiceStartModeFromString(settings.StartMode);

                // DependedOn
                this.serviceInstaller.ServicesDependedOn = settings.RequiredServices.ToArray();

                // Account
                this.serviceProcessInstaller.Account = this.GetServiceAccountFromString(settings.Account);
                if (this.serviceProcessInstaller.Account == ServiceAccount.User)
                {
                    this.serviceProcessInstaller.Username = settings.User;
                    this.serviceProcessInstaller.Password = settings.Password;
                }
            }

            this.Installers.AddRange(
                new Installer[]
                {
                    this.serviceProcessInstaller,
                    this.serviceInstaller
                });
        }

        public override void Install(IDictionary stateSaver)
        {
            // prida parametry prikazove radky za assemblypath tak, aby se pomoci DaemonNT.exe spustila sprava sluzba
            // TODO: use string.Format()
            string assemblyPath = Context.Parameters["assemblypath"];
            assemblyPath = string.Concat("\"", assemblyPath, "\"", " run ", this.serviceInstaller.ServiceName);
            Context.Parameters["assemblypath"] = assemblyPath;

            base.Install(stateSaver);
        }

        /// <summary>
        /// Initializes the installer.
        /// </summary>
        /// <remarks>
        /// This method must be called before creating logger for this class.
        /// </remarks>
        /// <param name="serviceName">Service name.</param>
        /// <param name="installerSettings">Installer settings, can be null
        /// in case of uninstallation but it is necessary for the installation
        /// process</param>
        internal static void Initialize(string serviceName, InstallerSettings installerSettings)
        {
            servicename = serviceName;
            settings = installerSettings;
        }

        // TODO: Converting from string to ServiceStartMode or ServiceAccount
        // enum instances could be done in extension methods to those enums.
        // This would be more convenient for usage and also reusable.

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
                default:
                    // TODO: better report an error
                    // (for a default value use eg. an empty string)
                    return ServiceStartMode.Manual;
            }
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
                default:
                    // TODO: better report an error
                    // (for a default value use eg. an empty string)
                    return ServiceAccount.LocalSystem;
            }
        }
    }
}
