using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;

namespace XRouter.Common
{
    public interface IComponentController : IRemotelyReferable
    {
        void StartComponent();
        void StopComponent();

        bool IsComponentRunning();
    }
}
