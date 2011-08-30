using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleDiagrammer
{
    /// <summary>
    /// Description of a node to be used by GraphCanvas to show its graphical representation.
    /// </summary>
    /// <typeparam name="TNode">Type of real node objects.</typeparam>
    public abstract class NodePresenter<TNode> : IInternalNodePresenter
    {
        /// <summary>
        /// A real instance of a described node.
        /// </summary>
        public TNode Node { get; private set; }

        /// <summary>
        /// WPF content to be displayed for this node.
        /// </summary>
        public object Content { get; protected set; }

        /// <summary>
        /// Logical location of the node.
        /// Point [0,0] is in the center.
        /// </summary>
        public virtual Point Location { get; set; }

        /// <summary>
        /// An WPF element which can be dragged and used to move node.
        /// </summary>
        public virtual FrameworkElement DragArea { 
            get { return null; } 
        }

        /// <summary>
        /// ZIndex on which the node will be displayed.
        /// </summary>
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
