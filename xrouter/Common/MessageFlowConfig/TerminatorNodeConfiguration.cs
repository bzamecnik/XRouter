using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    public class TerminatorNodeConfiguration : NodeConfiguration
    {
        [DataMember]
        public bool IsReturningOutput { get; set; }

        [DataMember]
        public TokenSelection ResultMessageSelection { get; set; }
    }
}