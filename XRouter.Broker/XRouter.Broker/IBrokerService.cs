using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    public interface IBrokerService : IBrokerServiceForManagement, IBrokerServiceForGateway, IBrokerServiceForProcessor
    {    
    }
}
