using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents an assertion.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal sealed class Assert
    {
        public string Id { get; set; }

        public bool IsReport { get; set; }

        public string Test { get; set; }

        public XPathExpression CompiledTest { get; set; }

        public string Message { get; set; }

        // TODO: use IEnumerable<T> or IList<T> instead of arrays

        public bool[] DiagnosticsIsValueOf { get; set; }

        public string[] Diagnostics { get; set; }

        public XPathExpression[] CompiledDiagnostics { get; set; }
    }
}
