using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace XRouter.Common
{
    [DataContract]
    public class AdapterConfiguration
    {
        [DataMember]
        public string AdapterName { get; set; }

        [DataMember]
        public string AdapterTypeName { get; private set; }

        [DataMember]
        public SerializableXDocument Configuration { get; private set; }

        public AdapterConfiguration(string adapterName, string adapterTypeName)
        {
            AdapterName = adapterName;
            AdapterTypeName = adapterTypeName;
            Configuration = new SerializableXDocument(XDocument.Parse("<objectConfig />"));
        }
    }
}
