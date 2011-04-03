using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchemaTron.Preprocessing
{
    /// <summary>
    /// Represents a parameter reference.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal class Parameter
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}