using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer
{
    interface IInternalGraphPresenter
    {
        event Action GraphChanged;

        IEnumerable<object> GetNodes();
        IEnumerable<object> GetEdges();
        IInternalNodePresenter CreateNodePresenter(object node);
        IInternalEdgePresenter CreateEdgePresenter(object edge);
    }
}
