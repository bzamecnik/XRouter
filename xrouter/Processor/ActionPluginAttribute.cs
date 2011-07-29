using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Processor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ActionPluginAttribute : Attribute
    {
        public string PluginName { get; private set; }
        public string PluginDescription { get; private set; }

        public ActionPluginAttribute(string name, string description)
        {
            PluginName = name;
            PluginDescription = description;
        }
    }
}
