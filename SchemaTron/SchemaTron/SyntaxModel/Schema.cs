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
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal sealed class Schema
    {
        public Namespace[] Namespaces { set; get; }

        public Pattern[] Patterns { set; get; }
    }
}
