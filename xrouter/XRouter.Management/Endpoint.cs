using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Management
{
    public abstract class Endpoint
    {
        public EndpointAddress Address { get; private set; }

        protected Endpoint(EndpointAddress address)
        {
            Address = address;
        }
    }
}
