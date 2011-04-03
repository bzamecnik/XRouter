﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SchemaTron.SyntaxModel
{
    /// <summary>
    /// Represents a rule.
    /// </summary>
    /// <remarks>
    /// TODO: describe in more detail.
    /// </remarks>
    internal sealed class Rule
    {
        public string Id { get; set; }

        public string Context { get; set; }

        public XPathExpression CompiledContext { get; set; }

        public Assert[] Asserts { get; set; }
    }
}
