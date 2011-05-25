using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace XRouter.Common
{
    [Serializable]
    public class SerializableXDocument : ISerializable
    {
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

        protected SerializableXDocument(SerializationInfo info, StreamingContext context)
        {
            string xml = info.GetString("xml");
            XDocument = XDocument.Parse(xml);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string xml = XDocument.ToString();
            info.AddValue("xml", xml);
        }

        public override string ToString()
        {
            return XDocument.ToString();
        }
    }
}
