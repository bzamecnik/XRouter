using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    [KnownType(typeof(EntryNodeConfiguration))]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class EntryNodeConfiguration : NodeConfiguration
    {
        /// <summary>
        /// Node to which continue after performing the action in this node.
        /// </summary>
        [DataMember]
        public NodeConfiguration NextNode { get; set; }

        public EntryNodeConfiguration()
        {
        }
    }
}
