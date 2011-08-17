using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Text;

namespace Library
{
    public class Reader
    {
        public Guid Id { get; set; }
        public string  Name { get; set; }
        public string Email { get; set; }

        public static Reader FromXml(XElement xml)
        {
            Reader reader = new Reader();
            reader.Id = Guid.Parse(xml.Attribute("id").Value);
            reader.Name = xml.XPathSelectElement("/reader/name").Value;
            reader.Email = xml.XPathSelectElement("/reader/email").Value;
            return reader;
        }

        public XElement ToXml()
        {
            XElement xReader = new XElement("reader");
            xReader.SetAttributeValue("id", Id);
            xReader.Add(new XElement("name") { Value = Name });
            xReader.Add(new XElement("email") { Value = Email });
            return xReader;
        }
    }
}
