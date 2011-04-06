using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace DaemonNT.Configuration
{  
    internal sealed class InstallerSetting
    {                  
        public String Description { set; get; }

        public String StartMode { set; get; }

        public String Account { set; get; }
             
        public String User { set; get; }

        public String Pwd { set; get; }

        public String[] DependentOn { set; get; }

        public InstallerSetting()
        {
            this.Description = "";
            this.StartMode = "Manual";
            this.Account = "LocalSystem";
            this.User = null;
            this.Pwd = null;
            this.DependentOn = new String[0];
        }
    }
}
