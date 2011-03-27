using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.Processor;

namespace XRouter.Prototype.CoreServices
{
    interface IComponentHost
    {
        void Start();
        void Stop();

        void OnCentralManagerStarted();
        void OnCentralManagerStopped();

        ComponentState GetState();

        void ChangeConfig(ApplicationConfiguration config);

        IProcessor GetComponentAsProcessor();
    }
}
