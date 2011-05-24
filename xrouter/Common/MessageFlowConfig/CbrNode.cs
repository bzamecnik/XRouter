using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.Xrm;

namespace XRouter.Common.MessageFlowConfig
{
    [Serializable]
    public class CbrNodeConfiguration : NodeConfiguration
    {
        public TokenSelection TestedSelection { get; set; }

        public IDictionary<XrmTarget/*Test*/, NodeConfiguration/*TargetNode*/> Branches { get; private set; }

        public NodeConfiguration DefaultTarget { get; set; }

        public CbrNodeConfiguration()
        {
            Branches = new Dictionary<XrmTarget, NodeConfiguration>();
        }
    }
}
