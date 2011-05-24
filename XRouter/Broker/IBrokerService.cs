using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Broker
{
    public interface IBrokerService : IBrokerServiceForManagement, IBrokerServiceForGateway, IBrokerServiceForProcessor, IBrokerServiceForHost
    {
    }
}
