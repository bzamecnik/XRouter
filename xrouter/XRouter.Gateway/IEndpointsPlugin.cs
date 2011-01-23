using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;

namespace XRouter.Gateway
{
    /// <summary>
    /// <remarks>
    /// Constructor must have signature:
    ///     constructor(XElement configuration, IEndpointsPluginService service)
    /// </remarks>
    /// </summary>
    public interface IEndpointsPlugin
    {
        void Start();
        void Stop();
    }
}
