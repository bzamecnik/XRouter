using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDiagrammer.Tests.Test1
{
    class Node
    {
        public object Content { get; set; }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}
