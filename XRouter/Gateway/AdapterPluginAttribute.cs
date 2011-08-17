using System;

namespace XRouter.Gateway
{
    /// <summary>
    /// Marks a class as being an adapter plugin. It can attach a plugin name
    /// and description useful eg. in a GUI editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AdapterPluginAttribute : Attribute
    {
        /// <summary>
        /// Adapter plugin name.
        /// </summary>
        /// <remarks>This is not an identifier an can have an arbitrary foramat.
        /// </remarks>
        public string PluginName { get; private set; }

        /// <summary>
        /// Adapter plugin description.
        /// </summary>
        public string PluginDescription { get; private set; }

        public AdapterPluginAttribute(string name, string description)
        {
            PluginName = name;
            PluginDescription = description;
        }
    }
}
