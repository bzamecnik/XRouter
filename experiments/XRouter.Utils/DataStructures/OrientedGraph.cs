using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.DataStructures
{
    public partial class OrientedGraph<TNode>
    {
        public ICollection<TNode> Nodes { get; private set; }

        private Dictionary<TNode, List<Edge>> inboundEdges = new Dictionary<TNode, List<Edge>>();
        private Dictionary<TNode, List<Edge>> outboundEdges = new Dictionary<TNode, List<Edge>>();

        public OrientedGraph()
        {
            Nodes = new GuardedCollection<TNode>(OnAddNode, OnRemoveNode);
        }

        public void AddEdge(TNode source, TNode target)
        {
            Edge newEdge = new Edge(source, target);
            if (!inboundEdges.ContainsKey(target)) {
                inboundEdges.Add(target, new List<Edge>());
            }
            if (!inboundEdges[target].Any(e=>e.Source.Equals(source))) {
                inboundEdges[target].Add(newEdge);
            }
            if (!outboundEdges.ContainsKey(source)) {
                outboundEdges.Add(source, new List<Edge>());
            }
            if (!outboundEdges[source].Any(e => e.Target.Equals(target))) {
                outboundEdges[source].Add(newEdge);
            }
        }

        public void RemoveEdge(TNode source, TNode target)
        {
            if (inboundEdges.ContainsKey(target)) {
                inboundEdges[target].RemoveAll(e => e.Source.Equals(source));
            }
            if (outboundEdges.ContainsKey(source)) {
                outboundEdges[source].RemoveAll(e => e.Target.Equals(target));
            }
        }

        public IEnumerable<TNode> GetInboundNodes(TNode node)
        {
            if (!inboundEdges.ContainsKey(node)) {
                inboundEdges.Add(node, new List<Edge>());
            }
            var result = from n in inboundEdges[node] select n.Source;
            return result;
        }

        public IEnumerable<TNode> GetOutboundNodes(TNode node)
        {
            if (!outboundEdges.ContainsKey(node)) {
                outboundEdges.Add(node, new List<Edge>());
            }
            var result = from n in outboundEdges[node] select n.Target;
            return result;
        }

        private void OnAddNode(TNode node)
        {            
        }

        private void OnRemoveNode(TNode node)
        {
            if (inboundEdges.ContainsKey(node)) {
                inboundEdges.Remove(node);
            }
            if (outboundEdges.ContainsKey(node)) {
                outboundEdges.Remove(node);
            }
            var allEdgesCollections = inboundEdges.Values.Concat(outboundEdges.Values);
            foreach (var edges in allEdgesCollections) {
                edges.RemoveAll(e => e.Source.Equals(node) || e.Target.Equals(node));
            }
        }

        private class Edge
        {
            public TNode Source { get; private set; }
            public TNode Target { get; private set; }

            internal Edge(TNode source, TNode target)
            {
                Source = source;
                Target = target;
            }
        }
    }
}
