using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlow
{
    [Serializable]
    public class TerminatorNodeConfiguration : NodeConfiguration
    {
        public bool IsReturningOutput { get; set; }

        public TokenSelection OutputMessageSelection { get; set; }
    }
}