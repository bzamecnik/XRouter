using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IBrokerServiceForManagement : IRemotelyReferable
    {
        // NOTE: there should not be a single property instead of the following
        // two methods
        // - these methods can execute slowly, non-locally and with side-effects

        ApplicationConfiguration GetConfiguration();
        void ChangeConfiguration(ApplicationConfiguration config);
    }
}
