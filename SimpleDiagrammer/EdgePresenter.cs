using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer
{
    /// <summary>
    /// Description of an edge to be used by GraphCanvas to show its graphical representation.
    /// </summary>
    /// <typeparam name="TNode">Type of real node objects.</typeparam>
    /// <typeparam name="TEdge">Type of real edge objects.</typeparam>
    public abstract class EdgePresenter<TNode, TEdge> : IInternalEdgePresenter
    {
        /// <summary>
        /// A real instance of a described edge.
        /// </summary>
        public TEdge Edge { get; private set; }

        /// <summary>
        /// A real instance of source node for the edge.
        /// </summary>
        public TNode Source { get; private set; }

        /// <summary>
        /// A real instance of target node for the edge.
        /// </summary>
        public TNode Target { get; private set; }

        /// <summary>
        /// ZIndex on which the edge will be displayed.
        /// </summary>
        public virtual int ZIndex {
            get { return 0; }
        }

        protected EdgePresenter(TEdge edge, TNode source, TNode target)
        {
            Edge = edge;
            Source = source;
            Target = target;
        }

        object IInternalEdgePresenter.Edge { get { return Edge; } }
        object IInternalEdgePresenter.Source { get { return Source; } }
        object IInternalEdgePresenter.Target { get { return Target; } }

        int IInternalEdgePresenter.ZIndex {
            get { return ZIndex; }
        }
    }
}
