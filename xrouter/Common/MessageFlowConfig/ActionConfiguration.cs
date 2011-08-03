using System.Runtime.Serialization;
using ObjectConfigurator;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a serializable configuration of an action plugin.
    /// </summary>
    /// <see cref="XRouter.Processor.IActionPlugin"/>
    [DataContract]
    public class ActionConfiguration
    {
        /// <summary>
        /// Full name of the action plugin type.
        /// </summary>
        /// <see cref="System.Type"/>
        [DataMember]
        public string PluginTypeFullName { get; set; }

        /// <summary>
        /// XML configuration of the action plugin.
        /// </summary>
        [DataMember]
        public SerializableXDocument Configuration { get; set; }

        /// <summary>
        /// Metadata about the action plugin class.
        /// </summary>
        [DataMember]
        public ClassMetadata ConfigurationMetadata { get; set; }
    }
}
