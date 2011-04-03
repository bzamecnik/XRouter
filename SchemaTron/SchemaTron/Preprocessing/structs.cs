using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SchemaTron.Preprocessing
{
    /// <summary>
    /// Represents an abstract rule.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal class AbstractRule
    {
        public String Id { set; get; }
        public XElement Element { set; get; }
    }

    /// <summary>
    /// Represents a parameter reference.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal class Parameter
    {
        public String Name { set; get; }
        public String Value { set; get; }
    }

    /// <summary>
    /// Represents a variable substitution.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal class Let
    {
        public String Name { set; get; }
        public String Value { set; get; }
    }
}
