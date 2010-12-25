using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.ComponentWatching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class WatchableComponentAttribute : Attribute
    {
        readonly string componentName;

        public string ComponentName
        {
            get { return componentName; }
        }

        public WatchableComponentAttribute(string componentName)
        {
            this.componentName = componentName;
        }
    }
}
