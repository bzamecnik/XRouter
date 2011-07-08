using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer
{
    public abstract class GraphPresenter<TNode, TEdge> : IInternalGraphPresenter
    {
        public event Action NodesChanged = delegate { };
        public event Action EdgesChanged = delegate { };

        public GraphCanvas CreateGraphCanvas()
        {
            GraphCanvas result = new GraphCanvas(this);
            return result;
        }

        protected void RaiseNodesChanged()
        {
            NodesChanged();
        }

        protected void RaiseEdgesChanged()
        {
            EdgesChanged();
        }

        public abstract IEnumerable<TNode> GetNodes();
        public abstract IEnumerable<TEdge> GetEdges();
        public abstract NodePresenter<TNode> CreateNodePresenter(TNode node);
        public abstract EdgePresenter<TNode, TEdge> CreateEdgePresenter(TEdge edge);

        IEnumerable<object> IInternalGraphPresenter.GetNodes()
        {
            return (IEnumerable<object>)GetNodes();
        }

        IEnumerable<object> IInternalGraphPresenter.GetEdges()
        {
            return (IEnumerable<object>)GetEdges();
        }

        IInternalNodePresenter IInternalGraphPresenter.CreateNodePresenter(object node)
        {
            return CreateNodePresenter((TNode)node);
        }

        IInternalEdgePresenter IInternalGraphPresenter.CreateEdgePresenter(object edge)
        {
            return CreateEdgePresenter((TEdge)edge);
        }
    }
}
