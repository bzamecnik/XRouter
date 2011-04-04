using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents a namespace.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal sealed class Namespace
    {
        public string Prefix { get; set; }

        public string Uri { get; set; }
    }
}
