using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;

namespace XRouter.Common.ComponentInterfaces
{
    // methods to be called by any component
    public interface IBrokerServiceForComponent : IRemotelyReferable
    {
        ApplicationConfiguration GetConfiguration(XmlReduction reduction);
    }
}
