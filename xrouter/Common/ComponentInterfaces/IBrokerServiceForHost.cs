using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IBrokerServiceForHost
    {
        void Start(IEnumerable<GatewayProvider> gatewayProviders, IEnumerable<ProcessorProvider> processorProviders);
        void Stop();
    }
}
