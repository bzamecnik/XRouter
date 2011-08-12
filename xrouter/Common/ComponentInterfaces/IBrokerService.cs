namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component. It integrates API views for several other
    /// components.
    /// </summary>
    public interface IBrokerService : IBrokerServiceForGateway, IBrokerServiceForProcessor, IBrokerServiceForHost
    {
        ApplicationConfiguration GetConfiguration();
        void UpdateConfiguration(ApplicationConfiguration configuration);
    }
}
