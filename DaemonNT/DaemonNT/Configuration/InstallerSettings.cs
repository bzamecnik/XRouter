namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents settings of a service installer.
    /// </summary>
    internal sealed class InstallerSettings
    {
        /// <summary>
        /// Creates a new instance of InstallerSettings with default values.
        /// </summary>
        /// <remarks>
        /// Default start mode is 'Manual', default account is 'LocalSystem'.
        /// </remarks>
        public InstallerSettings()
        {
            this.Description = string.Empty;
            this.StartMode = "Manual";
            this.Account = "LocalSystem";
            this.User = null;
            this.Password = null;
            this.RequiredServices = new List<string>();
        }

        internal static readonly IEnumerable<string> ValidStartModeValues =
            new[] { "Automatic", "Manual", "Disabled" };

        internal static readonly string DefaultStartModeValue = "Manual";

        internal static readonly IEnumerable<string> ValidAccountValues =
            new[] { "LocalSystem", "LocalService", "NetworkService", "User" };

        internal static readonly string DefaultAccountValue = "LocalSystem";

        /// <summary>
        /// Human-friendly description of a service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mode in which the service will be started.
        /// </summary>
        /// <remarks>
        /// Valid values are: 'Automatic', 'Manual', 'Disabled'.
        /// Default value is 'Manual'.
        /// </remarks>
        /// <see cref="ProjectInstaller.ServiceStartMode"/>
        public string StartMode { get; set; }

        /// <summary>
        /// Account with which the service will be installed.
        /// </summary>
        /// <remarks>
        /// Valid values are: 'LocalSystem', 'LocalService', 'NetworkService'
        /// and 'User'. Default value is 'LocalSystem'.
        /// If 'User' account is selected the user account detailed should be
        /// specified in User and Password properties.
        /// </remarks>
        /// <see cref="System.ServiceProcess.ServiceAccount"/>
        public string Account { get; set; }

        /// <summary>
        /// User name with which the service will be installed.
        /// </summary>
        /// <remarks>
        /// A user account specified here will be used only if the
        /// Account property is set to 'User'.
        /// </remarks>
        public string User { get; set; }

        /// <summary>
        /// Password of a user with which the service will be installed.
        /// </summary>
        /// <remarks>
        /// A password specified here will be used only if the
        /// Account property is set to 'User'.
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        /// Names of services which this service depends on, ie. which must
        /// be running before this service can be started.
        /// </summary>
        public IEnumerable<string> RequiredServices { get; set; }
    }
}
