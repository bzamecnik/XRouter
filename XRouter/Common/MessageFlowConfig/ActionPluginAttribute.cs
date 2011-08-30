using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Marks a class as being an action plugin. It can attach a plugin name
    /// and description useful eg. in a GUI editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ActionPluginAttribute : Attribute
    {
        /// <summary>
        /// Action plugin name.
        /// </summary>
        /// <remarks>This is not an identifier an can have an arbitrary foramat.
        /// </remarks>
        public string PluginName { get; private set; }

        /// <summary>
        /// Action plugin description.
        /// </summary>
        public string PluginDescription { get; private set; }

        public ActionPluginAttribute(string name, string description)
        {
            PluginName = name;
            PluginDescription = description;
        }
    }
}
