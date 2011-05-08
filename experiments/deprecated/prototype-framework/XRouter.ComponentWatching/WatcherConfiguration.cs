using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.ComponentWatching
{
    public class WatcherConfiguration
    {
        public bool AllObjectsAreComponents { get; set; }

        public bool HidePrimitiveTypes { get; set; }
        public bool HideValueTypes { get; set; }

        public Func<object, bool> CustomVisibilityFilter { get; set; }

        public WatcherConfiguration()
        {
        }
    }
}
