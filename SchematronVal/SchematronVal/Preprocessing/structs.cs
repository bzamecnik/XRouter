using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchematronVal.Preprocessing
{   
    internal class AbstractRule
    {
        public String Id { set; get; }
        public XElement Element { set; get; }
    }
   
    internal class Param
    {
        public String Name { set; get; }
        public String Value { set; get; }
    }

    internal class Let
    {
        public String Name { set; get; }
        public String Value { set; get; }       
    }
}
