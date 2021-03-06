﻿namespace DaemonNT.Configuration
{
    using System;

    // TODO: rename to ServiceConfig to avoid confusion with Settings

    /// <summary>
    /// Represents the information and settings of a service and its
    /// facilities.
    /// </summary>
    /// <remarks>
    /// Service information comprises service type (class and assembly),
    /// service settings themselves and settings of service installer
    /// and trace logger.
    /// </remarks>
    /// <seealso cref="DaemonNT.Installation.ProjectInstaller"/>
    [Serializable]
    internal sealed class ServiceSettings
    {
        public ServiceSettings()
        {
            Settings = new Settings();
            InstallerSettings = new InstallerSettings();
            TraceLoggerSettings = new TraceLoggerSettings();
        }

        /// <summary>
        /// Identifier of the service.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Identifier of the class defining the service type.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public string TypeClass { get; set; }

        // TODO: not is can be null, but this probably is not serirable in
        // general

        /// <summary>
        /// Path to and assembly where the service type is located.
        /// </summary>
        /// <remarks>
        /// Can be both relative or absolute. Relative path is based on the
        /// current appdomain base directory.
        /// </remarks>
        public string TypeAssembly { get; set; }

        /// <summary>
        /// Service settings itself (key-value pairs in a hierarchy of
        /// sections).
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public Settings Settings { get; set; }

        /// <summary>
        /// Settings of the service's installer.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public InstallerSettings InstallerSettings { get; set; }

        /// <summary>
        /// Settings of the service's trace logger.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public TraceLoggerSettings TraceLoggerSettings { get; set; }
    }
}
