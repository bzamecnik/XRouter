using System.Collections.Generic;
using System.Runtime.Serialization;
using XRouter.Common.Xrm;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a serializable configuration of an content-based router
    /// node.
    /// </summary>
    /// <seealso cref="XRouter.Processor.MessageFlowParts.CbrNode"/>
    [DataContract]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class CbrNodeConfiguration : NodeConfiguration
    {
        /// <summary>
        /// Expression to select a document to be tested from the token.
        /// </summary>
        [DataMember]
        public TokenSelection TestedSelection { get; set; }

        /// <summary>
        /// CBR branches: XRM URI of a Schematron schema -> target node
        /// configuration.
        /// </summary>
        [DataMember]
        public IDictionary<XrmUri, NodeConfiguration> Branches { get; private set; }

        /// <summary>
        /// Default node acting as a fall-back.
        /// </summary>
        [DataMember]
        public NodeConfiguration DefaultTarget { get; set; }

        public CbrNodeConfiguration()
        {
            Branches = new Dictionary<XrmUri, NodeConfiguration>();
            TestedSelection = new TokenSelection(string.Empty);
        }
    }
}
