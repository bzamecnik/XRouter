using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class ActionNodeConfiguration : NodeConfiguration
    {
        [DataMember]
        public List<ActionConfiguration> Actions { get; private set; }

        [DataMember]
        public NodeConfiguration NextNode { get; set; }

        public ActionNodeConfiguration()
        {
            Actions = new List<ActionConfiguration>();
        }
    }
}
