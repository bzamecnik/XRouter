using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Broker
{
    public interface IBrokerServiceForHost
    {
        void Start();
        void Stop();
    }
}
