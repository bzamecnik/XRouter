using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;

namespace XRouter.Common.Utils
{
    public class XSerializer
    {
        public static void Serializer<T>(T obj, XElement xTargetContainer)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, true, null);
            StringBuilder output = new StringBuilder();            
            using (var writer = XmlDictionaryWriter.Create(output, new XmlWriterSettings { })) {
                serializer.WriteObject(writer, obj);
            }
            XElement xContent = XElement.Parse(output.ToString());

            foreach (var childElement in xTargetContainer.Elements()) {
                childElement.Remove();
            }
            xTargetContainer.Add(xContent);
        }

        public static T Deserialize<T>(XElement xTargetContainer)
        {
            XElement xContent = xTargetContainer.Elements().FirstOrDefault();
            if (xContent == null) {
                return default(T);
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (var reader = xContent.CreateReader()) {
                object obj = serializer.ReadObject(reader);
                return (T)obj;
            }
        }
    }
}
