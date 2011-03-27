using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Prototype.CoreTypes;

namespace XRouter.Prototype.CoreServices
{
    interface IConfigurationManagement
    {
        bool IsCentralManagerRunning { get; }

        void StartCentralManager();
        void StopCentralManager();

        ComponentInfo[] GetComponentsInfo();

        ApplicationConfiguration GetConfiguration();
        void ChangeConfiguration(ApplicationConfiguration config);
    }
}
