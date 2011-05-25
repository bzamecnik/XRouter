using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IBrokerService : IBrokerServiceForManagement, IBrokerServiceForGateway, IBrokerServiceForProcessor, IBrokerServiceForHost
    {
    }
}
