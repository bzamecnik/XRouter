using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;

namespace SchematronVal.SyntaxModel
{
    internal sealed class Rule 
    {                
        public String Id { set; get; }

        public String Context { set; get; }

        public XPathExpression CompiledContext { set; get; }
        
        public Assert[] Asserts { set; get; }       
    }
}
