namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API for an XRouter component.
    /// </summary>
    /// <remarks>
    /// A component can be at least configured.
    /// </remarks>
    public interface IComponentService
    {
        void UpdateConfig(ApplicationConfiguration config);
    }
}
