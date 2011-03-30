using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace SchemaTron.SyntaxModel
{
    internal sealed class Assert
    {
        public String Id { set; get; }

        public Boolean IsReport { set; get; }

        public String Test { set; get; }
             
        public XPathExpression CompiledTest { set; get; }

        public String Message { set; get; }
       
        public Boolean[] DiagnosticsIsValueOf { set; get; }

        public String[] Diagnostics { set; get; }

        public XPathExpression[] CompiledDiagnostics { set; get; }        
    }
}
