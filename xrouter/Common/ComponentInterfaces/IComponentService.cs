namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// Basic API of an XRouter component.
    /// </summary>
    /// <remarks>
    /// A component can be at least configured.
    /// </remarks>
    public interface IComponentService
    {
        /// <summary>
        /// Replaces current configuration of the component with a new
        /// application configuration.
        /// </summary>
        /// <param name="config">application configuration</param>
        void UpdateConfig(ApplicationConfiguration config);
    }
}
