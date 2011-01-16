using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;

namespace XRouter.Scheduler
{
    public interface IScheduler : IXRouterComponent
    {
        void ScheduleMessage(Message message);
    }
}
