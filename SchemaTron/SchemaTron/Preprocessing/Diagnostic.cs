using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchemaTron.Preprocessing
{
    internal sealed class Diagnostic
    {
        public string Id { get; set; }
      
        public XElement Element { get; set; }
    }
}
