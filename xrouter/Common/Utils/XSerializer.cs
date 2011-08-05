using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XRouter.Common.Utils
{
    /// <summary>
    /// Serializer and deserializer of object graphs which can store the
    /// serialized from as XML.
    /// </summary>
    /// <remarks>
    /// It acts as a wrapper around DataContractSerializer which adds storing
    /// the serialized XML form into an XElement.
    /// </remarks>
    public class XSerializer
    {
        /// <summary>
        /// Serializes the provided object into XML which is then stored as
        /// the only child element of the target container XML element.
        /// </summary>
        /// <remarks>
        /// The serialized object must conform any contraints given by the
        /// DataContractSerializer. In particular its class must be marked
        /// using the [DataContract] attribute and its persistent members
        /// should be marked with the [DataMember] attribute.
        /// </remarks>
        /// <typeparam name="T">type of the serialized object</typeparam>
        /// <param name="obj">object to be serialized</param>
        /// <param name="xTargetContainer">prepared target XML container
        /// where the result should be placed</param>
        public static void Serializer<T>(T obj, XElement xTargetContainer)
        {
            DataContractSerializer serializer = new DataContractSerializer(
                typeof(T), null, int.MaxValue, false, true, null);
            StringBuilder output = new StringBuilder();
            using (var writer = XmlDictionaryWriter.Create(output, new XmlWriterSettings { }))
            {
                serializer.WriteObject(writer, obj);
            }
            XElement xContent = XElement.Parse(output.ToString());

            foreach (var childElement in xTargetContainer.Elements())
            {
                childElement.Remove();
            }
            xTargetContainer.Add(xContent);
        }

        /// <summary>
        /// Deserializes a previously serialized object graph from its XML
        /// representation stored in an XML element.
        /// </summary>
        /// <remarks>
        /// If the target container does not contain any children the default
        /// value of the type is returned.
        /// </remarks>
        /// <typeparam name="T">type of the serialized object</typeparam>
        /// <param name="xTargetContainer">XML element whose first child
        /// should contain the XML representation of the object to be
        /// deserialized</param>
        /// <returns>deserialized object with its references</returns>
        public static T Deserialize<T>(XElement xTargetContainer)
        {
            XElement xContent = xTargetContainer.Elements().FirstOrDefault();
            if (xContent == null)
            {
                return default(T);
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (var reader = xContent.CreateReader())
            {
                object obj = serializer.ReadObject(reader);
                return (T)obj;
            }
        }
    }
}
