using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Management
{
    public interface IXRouterComponent
    {
        string Name { get; }

        void Initialize();
    }
}
