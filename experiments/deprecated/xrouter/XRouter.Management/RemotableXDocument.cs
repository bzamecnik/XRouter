using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using XRouter.Remoting;

namespace XRouter.Management
{
    public class RemotableXDocument : IRemotelyCloneable
    {
        public XDocument XDocument { get; private set; }

        public RemotableXDocument(XDocument xdocument)
        {
            XDocument = xdocument;
        }

        public RemotableXDocument(string serialized)
        {
            DeserializeClone(serialized);
        }

        public string SerializeClone()
        {
            string result;
            using (var ms = new MemoryStream()) {
                XDocument.Save(ms);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms)) {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        public void DeserializeClone(string serialized)
        {
            XDocument = XDocument.Parse(serialized);
        }
    }
}
