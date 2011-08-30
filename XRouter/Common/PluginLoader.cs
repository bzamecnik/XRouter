using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace XRouter.Common
{
    public class PluginLoader
    {
        private static readonly string[] PluginFilePatterns = new[] { "*.dll", "*.exe" };

        public static PluginInfo<TPluginAttribute>[] FindPlugins<TPluginAttribute>(string searchDirectoryPath)
            where TPluginAttribute : Attribute
        {
            List<PluginInfo<TPluginAttribute>> result = new List<PluginInfo<TPluginAttribute>>();

            foreach (string filePattern in PluginFilePatterns) {
                var files = Directory.GetFiles(searchDirectoryPath, filePattern, SearchOption.TopDirectoryOnly);
                foreach (string file in files) {
                    Type[] types;
                    try {
                        Assembly asm = Assembly.LoadFrom(file);
                        types = asm.GetTypes();
                    } catch (Exception ex) {
                        TraceLog.Warning(string.Format("Unable to load assembly '{0}': {1}", file, ex.Message));
                        continue;
                    }
                    foreach (Type type in types) {
                        TPluginAttribute pluginAttribute;
                        try {
                            pluginAttribute = PluginInfo<TPluginAttribute>.GetPluginAttribute(type);
                        } catch (Exception) {
                            continue;
                        }
                        if (pluginAttribute != null) {
                            PluginInfo<TPluginAttribute> pluginInfo = new PluginInfo<TPluginAttribute>(file, type, pluginAttribute);
                            result.Add(pluginInfo);
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}
