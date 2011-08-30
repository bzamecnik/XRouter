using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer
{
    /// <summary>
    /// Description of a graph to be used by GraphCanvas to show its graphical representation.
    /// </summary>
    /// <typeparam name="TNode">Type of real node objects.</typeparam>
    /// <typeparam name="TEdge">Type of real edge objects.</typeparam>
    public abstract class GraphPresenter<TNode, TEdge> : IInternalGraphPresenter
    {
        /// <summary>
        /// Raised when a real graph is changed.
        /// </summary>
        public event Action GraphChanged = delegate { };

        /// <summary>
        /// Creates a GraphCanvas for graph represented by this instance.
        /// </summary>
        /// <returns></returns>
        public GraphCanvas CreateGraphCanvas()
        {
            GraphCanvas result = new GraphCanvas(this);
            return result;
        }

        protected void RaiseGraphChanged()
        {
            GraphChanged();
        }

        /// <summary>
        /// Provides objects of real nodes.
        /// </summary>
        /// <returns>Real nodes.</returns>
        public abstract IEnumerable<TNode> GetNodes();

        /// <summary>
        /// Provides objects of real edges.
        /// </summary>
        /// <returns>Real edges</returns>
        public abstract IEnumerable<TEdge> GetEdges();

        /// <summary>
        /// Create description of a real node.
        /// </summary>
        /// <param name="node">A real node.</param>
        /// <returns>A real node description.</returns>
        public abstract NodePresenter<TNode> CreateNodePresenter(TNode node);

        /// <summary>
        /// Create description of a real edge.
        /// </summary>
        /// <param name="edge">A real edge.</param>
        /// <returns>A real edge description.</returns>
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
