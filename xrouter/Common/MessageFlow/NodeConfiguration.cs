using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlow
{
    [Serializable]
    public abstract class NodeConfiguration
    {
        public string Name { get; set; }
    }
}
