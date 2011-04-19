using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// This is an internal (invisible) interface for proxies.
    /// </summary>
    interface IRemoteObjectProxy
    {
        RemoteObjectAddress RemoteObjectAddress { get; }
    }
}
