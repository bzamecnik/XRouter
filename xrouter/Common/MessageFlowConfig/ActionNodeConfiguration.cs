using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a serializable configuration of an action node in a
    /// message flow.
    /// </summary>
    /// <see cref="XRouter.Processor.MessageFlowParts.ActionNode"/>
    [DataContract]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class ActionNodeConfiguration : NodeConfiguration
    {
        /// <summary>
        /// Action plugins for this action node.
        /// </summary>
        [DataMember]
        public List<ActionConfiguration> Actions { get; private set; }

        /// <summary>
        /// Node to which continue after performing the action in this node.
        /// </summary>
        [DataMember]
        public NodeConfiguration NextNode { get; set; }

        public ActionNodeConfiguration()
        {
            Actions = new List<ActionConfiguration>();
        }
    }
}
