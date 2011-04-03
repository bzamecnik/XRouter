using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents a pattern.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal sealed class Pattern
    {
        public String Id { set; get; }

        public Rule[] Rules { set; get; }
    }
}
