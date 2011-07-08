using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer
{
    public abstract class EdgePresenter<TNode, TEdge> : IInternalEdgePresenter
    {
        public TEdge Edge { get; private set; }
        public TNode Source { get; private set; }
        public TNode Target { get; private set; }

        protected EdgePresenter(TEdge edge, TNode source, TNode target)
        {
            Edge = edge;
            Source = source;
            Target = target;
        }

        object IInternalEdgePresenter.Edge { get { return Edge; } }
        object IInternalEdgePresenter.Source { get { return Source; } }
        object IInternalEdgePresenter.Target { get { return Target; } }
    }
}
