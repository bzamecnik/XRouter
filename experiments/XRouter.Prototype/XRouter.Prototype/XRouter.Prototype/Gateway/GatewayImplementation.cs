using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.Gateway
{
    class GatewayImplementation : IGateway
    {
        public XmlReduction ConfigReduction { get { return new XmlReduction(); } }

        public void Initalize(ApplicationConfiguration config, ICentralComponentServices services)
        {
        }

        public void ChangeConfig(ApplicationConfiguration config)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
