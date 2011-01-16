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
    ///     constructor(XElement configuration,  IGateway gateway)
    /// </remarks>
    /// </summary>
    public interface IEndpointsPlugin
    {
        IEnumerable<InputEndpoint> InputEndpoints { get; }
        IEnumerable<OutputEndpoint> OutputEndpoints { get; }

        void Start();
        void Stop();
    }
}
