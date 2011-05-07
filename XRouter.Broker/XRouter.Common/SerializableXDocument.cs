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
            return serializableXDocument.XDocument;
        }

        public SerializableXDocument(XDocument xdocument)
        {
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
