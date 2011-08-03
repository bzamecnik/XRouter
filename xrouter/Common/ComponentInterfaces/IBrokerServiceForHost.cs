using System.Collections.Generic;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IBrokerServiceForHost
    {
        void Start(string dbConnectionString, IEnumerable<GatewayProvider> gatewayProviders, IEnumerable<ProcessorProvider> processorProviders);
        void Stop();
    }
}
