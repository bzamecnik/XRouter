using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlow
{
    [Serializable]
    public class ActionConfiguration
    {
        public string PluginTypeFullName { get; set; }

        public SerializableXDocument PluginConfiguration { get; set; }
    }
}
