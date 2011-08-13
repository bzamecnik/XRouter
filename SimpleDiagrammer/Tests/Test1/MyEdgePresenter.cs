using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer.Tests.Test1
{
    class MyEdgePresenter : EdgePresenter<Node, Edge>
    {
        public MyEdgePresenter(Edge edge)
            : base(edge, edge.Source, edge.Target)
        {
        }
    }
}
