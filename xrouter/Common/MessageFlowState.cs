using System;
using System.Runtime.Serialization;

namespace XRouter.Common
{
    /// <summary>
    /// Represents the state of the message flow while processing a single
    /// token. Used to store the state between steps of the processing.
    /// </summary>
    [DataContract]
    public class MessageFlowState
    {
        /// <summary>
        /// Unique identifier of the message flow used to process the token.
        /// </summary>
        /// <remarks>
        /// The active message flow might change during the course of
        /// processing a token and this identifies the original message flow
        /// which was active when the token's processing started.
        /// </remarks>
        [DataMember]
        public Guid MessageFlowGuid { get; set; }

        /// <summary>
        /// Node in the message flow into which to go in the next processing
        /// step.
        /// </summary>
        /// <remarks>Can be null initially which means the traversal should
        /// start at the root node.</remarks>
        [DataMember]
        public string NextNodeName { get; set; }

        /// <summary>
        /// Identifies the processor to which the token is currently assigned.
        /// The identifier corresponds to the processor name in the
        /// configuration of XRouter components.
        /// </summary>
        /// <remarks>Can be null, which means the token is not assigned to any
        /// processor. This can happed before the token has been dispatched or
        /// when the assigned processor was lost and the token need to be
        /// redispatched to another processor.
        /// </remarks>
        [DataMember]
        public string AssignedProcessor { get; set; }

        /// <summary>
        /// Date of the last action when a processor modified the token.
        /// </summary>
        /// <remarks>This can be useful to track unresponsible processor.
        /// </remarks>
        [DataMember]
        public DateTime LastResponseFromProcessor { get; set; }

        public MessageFlowState()
        {
        }
    }
}
