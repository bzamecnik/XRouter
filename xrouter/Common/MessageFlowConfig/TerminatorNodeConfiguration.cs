using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a serializable configuration of a terminator node in a
    /// message flow.
    /// </summary>
    /// <see cref="XRouter.Processor.MessageFlowParts.TerminatorNode"/>
    [DataContract]
    public class TerminatorNodeConfiguration : NodeConfiguration
    {
        /// <summary>
        /// Indicates whether an output message should be returned when a
        /// token enters this terminator node.
        /// </summary>
        [DataMember]
        public bool IsReturningOutput { get; set; }

        /// <summary>
        /// Expression to select the output message from the token.
        /// </summary>
        [DataMember]
        public TokenSelection ResultMessageSelection { get; set; }

        public TerminatorNodeConfiguration()
        {
            ResultMessageSelection = new TokenSelection(string.Empty);
        }
    }
}