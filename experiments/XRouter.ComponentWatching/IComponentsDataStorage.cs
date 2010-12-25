using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XRouter.ComponentWatching
{
    public interface IComponentsDataStorage
    {        
        Point GetLocation(object componentObject);
        void SetLocation(object componentObject, Point location);
    }
}
