using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchemaTron.Preprocessing
{
    /// <summary>
    /// Represents a variable substitution.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal sealed class Let
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
