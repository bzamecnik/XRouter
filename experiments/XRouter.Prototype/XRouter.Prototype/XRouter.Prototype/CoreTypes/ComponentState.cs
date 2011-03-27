using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.CoreTypes
{
    class ComponentState
    {
        public string ComponentName { get; private set; }

        public ComponentStatus Status { get; private set; }

        public ComponentState(string componentName, ComponentStatus status)
        {
            ComponentName = componentName;
            Status = status;
        }
    }
}
