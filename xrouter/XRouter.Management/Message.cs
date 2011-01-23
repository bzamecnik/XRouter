using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Remoting;

namespace XRouter.Management
{
    public class Message : IRemotelyCloneable
    {
        private static readonly string SerializationSeparator = "[[/!)^4g*@t=w+_}]]";

        public IInputEndpoint Source { get; private set; }

        private RemotableXDocument RemotableContent { get; set; }

        public XDocument Content { get { return RemotableContent.XDocument; } }

        public Message(XDocument content, IInputEndpoint source)
        {
            RemotableContent = new RemotableXDocument(content);
            Source = source;
        }

        public string SerializeClone()
        {
            var result = new StringBuilder();
            
            RemoteObjectAddress sourceObjectAddress = ObjectServer.PublishObject(Source);
            result.Append(sourceObjectAddress.Serialize());

            result.Append(SerializationSeparator);
            result.Append(RemotableContent.SerializeClone());

            return result.ToString();
        }

        public void DeserializeClone(string serialized)
        {
            string[] parts = serialized.Split(new string[] { SerializationSeparator }, StringSplitOptions.None);
            
            RemoteObjectAddress sourceObjectAddress = RemoteObjectAddress.Deserialize(parts[0]);
            Source = RemoteObjectProxyProvider.GetProxy<IInputEndpoint>(sourceObjectAddress);

            RemotableContent = new RemotableXDocument(parts[1]);
        }
    }
}
