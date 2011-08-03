
namespace DaemonNT
{
    /// <summary>
    /// Arguments for starting the service with the Service.OnStart() method.
    /// </summary>
    /// <see cref="Service"/>
    public sealed class OnStartServiceArgs
    {
        /// <summary>
        /// Identifier of the service in a configuration file or the operating
        /// system.
        /// </summary>
        public string ServiceName { internal set; get; }

        // TODO: could be renamed to DebugModeEnabled

        /// <summary>
        /// Specifies whether the instance is hosted inside the DaemonNT's
        /// debug environment.
        /// </summary>
        public bool IsDebugMode { internal set; get; }

        /// <summary>
        /// Service settings as defined in the configuration file.
        /// </summary>
        public DaemonNT.Configuration.Settings Settings { internal set; get; }

        internal OnStartServiceArgs()
        { }
    }
}
