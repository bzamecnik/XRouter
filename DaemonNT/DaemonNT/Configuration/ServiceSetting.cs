using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Configuration
{
    internal sealed class ServiceSetting
    {        
        public String TypeClass { set; get; }

        public String TypeAssembly { set; get; }

        public Setting Setting { set; get; }
       
        public InstallerSetting InstallerSetting { set; get; }
       
        public ServiceSetting()
        { 
        
        }
    }
}
