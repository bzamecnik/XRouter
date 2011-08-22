using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace XRouter.Common
{
    /// <summary>
    /// Represent serializable configuration of a gateway adapter.
    /// </summary>
    /// <seealso cref="XRouter.Gateway.Adapter"/>
    [DataContract]
    public class AdapterConfiguration
    {
        [DataMember]
        public string AdapterName { get; set; }

        [DataMember]
        public string GatewayName { get; private set; }

        [DataMember]
        public string AdapterTypeName { get; set; }

        [DataMember]
        public SerializableXDocument Configuration { get; set; }

        public AdapterConfiguration(string adapterName, string gatewayName, string adapterTypeName)
        {
            AdapterName = adapterName;
            GatewayName = gatewayName;
            AdapterTypeName = adapterTypeName;
            Configuration = new SerializableXDocument(XDocument.Parse("<objectConfig />"));
        }

        public AdapterConfiguration(string adapterName, string gatewayName, string adapterTypeName, XDocument config)
        {
            AdapterName = adapterName;
            GatewayName = gatewayName;
            AdapterTypeName = adapterTypeName;
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (config.Root.Name != XName.Get("objectConfig")) {
                throw new ArgumentException("config");
            }
            Configuration = new SerializableXDocument(config);
        }
    }
}
