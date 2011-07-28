using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IComponentController
    {
        void StartComponent();
        void StopComponent();

        bool IsComponentRunning();
    }
}
