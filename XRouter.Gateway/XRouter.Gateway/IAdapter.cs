using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Gateway
{
    /// <summary>
    /// <remarks>
    /// Constructor must have signature:
    ///     constructor(XElement configuration, IEndpointsPluginService service)
    /// </remarks>
    /// </summary>
    public interface IAdapter
    {
        void Start();
        void Stop();

        SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message);
    }
}
