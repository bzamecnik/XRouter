using System.Runtime.Serialization;
using System.Xml.Linq;

namespace XRouter.Common
{
    /// <summary>
    /// Provides a wrapper over XDocument which can be (de)serialized using
    /// the DataContractSerializer class.
    /// </summary>
    /// <see cref="System.Runtime.Serialization.DataContractSerializer"/>
    /// <see cref="System.Xml.Linq.XDocument"/>
    [DataContract]
    public class SerializableXDocument
    {
        [DataMember]
        private string XmlContent
        {
            get { return XDocument.ToString(); }
            set { XDocument = XDocument.Parse(value); }
        }

        public XDocument XDocument { get; private set; }

        public static implicit operator XDocument(SerializableXDocument serializableXDocument)
        {
            if (serializableXDocument == null)
            {
                return null;
            }
            return serializableXDocument.XDocument;
        }

        public SerializableXDocument(XDocument xdocument)
        {
            if (xdocument == null)
            {
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
