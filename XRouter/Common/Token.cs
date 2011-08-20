using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.Utils;

namespace XRouter.Common
{
    /// <summary>
    /// Tokens represents a serializable container for a message, its metadata
    /// and transformed versions of the message as it is processed in XRouter
    /// pipeline. Access to is thread-safe and can be blocking. It is uniquely
    /// identified by a GUID.
    /// </summary>
    /// <remarks>
    /// As a message is processed its content can be transformed and each
    /// version is stored in its token.
    /// </remarks>
    [Serializable]
    [DataContract]
    public class Token
    {
        /// <summary>
        /// A unique identifier of a token.
        /// </summary>
        [DataMember]
        public Guid Guid { get; private set; }

        private object _syncLock = new object();
        private object SyncLock {
            get {
                if (_syncLock == null) {
                    _syncLock = new object();
                }
                return _syncLock;
            }
        }

        /// <summary>
        /// XML content of the message.
        /// </summary>
        [DataMember]
        public SerializableXDocument Content { get; private set; }

        /// <summary>
        /// State of the token processing.
        /// </summary>
        public TokenState State {
            get {
                lock (SyncLock) {
                    TokenState result;
                    Enum.TryParse<TokenState>(GetTokenAttribute("state"), out result);
                    return result;
                }
            } set {
                lock (SyncLock) {
                    SetTokenAttribute("state", value.ToString());
                }
            }
        }

        public bool IsPersistent {
            get {
                lock (SyncLock) {
                    return bool.Parse(GetTokenAttribute("is-persistent"));
                }
            }
            set {
                lock (SyncLock) {
                    SetTokenAttribute("is-persistent", value.ToString());
                }
            }
        }

        public DateTime Timeout {
            get {
                lock (SyncLock) {
                    return DateTime.Parse(GetTokenAttribute("timeout"), CultureInfo.InvariantCulture);
                }
            } set {
                lock (SyncLock) {
                    SetTokenAttribute("timeout", value.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public Token()
            : this(Guid.NewGuid(), @"
<token guid='' is-persistent='false'>
    <source-address>
    </source-address>
    <source-metadata>
    </source-metadata>
    <messageflow-state>
    </messageflow-state>
    <messages>
    </messages>
    <exceptions>
    </exceptions>
</token>
")
        {
        }

        public Token(string contentXml)
        {
            Content = new SerializableXDocument(XDocument.Parse(contentXml));
            Guid = Guid.Parse(Content.XDocument.Root.Attribute(XName.Get("guid")).Value);
        }

        public Token(Guid guid, string contentXml)
        {
            Guid = guid;
            Content = new SerializableXDocument(XDocument.Parse(contentXml));
            Content.XDocument.Root.SetAttributeValue(XName.Get("guid"), guid.ToString());
        }

        public Token Clone()
        {
            lock (SyncLock) {
                Token result = new Token();
                result.Guid = Guid;
                result.Content = Content;
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>may return null</returns>
        public EndpointAddress GetSourceAddress()
        {
            lock (SyncLock) {
                var xSourceAddress = Content.XDocument.XPathSelectElement("token/source-address");
                if (xSourceAddress != null) {
                    string gateway = xSourceAddress.Attribute(XName.Get("gateway")).Value;
                    string adapter = xSourceAddress.Attribute(XName.Get("adapter")).Value;
                    string endpoint = xSourceAddress.Attribute(XName.Get("endpoint")).Value;
                    return new EndpointAddress(gateway, adapter, endpoint);
                } else {
                    return null;
                }
            }
        }

        public void SetSourceAddress(EndpointAddress sourceAddress)
        {
            lock (SyncLock) {
                var xSourceAddress = Content.XDocument.XPathSelectElement("token/source-address");
                if (xSourceAddress == null) {
                    xSourceAddress = new XElement("source-address");
                    Content.XDocument.XPathSelectElement("token").Add(xSourceAddress);
                }
                xSourceAddress.SetAttributeValue("gateway", sourceAddress.GatewayName);
                xSourceAddress.SetAttributeValue("adapter", sourceAddress.AdapterName);
                xSourceAddress.SetAttributeValue("endpoint", sourceAddress.EndpointName);
            }
        }

        public MessageFlowState GetMessageFlowState()
        {
            lock (SyncLock) {
                XElement xWorkFlowState = Content.XDocument.XPathSelectElement("token/messageflow-state");
                MessageFlowState result = XSerializer.Deserialize<MessageFlowState>(xWorkFlowState);
                if (result == null) {
                    result = new MessageFlowState();
                }
                return result;
            }
        }

        public void UpdateMessageFlowState(Action<MessageFlowState> updater)
        {
            MessageFlowState messageFlowState = GetMessageFlowState();
            updater(messageFlowState);
            SetMessageFlowState(messageFlowState);
        }

        public void SetMessageFlowState(MessageFlowState messageFlowState)
        {
            lock (SyncLock) {
                XElement xWorkFlowState = Content.XDocument.XPathSelectElement("token/messageflow-state");
                XSerializer.Serializer(messageFlowState, xWorkFlowState);
            }
        }

        public void SaveSourceMetadata(XDocument sourceMetadata)
        {
            if (sourceMetadata == null) {
                return;
            }
            lock (SyncLock) {
                XElement sourceMetadataElement = Content.XDocument.XPathSelectElement("token/source-metadata");
                sourceMetadataElement.Add(sourceMetadata.Root);
            }
        }

        public XDocument GetSourceMetadata()
        {
            lock (SyncLock) {
                XElement sourceMetadataElement = Content.XDocument.XPathSelectElement("token/source-metadata");
                XDocument result = new XDocument();
                result.Add(sourceMetadataElement);
                return result;
            }
        }

        public void AddMessage(string name, XDocument message)
        {
            lock (SyncLock) {
                var xMessages = Content.XDocument.XPathSelectElement("token/messages");
                XElement xMessage = new XElement(XName.Get("message"), message.Root);
                xMessage.SetAttributeValue(XName.Get("name"), name);
                xMessages.Add(xMessage);
            }
        }

        public void AddException(string sourceNodeName, string message, string stackTrace)
        {
            lock (SyncLock) {
                var xExceptions = Content.XDocument.XPathSelectElement("token/exceptions");
                XElement xException = new XElement(XName.Get("exception"));
                xException.SetAttributeValue(XName.Get("source-node"), sourceNodeName);
                xException.SetAttributeValue(XName.Get("message"), message);
                xException.SetAttributeValue(XName.Get("stack-trace"), stackTrace);
                xExceptions.Add(xException);
            }
        }

        public XElement GetMessage(string name)
        {
            lock (SyncLock) {
                var message = Content.XDocument.XPathSelectElement("token/messages/message[@name=]" + name);
                return message;
            }
        }

        public void UpdateContent(XDocument content)
        {
            lock (SyncLock)
            {
                Content = new SerializableXDocument(content);
            }
        }

        private string GetTokenAttribute(string name)
        {
            lock (SyncLock) {
                var tokenAttribute = Content.XDocument.XPathSelectElement("token").Attribute(XName.Get(name));
                if (tokenAttribute != null) {
                    return tokenAttribute.Value;
                } else {
                    return null;
                }
            }
        }

        private void SetTokenAttribute(string name, string value)
        {
            lock (SyncLock) {
                Content.XDocument.XPathSelectElement("token").SetAttributeValue(name, value);
            }
        }

        //public void SetInputAdapterVariable(string name, string value)
        //{
        //    var inputAdapterVariables = Content.XDocument.XPathSelectElement("token/metadata/source-adapter-variables");
        //    var inputAdapterVariable = inputAdapterVariables.XPathSelectElement("variable[@name=]" + name);
        //    if (inputAdapterVariable == null) {
        //        inputAdapterVariable = new XElement("variable");
        //        inputAdapterVariable.SetAttributeValue("name", name);
        //        inputAdapterVariables.Add(inputAdapterVariable);
        //    }
        //    inputAdapterVariable.SetAttributeValue("value", value);
        //}

        //public string GetInputAdapterVariable(string name)
        //{
        //    var inputAdapterVariable = Content.XDocument.XPathSelectElement("token/metadata/source-adapter-variables/variable[@name=]" + name);
        //    return inputAdapterVariable.Attribute(XName.Get("value")).Value;
        //}

        //private string GetMetadataAttribute(string name)
        //{
        //    var metedataAttribute = Content.XDocument.XPathSelectElement("token/metadata").Attribute(XName.Get(name));
        //    if (metedataAttribute != null) {
        //        return metedataAttribute.Value;
        //    } else {
        //        return null;
        //    }
        //}

        //private void SetMetadataAttribute(string name, string value)
        //{
        //    Content.XDocument.XPathSelectElement("token/metadata").SetAttributeValue(name, value);
        //}
    }
}
