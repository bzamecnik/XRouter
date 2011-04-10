namespace DaemonNT.Configuration
{
    internal sealed class ServiceSettings
    {
        public ServiceSettings()
        {
        }

        public string TypeClass { get; set; }

        public string TypeAssembly { get; set; }

        public Settings Settings { get; set; }

        public InstallerSettings InstallerSettings { get; set; }
    }
}
