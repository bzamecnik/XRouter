using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlowConfig
{
    [Serializable]
    public abstract class NodeConfiguration
    {
        public string Name { get; set; }
    }
}
