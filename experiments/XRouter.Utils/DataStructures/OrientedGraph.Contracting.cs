using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.DataStructures
{
    public partial class OrientedGraph<TNode>
    {
        public void ContractNodes(Func<TNode, bool> contractPredicate)
        {
            foreach (TNode node in Nodes.ToArray()) {
                bool contract = contractPredicate(node);
                if (contract) {
                    if ((inboundEdges.ContainsKey(node)) && (outboundEdges.ContainsKey(node))) {
                        var sourceNodes = from e in inboundEdges[node] select e.Source;
                        var targetNodes = from e in outboundEdges[node] select e.Target;
                        foreach (TNode sourceNode in sourceNodes) {
                            foreach (TNode targetNode in targetNodes) {
                                AddEdge(sourceNode, targetNode);
                            }
                        }
                    }
                    Nodes.Remove(node);
                }
            }
        }
    }
}
