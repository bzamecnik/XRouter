using System.Runtime.Serialization;
using ObjectConfigurator;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a serializable configuration of an action plugin.
    /// </summary>
    /// <seealso cref="XRouter.Processor.IActionPlugin"/>
    [DataContract]
    public class ActionConfiguration
    {
        /// <summary>
        /// Symbolic name of the action plugin type.
        /// </summary>
        [DataMember]
        public string ActionTypeName { get; set; }

        /// <summary>
        /// XML configuration of the action plugin.
        /// </summary>
        [DataMember]
        public SerializableXDocument Configuration { get; set; }

        public ActionConfiguration(string actionTypeName)
        {
            ActionTypeName = actionTypeName;
        }
    }
}
