using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchematronVal.SyntaxModel
{
    internal sealed class Pattern 
    {                     
        public String Id { set; get; }

        public Rule[] Rules { set; get; }       
    }
}
