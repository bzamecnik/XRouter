namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    internal sealed class InstallerSettings
    {
        public InstallerSettings()
        {
            this.Description = string.Empty;
            this.StartMode = "Manual";
            this.Account = "LocalSystem";
            this.User = null;
            this.Password = null;
            this.RequiredServices = new List<string>();
        }

        public string Description { get; set; }

        /// <summary>
        /// Mode in which the service will be started.
        /// Valid values are: 'Automatic', 'Manual', 'Disabled'.
        /// </summary>
        /// <see cref="ProjectInstaller.ServiceStartMode"/>
        public string StartMode { get; set; }

        public string Account { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Names of services which this service depends on, ie. which must be running
        /// before this service can be started.
        /// </summary>
        public IEnumerable<string> RequiredServices { get; set; }
    }
}
