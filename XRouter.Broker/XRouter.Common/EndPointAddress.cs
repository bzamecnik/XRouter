using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public class EndPointAddress
    {
        public string GatewayName { get; private set; }

        public string AdapterName { get; private set; }

        public string EndPointName { get; private set; }

        public EndPointAddress(string gatewayName, string adapterName, string endpointName)
        {
            GatewayName = gatewayName;
            AdapterName = adapterName;
            EndPointName = endpointName;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is EndPointAddress) {
                return obj.ToString() == ToString();
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2]", GatewayName, AdapterName, EndPointName);
        }
    }
}
