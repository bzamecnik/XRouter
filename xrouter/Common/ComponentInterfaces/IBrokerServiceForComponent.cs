namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component to be used by any component.
    /// </summary>
    public interface IBrokerServiceForComponent
    {
        /// <summary>
        /// Obtains an configuration for a component optionally filtered by
        /// given reduction in order to reduce bandwidth.
        /// </summary>
        /// <param name="reduction">reduction to filter the configuration
        /// </param>
        /// <returns>(optionally reduced) application configuration
        /// </returns>
        ApplicationConfiguration GetConfiguration(XmlReduction reduction);
    }
}
