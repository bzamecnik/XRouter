using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class MessageFlowConfiguration
    {
        [DataMember]
        public Guid Guid { get; private set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public NodeConfiguration RootNode { get; set; }

        [DataMember]
        public IList<NodeConfiguration> Nodes { get; private set; }

        public MessageFlowConfiguration(string name, int version)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Version = version;
            Nodes = new List<NodeConfiguration>();
        }
    }
}
