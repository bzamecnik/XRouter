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
        public string Id { get; set; }

        public XElement Element { get; set; }
    }
}