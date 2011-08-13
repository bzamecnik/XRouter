using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer
{
    interface IInternalEdgePresenter
    {
        object Edge { get; }
        object Source { get; }
        object Target { get; }

        int ZIndex { get; }
    }
}
