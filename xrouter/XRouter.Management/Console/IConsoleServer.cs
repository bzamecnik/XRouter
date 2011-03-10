using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Management.Console
{
    public interface IConsoleServer : IXRouterComponent
    {
        void ChangeConfiguration(RemotableXDocument configuration);
    }
}
