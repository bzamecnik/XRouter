using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;

namespace XRouter.Common
{
    public interface IComponentService : IRemotelyReferable
    {
        void UpdateConfig(ApplicationConfiguration config);
    }
}
