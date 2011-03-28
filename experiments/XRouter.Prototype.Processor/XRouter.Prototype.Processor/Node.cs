using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    class Node
    {
        public Int32 Id { set; get; }

        public String Name { set; get; }
        
        public INodeFunction Function { set; get; }
    }
}
