using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Remoting;

namespace XRouter.Management
{
    public interface IEndpoint : IRemotelyReferable
    {
        EndpointAddress Address { get; }
    }
}
