﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer.Layouts
{
    abstract class LayoutAlgorithm
    {
        public abstract void UpdateLayout(IEnumerable<Node> nodes);
    }
}
