using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Remoting;

namespace XRouter.Management
{
    public class EndpointAddress : IRemotelyCloneable
    {
        private static readonly string SerializationSeparator = "[[/_-a2g*&tv!*_(]]";

        public string GatewayName { get; private set; }

        public string EndpointsPluginName { get; private set; }

        public string EndpointName { get; private set; }

        public EndpointAddress(string gatewayName, string endpointsPluginName, string endpointName)
        {
            GatewayName = gatewayName;
            EndpointsPluginName = endpointsPluginName;
            EndpointName = endpointName;
        }

        public override int GetHashCode()
        {
            return GatewayName.GetHashCode() ^ EndpointName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is EndpointAddress) {
                var other = (EndpointAddress)obj;
                bool result = (other.GatewayName == GatewayName) && (other.EndpointsPluginName == EndpointsPluginName) && (other.EndpointName == EndpointName);
                return result;
            }
            return false;
        }

        public override string ToString()
        {
            string result = string.Format("{0}/{1}/{2}", GatewayName, EndpointsPluginName, EndpointName);
            return result;
        }

        public string SerializeClone()
        {
            string result = string.Format("{1}{0}{2}{0}{3}", SerializationSeparator, GatewayName, EndpointsPluginName, EndpointName);
            return result;
        }

        public void DeserializeClone(string serialized)
        {
            string[] parts = serialized.Split(new string[] { SerializationSeparator }, StringSplitOptions.None);
            GatewayName = parts[0];
            EndpointsPluginName = parts[1];
            EndpointName = parts[2];
        }
    }
}
