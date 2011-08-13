using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Prototype.CoreTypes;

namespace XRouter.Prototype.CoreServices
{
    class ConnectComponentResult
    {
        public ApplicationConfiguration Configuration { get; private set; }

        public ConnectComponentResult(ApplicationConfiguration config)
        {
            Configuration = config;
        }
    }
}
