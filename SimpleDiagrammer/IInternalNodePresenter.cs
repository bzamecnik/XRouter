using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleDiagrammer
{
    interface IInternalNodePresenter
    {
        object Node { get;  }

        object Content { get; }

        FrameworkElement DragArea { get; }
    }
}
