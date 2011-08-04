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
        void UpdateConfig(ApplicationConfiguration config);
    }
}
