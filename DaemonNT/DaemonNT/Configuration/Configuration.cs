using System;
using System.Collections.Generic;

namespace DaemonNT.Configuration
{
    [Serializable]
    internal class Configuration
    {
        public IList<ServiceSettings> Services { get; set; }

        public Configuration()
        {
            Services = new List<ServiceSettings>();
        }
    }
}
