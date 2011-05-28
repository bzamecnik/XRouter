using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace XRouter.Common
{
    [DataContract]
    public class SerializableXDocument
    {
        [DataMember]
        private string XmlContent {
            get { return XDocument.ToString(); }
            set { XDocument = XDocument.Parse(value); }
        }

        public XDocument XDocument { get; private set; }

        public static implicit operator XDocument(SerializableXDocument serializableXDocument)
        {
            if (serializableXDocument == null) {
                return null;
            }
            return serializableXDocument.XDocument;
        }

        public SerializableXDocument(XDocument xdocument)
        {
            if (xdocument == null) {
                xdocument = new XDocument();
            }
            XDocument = xdocument;
        }

        public override string ToString()
        {
            return XDocument.ToString();
        }
    }
}
