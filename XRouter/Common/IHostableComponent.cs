using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IHostableComponent
    {
        void Start(string componentName, IDictionary<string, string> settings);
        void Stop();
    }
}
