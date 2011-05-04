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
        // implementation note:
        // - for a single-process XRouter threads can be directly
        //   started or stopped
        // - for a distributed multi-process XRouter watchdogs for
        //   components are needed

        // NOTE: start/stop component except broker itself

        void StartComponents();
        void StopComponents();

        // NOTE: there should not be a single property instead of the following
        // two methods
        // - these methods can execute slowly, non-locally and with side-effects

        ApplicationConfiguration GetConfiguration();
        void ChangeConfiguration(ApplicationConfiguration config);
    }
}
