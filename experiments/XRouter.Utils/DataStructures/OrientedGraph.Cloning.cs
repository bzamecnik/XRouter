using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.DataStructures
{
    public partial class OrientedGraph<TNode>
    {
        public OrientedGraph<TTargetNode> Clone<TTargetNode>(Func<TNode, TTargetNode> convert)
        {
            var nodeMapping = new Dictionary<TNode, TTargetNode>();
            OrientedGraph<TTargetNode> result = new OrientedGraph<TTargetNode>();

            foreach (TNode node in Nodes) {
                TTargetNode convertedNode = convert(node);
                nodeMapping.Add(node, convertedNode);
                result.Nodes.Add(convertedNode);
            }

            foreach (var edge in outboundEdges.SelectMany(lst => lst.Value)) {
                var sourceNode = nodeMapping[edge.Source];
                var targetNode = nodeMapping[edge.Target];
                result.AddEdge(sourceNode, targetNode);
            }

            return result;
        }
    }
}
