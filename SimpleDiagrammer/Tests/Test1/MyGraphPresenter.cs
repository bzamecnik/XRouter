using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer.Tests.Test1
{
    class MyGraphPresenter : GraphPresenter<Node, Edge>
    {
        private static readonly int NodesCount = 20;
        private static readonly int EdgesCount = 15;

        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();

        public MyGraphPresenter()
        {
            //Node node1 = new Node() { Content = 1 };
            //Node node2 = new Node() { Content = 2 };
            //nodes.Add(node1);
            //nodes.Add(node2);
            //edges.Add(new Edge() { Source = node1, Target = node2 });
            //return;

            Random rnd = new Random();

            for (int i = 0; i < NodesCount; i++) {
                Node node = new Node();
                node.Content = i;
                nodes.Add(node);
            }

            for (int i = 0; i < EdgesCount; i++) {
                Node source = nodes[rnd.Next(nodes.Count)];
                Node target = nodes[rnd.Next(nodes.Count)];
                if (source != target) {
                    bool alreadyExists = edges.Any(e => (e.Source == source) && (e.Target == target));
                    if (!alreadyExists) {
                        Edge edge = new Edge();
                        edge.Source = source;
                        edge.Target = target;
                        edges.Add(edge);
                    }
                }
            }
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
            RaiseNodesChanged();
        }

        public void RemoveNode(Node node)
        {
            edges.RemoveAll(e => e.Source == node || e.Target == node);
            RaiseEdgesChanged();

            nodes.Remove(node);
            RaiseNodesChanged();
        }

        public void AddEdge(Node source, Node target)
        {
            Edge edge = new Edge();
            edge.Source = source;
            edge.Target = target;
            edges.Add(edge);
            RaiseEdgesChanged();
        }

        public override IEnumerable<Node> GetNodes()
        {
            return nodes;
        }

        public override IEnumerable<Edge> GetEdges()
        {
            return edges;
        }

        public override NodePresenter<Node> CreateNodePresenter(Node node)
        {
            return new MyNodePresenter(node, this);
        }

        public override EdgePresenter<Node, Edge> CreateEdgePresenter(Edge edge)
        {
            return new MyEdgePresenter(edge);
        }
    }
}
