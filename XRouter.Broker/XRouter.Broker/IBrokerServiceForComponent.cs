using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    public interface IBrokerServiceForComponent : IRemotelyReferable
    {
        void UpdateComponentControllerAddress(string componentName, Uri controllerAddress);
        void UpdateComponentInfo(string componentName, Uri componentAddress, XmlReduction configReduction);
    }
}
