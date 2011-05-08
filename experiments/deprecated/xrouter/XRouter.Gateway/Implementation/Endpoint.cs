using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;

namespace XRouter.Gateway.Implementation
{
    abstract class Endpoint : IEndpoint
    {
        public EndpointAddress Address { get; private set; }

        protected Endpoint(EndpointAddress address)
        {
            Address = address;
        }
    }
}
