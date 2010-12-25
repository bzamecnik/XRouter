using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XRouter.ComponentWatching
{
    public interface IRepresentationProvider
    {
        FrameworkElement CreateRepresentation();
    }
}
