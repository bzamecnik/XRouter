
namespace XRouter.Common
{
    /// <summary>
    /// Represents an address of an adapter endpoint which can be a source
    /// or target of a message. It consists of a gateway name, adapter name
    /// and endpoint name.
    /// </summary>
    public class EndpointAddress
    {
        /// <summary>
        /// Gateway name.
        /// </summary>
        public string GatewayName { get; private set; }

        /// <summary>
        /// Adapter name.
        /// </summary>
        public string AdapterName { get; private set; }

        /// <summary>
        /// Adapter endpoint name.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        public string EndpointName { get; private set; }

        public EndpointAddress(string gatewayName, string adapterName, string endpointName)
        {
            GatewayName = gatewayName;
            AdapterName = adapterName;
            EndpointName = endpointName;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is EndpointAddress) {
                return obj.ToString() == ToString();
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", GatewayName, AdapterName, EndpointName);
        }
    }
}
