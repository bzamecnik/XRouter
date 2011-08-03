namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a processor component.
    /// </summary>
    public interface IProcessorService : IComponentService
    {
        void Start(string componentName, IBrokerServiceForProcessor brokerService);
        void Stop();

        double GetUtilization();

        /// <summary>
        /// Passes a token to the processor to be processed.
        /// </summary>
        /// <param name="token">token to be processed</param>
        void AddWork(Token token);
    }
}
