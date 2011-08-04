using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represent serializable configuration of a message flow.
    /// </summary>
    /// <seealso cref="XRouter.Processor.MessageFlowParts.MessageFlow"/>
    [DataContract]
    [KnownType(typeof(TerminatorNodeConfiguration))]
    [KnownType(typeof(CbrNodeConfiguration))]
    [KnownType(typeof(ActionNodeConfiguration))]
    public class MessageFlowConfiguration
    {
        [DataMember]
        public Guid Guid { get; private set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public NodeConfiguration RootNode { get; set; }

        [DataMember]
        public List<NodeConfiguration> Nodes { get; private set; }

        public MessageFlowConfiguration(string name, int version)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Version = version;
            Nodes = new List<NodeConfiguration>();
        }

        public void PromoteToNewVersion()
        {
            Version++;
            Guid = Guid.NewGuid();
        }

        public void RemoveNode(NodeConfiguration nodeToRemove)
        {
            Nodes.Remove(nodeToRemove);
            if (RootNode == nodeToRemove) {
                RootNode = null;
            }

            foreach (NodeConfiguration node in Nodes) {
                if (node is ActionNodeConfiguration) {
                    var actionNode = (ActionNodeConfiguration)node;
                    if (actionNode.NextNode == nodeToRemove) {
                        actionNode.NextNode = null;
                    }
                } else if (node is CbrNodeConfiguration) {
                    var cbrNode = (CbrNodeConfiguration)node;
                    if (cbrNode.DefaultTarget == nodeToRemove) {
                        cbrNode.DefaultTarget = null;
                    }
                    foreach (var key in cbrNode.Branches.Keys.ToArray()) {
                        if (cbrNode.Branches[key] == nodeToRemove) {
                            cbrNode.Branches.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
