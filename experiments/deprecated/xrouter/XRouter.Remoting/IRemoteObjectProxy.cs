using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    interface IRemoteObjectProxy
    {
        RemoteObjectAddress RemoteObjectAddress { get; }
    }
}
