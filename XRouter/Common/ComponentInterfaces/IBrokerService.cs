namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component. It integrates API views for several other
    /// components.
    /// </summary>
    public interface IBrokerService : IBrokerServiceForGateway, IBrokerServiceForProcessor, IBrokerServiceForHost
    {
        /// <summary>
        /// Obtains the complete XRouter application configuration.
        /// </summary>
        /// <remarks>
        /// NOTE: There should not be a single property instead of the methods
        /// GetConfiguration() and ChangeConfiguration(). These methods can
        /// execute slowly, non-locally and with side-effects.
        /// </remarks>
        /// <returns>current configuration</returns>
        ApplicationConfiguration GetConfiguration();
    }
}
