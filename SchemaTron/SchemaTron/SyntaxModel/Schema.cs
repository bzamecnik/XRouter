using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace SchemaTron.SyntaxModel
{       
    internal sealed class Schema 
    {                                                   
        public Ns[] Namespaces { set; get; }
                    
        public Pattern[] Patterns { set; get; }            
    }    
}
