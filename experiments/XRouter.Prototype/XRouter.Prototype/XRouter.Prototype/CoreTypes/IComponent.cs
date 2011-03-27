using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.CoreTypes
{
    interface IComponent
    {
        XmlReduction ConfigReduction { get; }

        void Initalize(ApplicationConfiguration config, ICentralComponentServices services);
        
        void ChangeConfig(ApplicationConfiguration config);

        void Start();
        void Stop();
    }
}
