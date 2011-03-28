using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XRouter.Prototype.Processor
{
    class Token
    {
        public Int32 Step { set; get; }

        public XDocument Content { set; get; }
    }
}
