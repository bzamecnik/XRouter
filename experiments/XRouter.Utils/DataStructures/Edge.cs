using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.DataStructures
{
    public class Edge<TNode>
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
