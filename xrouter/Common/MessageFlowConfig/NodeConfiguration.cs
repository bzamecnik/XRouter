using System;
using System.Runtime.Serialization;
using System.Windows;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    public abstract class NodeConfiguration
    {
        public event Action NameChanged = delegate { };

        [DataMember]
        private string _name;
        public string Name {
            get { return _name; }
            set { 
                _name = value;
                NameChanged();
            }
        }

        [DataMember]
        public Point Location { get; set; }
    }
}
