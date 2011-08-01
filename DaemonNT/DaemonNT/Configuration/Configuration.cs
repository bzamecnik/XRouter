using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
