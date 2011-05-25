using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IComponentService : IRemotelyReferable
    {
        void UpdateConfig(ApplicationConfiguration config);
    }
}
