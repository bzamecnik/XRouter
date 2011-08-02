using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents a Schematron schema.
    /// </summary> 
    internal sealed class Schema
    {
        public IEnumerable<Namespace> Namespaces { get; set; }

        public IEnumerable<Pattern> Patterns { get; set; }
    }
}
