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
            return null;
        }
    }
}
