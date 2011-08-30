using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace XRouter.Common
{
    public class PluginInfo<TPluginAttribute>
        where TPluginAttribute : Attribute
    {
        public string AssemblyFullPath { get; private set; }

        public string TypeFullName { get; private set; }

        public TPluginAttribute PluginAttribute { get; private set; }

        public Type PluginType { get; private set; }

        public PluginInfo(Assembly pluginAssembly, Type pluginType)
        {
            AssemblyFullPath = pluginAssembly.Location;
            TypeFullName = pluginType.FullName;
            PluginAttribute = GetPluginAttribute(pluginType);
            PluginType = pluginType;
        }

        internal PluginInfo(string assemblyFullPath, Type pluginType, TPluginAttribute pluginAttribute)
        {
            AssemblyFullPath = assemblyFullPath;
            TypeFullName = pluginType.FullName;
            PluginAttribute = pluginAttribute;
            PluginType = pluginType;
        }

        internal static TPluginAttribute GetPluginAttribute(Type pluginType)
        {
            TPluginAttribute result = (TPluginAttribute)pluginType.GetCustomAttributes(typeof(TPluginAttribute), true).FirstOrDefault();
            return result;
        }
    }
}
