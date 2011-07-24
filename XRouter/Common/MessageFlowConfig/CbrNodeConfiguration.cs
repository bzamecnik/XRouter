using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.Xrm;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class CbrNodeConfiguration : NodeConfiguration
    {
        [DataMember]
        public TokenSelection TestedSelection { get; set; }

        [DataMember]
        public IDictionary<XrmUri/*Test*/, NodeConfiguration/*TargetNode*/> Branches { get; private set; }

        [DataMember]
        public NodeConfiguration DefaultTarget { get; set; }

        public CbrNodeConfiguration()
        {
            Branches = new Dictionary<XrmUri, NodeConfiguration>();
            TestedSelection = new TokenSelection(string.Empty);
        }
    }
}
