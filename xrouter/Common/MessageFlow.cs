using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    [Serializable]
    public class MessageFlow
    {
        public Guid Guid { get; private set; }

        public string Name { get; set; }

        public int Version { get; private set; }

        public MessageFlow(string name, int version)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Version = version;
        }
    }
}
