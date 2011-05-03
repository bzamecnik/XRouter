using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    public interface IBrokerServiceForManagement : IRemotelyReferable
    {
        void StartComponents();
        void StopComponents();

        ApplicationConfiguration GetConfiguration();
        void ChangeConfiguration(ApplicationConfiguration config);
    }
}
