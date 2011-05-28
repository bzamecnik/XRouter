using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    public class ActionConfiguration
    {
        [DataMember]
        public string PluginTypeFullName { get; set; }

        [DataMember]
        public SerializableXDocument PluginConfiguration { get; set; }
    }
}
