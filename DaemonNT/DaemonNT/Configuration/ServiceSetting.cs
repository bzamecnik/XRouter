namespace DaemonNT.Configuration
{
    // TODO: better rename to ServiceSettings

    internal sealed class ServiceSetting
    {
        public ServiceSetting()
        {
        }

        public string TypeClass { get; set; }

        public string TypeAssembly { get; set; }

        public Setting Setting { get; set; }

        public InstallerSetting InstallerSetting { get; set; }
    }
}
