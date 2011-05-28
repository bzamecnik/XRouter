using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    public abstract class NodeConfiguration
    {
        [DataMember]
        public string Name { get; set; }
    }
}
