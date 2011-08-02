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
    internal sealed class Parameter
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}