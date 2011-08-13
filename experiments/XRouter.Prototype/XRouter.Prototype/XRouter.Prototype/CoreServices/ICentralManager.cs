using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;

namespace XRouter.Prototype.CoreServices
{
    interface ICentralManager : ICentralComponentServices
    {
        ConnectComponentResult ConnectComponent(string name, IComponentHost componentHost, ComponentState state, XmlReduction configReduction);
    }
}
