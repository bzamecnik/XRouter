using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IHostableComponent
    {
        XmlReduction ConfigurationReduction { get; }

        void Start();
        void Stop();
    }
}
