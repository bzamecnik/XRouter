namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    internal sealed class InstallerSetting
    {
        public InstallerSetting()
        {
            this.Description = string.Empty;
            this.StartMode = "Manual";
            this.Account = "LocalSystem";
            this.User = null;
            this.Password = null;
            this.RequiredServices = new List<string>();
        }

        public string Description { get; set; }

        public string StartMode { get; set; }

        public string Account { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Names of services which this service depends on, ie. which must be runnning
        /// before this service can be started.
        /// </summary>
        public IEnumerable<string> RequiredServices { get; set; }
    }
}
