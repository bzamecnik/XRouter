namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a processor component.
    /// </summary>
    public interface IProcessorService : IComponentService
    {
        /// <summary>
        /// Starts the processor component instance.
        /// </summary>
        /// <remarks>This method can be called only when the gateway instance
        /// is not running. It returns as soon as the gateway is started.
        /// </remarks>
        /// <param name="componentName">identifier of the processor</param>
        /// <param name="brokerService">reference to the broker component
        /// </param>
        void Start(string componentName, IBrokerServiceForProcessor brokerService);

        /// <summary>
        /// Stops a running processor component.
        /// </summary>
        void Stop();

        /// <summary>
        /// Obtains the percent of current utilization of a processor, ie. the
        /// ration of its current load to its processing capacity.
        /// </summary>
        /// <returns>current processor utilization [0.0-1.0]</returns>
        double GetUtilization();

        /// <summary>
        /// Passes a token to the processor to be processed.
        /// </summary>
        /// <remarks>This method does not wait for the token to be actually
        /// processed.</remarks>
        /// <param name="token">token to be processed</param>
        void AddWork(Token token);
    }
}
