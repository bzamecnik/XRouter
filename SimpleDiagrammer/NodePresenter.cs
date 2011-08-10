using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleDiagrammer
{
    public abstract class NodePresenter<TNode> : IInternalNodePresenter
    {
        public TNode Node { get; private set; }

        public object Content { get; protected set; }

        public virtual Point Location { get; set; }

        public virtual FrameworkElement DragArea { 
            get { return null; } 
        }

        public virtual int ZIndex { 
            get { return 1; }
        }

        protected NodePresenter(TNode node)
        {
            Node = node;
            Content = (object)node ?? "<null>";
        }

        object IInternalNodePresenter.Node {
            get { return Node; }
        }

        int IInternalNodePresenter.ZIndex {
            get { return ZIndex; }
        }
    }
}
