using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.ComponentInterfaces
{
    // methods to be called by any component
    public interface IBrokerServiceForComponent
    {
        ApplicationConfiguration GetConfiguration(XmlReduction reduction);
    }
}
