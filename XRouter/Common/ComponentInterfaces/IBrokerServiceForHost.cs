using System.Collections.Generic;

namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component to be used by the XRouter component host.
    /// </summary>
    public interface IBrokerServiceForHost
    {
        /// <summary>
        /// Starts the broker component.
        /// </summary>
        /// <param name="dbConnectionString">parameters for connecting to a
        /// database</param>
        /// <param name="gatewayProviders">references to gateway components
        /// </param>
        /// <param name="processorProviders">references to processor
        /// components</param>
        void Start(
            string dbConnectionString,
            IEnumerable<GatewayProvider> gatewayProviders,
            IEnumerable<ProcessorProvider> processorProviders);

        /// <summary>
        /// Stops a running broker component.
        /// </summary>
        void Stop();
    }
}
