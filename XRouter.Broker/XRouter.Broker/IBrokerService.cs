using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    // NOTE:
    // - only methods can be called (no events)
    // - only serializable method arguments can be passed (no remote references)

    public interface IBrokerService : IBrokerServiceForManagement, IBrokerServiceForGateway, IBrokerServiceForProcessor
    {
    }
}
