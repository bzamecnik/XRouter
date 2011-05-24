using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlowConfig
{
    [Serializable]
    public class MessageFlowConfiguration
    {
        public Guid Guid { get; private set; }

        public string Name { get; set; }

        public int Version { get; private set; }

        public NodeConfiguration RootNode { get; set; }

        public IList<NodeConfiguration> Nodes { get; private set; }

        public MessageFlowConfiguration(string name, int version)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Version = version;
            Nodes = new List<NodeConfiguration>();
        }
    }
}
