using System;
using System.Runtime.Serialization;
using System.Windows;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a serializable configuration of a node in a message flow.
    /// </summary>
    /// <seealso cref="XRouter.Processor.MessageFlowParts.Node"/>
    [DataContract]
    public abstract class NodeConfiguration
    {
        /// <summary>
        /// Event fired when the Name property is changed.
        /// </summary>
        public event Action NameChanged = delegate { };

        /// <summary>
        /// Name of the node. Fires the NameChanged event when changed.
        /// </summary>
        [DataMember]
        private string _name;
        public string Name {
            get { return _name; }
            set { 
                _name = value;
                NameChanged();
            }
        }

        /// <summary>
        /// Location of the node box on screen in a GUI editor.
        /// </summary>
        [DataMember]
        public Point Location { get; set; }
    }
}
