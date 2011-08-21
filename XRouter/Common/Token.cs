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
        /// <summary>
        /// Lock for exclusive access to the token data.
        /// </summary>
        private object SyncLock {
            get {
                if (_syncLock == null) {
                    _syncLock = new object();
                }
                return _syncLock;
            }
        }

        /// <summary>
        /// XML content of the token, including the original message and
        /// subsequent transformed and possibly output messages; token state
        /// and other useful information related to token processing.
        /// </summary>
        /// <remarks>This property is not synchronized.</remarks>
        [DataMember]
        public SerializableXDocument Content { get; private set; }

        /// <summary>
        /// State of the token processing.
        /// </summary>
        /// <remarks>This property is synchronized and can block.</remarks>
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

        /// <summary>
        /// Indicates whether the message flow state of the token should be
        /// persisted after performing each step of the processing according
        /// to the message flow associated with the token.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Consider that the processing of a token is interrupted. For
        /// (IsPersistent == false) this should result in starting the
        /// message flow from the beginning, while for (IsPersistent == false)
        /// processing should continue from the last node stored in the
        /// message flow state.
        /// </para>
        /// <para>
        /// This property is synchronized and can block.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This property is synchronized and can block.</remarks>
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

        /// <summary>
        /// Creates a new instance of Token with a prepared blank content.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of Token from given content including a
        /// GUID.
        /// </summary>
        /// <param name="contentXml">initial token content</param>
        public Token(string contentXml)
        {
            Content = new SerializableXDocument(XDocument.Parse(contentXml));
            Guid = Guid.Parse(Content.XDocument.Root.Attribute(XName.Get("guid")).Value);
        }

        /// <summary>
        /// Creates a new instance of Token from given content and GUID.
        /// </summary>
        /// <param name="guid">token GUID; replaces any GUID in contentXml</param>
        /// <param name="contentXml">initial token content</param>
        public Token(Guid guid, string contentXml)
        {
            Guid = guid;
            Content = new SerializableXDocument(XDocument.Parse(contentXml));
            Content.XDocument.Root.SetAttributeValue(XName.Get("guid"), guid.ToString());
        }

        // TODO: Clone() is never used and also can't be successfully used
        // in the Processor component -> better remove it

        /// <summary>
        /// Creates a new instance of Token by cloning this instance.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <returns>new token instance with the same</returns>
        public Token Clone()
        {
            lock (SyncLock) {
                Token result = new Token();
                // TODO: make sure that Guid and Content are also cloned!
                result.Guid = Guid;
                result.Content = Content;
                return result;
            }
        }

        /// <summary>
        /// Obtains the address of the source endpoint which the input message
        /// originated from.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
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

        /// <summary>
        /// Obtains the address of source endpoint which the input message
        /// originated from.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <paparam name="sourceAddress">Address of the original
        /// gateway/adapter/endpoint</paparam>
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

        /// <summary>
        /// Obtains the state of the message flow processing of this token.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <returns>existing message flow state; or a new instance if there
        /// is no message flow state stored; never null</returns>
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

        /// <summary>
        /// Updates the state of the message flow processing of this token
        /// with a provided action.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="updater">action which updates the current message
        /// flow state</param>
        public void UpdateMessageFlowState(Action<MessageFlowState> updater)
        {
            MessageFlowState messageFlowState = GetMessageFlowState();
            updater(messageFlowState);
            SetMessageFlowState(messageFlowState);
        }

        /// <summary>
        /// Sets the new state of the message flow processing of this token.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="messageFlowState">new message flow state</param>
        public void SetMessageFlowState(MessageFlowState messageFlowState)
        {
            lock (SyncLock) {
                XElement xWorkFlowState = Content.XDocument.XPathSelectElement("token/messageflow-state");
                XSerializer.Serializer(messageFlowState, xWorkFlowState);
            }
        }

        /// <summary>
        /// Obtains the metadata about the token source.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <returns>source metadata; never null</returns>
        public XDocument GetSourceMetadata()
        {
            lock (SyncLock)
            {
                XElement sourceMetadataElement = Content.XDocument.XPathSelectElement("token/source-metadata");
                XDocument result = new XDocument();
                result.Add(sourceMetadataElement);
                return result;
            }
        }

        // TODO: better rename to AddSourceMetadata() or change the sematics
        // to replace the metadata and rename to SetSourceMetadata() for
        // consistency with the rest of code

        /// <summary>
        /// Set the metadata about the token source.
        /// </summary>
        /// <remarks>This method is synchronized and can block, except from
        /// when the sourceMetadata parameter is null.</remarks>
        /// <remarks>null is safely ignored</remarks>
        /// <param name="sourceMetadata">metadata about the token source;
        /// can be null</param>
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

        /// <summary>
        /// Obtains a message, specified by its name, stored in a token.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="name">name of the message</param>
        /// <returns>message contents; or null if there is no such a message
        /// </returns>
        public XElement GetMessage(string name)
        {
            lock (SyncLock)
            {
                var message = Content.XDocument.XPathSelectElement("token/messages/message[@name=]" + name);
                return message;
            }
        }

        /// <summary>
        /// Adds a new XML message to the token identified by its name.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="name">name of the new message</param>
        /// <param name="message">XML contents of the new message</param>
        public void AddMessage(string name, XDocument message)
        {
            lock (SyncLock) {
                var xMessages = Content.XDocument.XPathSelectElement("token/messages");
                XElement xMessage = new XElement(XName.Get("message"), message.Root);
                xMessage.SetAttributeValue(XName.Get("name"), name);
                // TODO: name should be unique!
                // now there could be added multiple messages with the name name!
                xMessages.Add(xMessage);
            }
        }

        /// <summary>
        /// Adds an exception to the token
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="sourceNodeName">name of the message flow node where
        /// the exception was thrown
        /// </param>
        /// <param name="message">exception message</param>
        /// <param name="stackTrace">exception stack trace</param>
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

        /// <summary>
        /// Replaces the whole XML content of the token with a new one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This can be useful for updating the token without
        /// creating a new instance.
        /// </para>
        /// <para>NOTE: It could be dangerous to replace the contents with
        /// arbitrary XML because it can damage the internal invariants of a
        /// token instance. Use with care!
        /// </para>
        /// <para>
        /// This method is synchronized and can block.
        /// </para>
        /// </remarks>
        /// <param name="content">new content of the token</param>
        public void UpdateContent(XDocument content)
        {
            lock (SyncLock)
            {
                Content = new SerializableXDocument(content);
            }
        }

        /// <summary>
        /// Obtains an attribute of the token specified by the attribute name.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="name">name of the token atrribute</param>
        /// <returns>token attribute value; or null if no such an attribute
        /// exists</returns>
        private string GetTokenAttribute(string name)
        {
            lock (SyncLock) {
                var tokenAttribute = Content.XDocument.XPathSelectElement("token").Attribute(XName.Get(name));
                return (tokenAttribute != null) ? tokenAttribute.Value : null;
            }
        }

        /// <summary>
        /// Replaces a token attribute specified by its name with a new value
        /// or creates a new attribute.
        /// </summary>
        /// <remarks>This method is synchronized and can block.</remarks>
        /// <param name="name">token attribute name</param>
        /// <param name="value">token attribute null</param>
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
