using System;

namespace XRouter.Gateway
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AdapterPluginAttribute : Attribute
    {
        public string PluginName { get; private set; }
        public string PluginDescription { get; private set; }

        public AdapterPluginAttribute(string name, string description)
        {
            PluginName = name;
            PluginDescription = description;
        }
    }
}
