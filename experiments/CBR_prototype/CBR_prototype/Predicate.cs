using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CBR_prototype
{
    public class Predicate
    {
        [XmlAttribute("id")]
        public String Id = null;

        [XmlAttribute("schematron-path")]
        public String SchematronPath = null;
    }
}
