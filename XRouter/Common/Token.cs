using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;

namespace XRouter.Common
{
    [Serializable]
    public class Token
    {
        public Guid Guid { get; private set; }

        private object syncLock = new object();

        public SerializableXDocument Content { get; private set; }

        private MessageFlowState _messageFlowState;
        public MessageFlowState MessageFlowState {
            get {
                lock (syncLock) {
                    if (_messageFlowState == null) {
                        XElement workFlowStateElement = Content.XDocument.XPathSelectElement("token/messageflow-state");
                        XmlSerializer serializer = new XmlSerializer(typeof(MessageFlowState));
                        XmlReader xmlReader = workFlowStateElement.CreateReader();
                        _messageFlowState = (MessageFlowState)serializer.Deserialize(xmlReader);
                    }
                    return _messageFlowState;
                }

                //var workFlowStateElement = Content.XDocument.XPathSelectElement("token/messageflow-state");
                //if (workFlowStateElement == null) {
                //    return null;
                //}
                //MessageFlowState workflowState = new MessageFlowState();
                //workflowState.MessageFlowGuid = Guid.Parse(workFlowStateElement.Attribute(XName.Get("messageflow-guid")).Value);
                //workflowState.AssignedProcessor = workFlowStateElement.Attribute(XName.Get("assigned-processor")).Value;
                //workflowState.LastResponseFromProcessor = DateTime.Parse(workFlowStateElement.Attribute(XName.Get("last-response-from-processor")).Value);
                //workflowState.NextNodeName = workFlowStateElement.Attribute(XName.Get("next-node-name")).Value;
                //return workflowState;
            }
        }

        public TokenState State {
            get {
                lock (syncLock) {
                    TokenState result;
                    Enum.TryParse<TokenState>(GetTokenAttribute("state"), out result);
                    return result;
                }
            } set {
                lock (syncLock) {
                    SetTokenAttribute("state", value.ToString());
                }
            }
        }

        public DateTime Timeout {
            get {
                lock (syncLock) {
                    return DateTime.Parse(GetTokenAttribute("timeout"), CultureInfo.InvariantCulture);
                }
            } set {
                lock (syncLock) {
                    SetTokenAttribute("timeout", value.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private EndpointAddress _sourceAddress;
        public EndpointAddress SourceAddress {
            get {
                lock (syncLock) {
                    if (_sourceAddress == null) {
                        var sourceAddressElement = Content.XDocument.XPathSelectElement("token/source-address");
                        if (sourceAddressElement != null) {
                            string gateway = sourceAddressElement.Attribute(XName.Get("gateway")).Value;
                            string adapter = sourceAddressElement.Attribute(XName.Get("adapter")).Value;
                            string endpoint = sourceAddressElement.Attribute(XName.Get("endpoint")).Value;
                            _sourceAddress = new EndpointAddress(gateway, adapter, endpoint);
                        } else {
                            _sourceAddress = new EndpointAddress(string.Empty, string.Empty, string.Empty);
                        }
                    }
                    return _sourceAddress;
                }
            }
        }

        public Token()
        {
            Guid = Guid.NewGuid();
            Content = new SerializableXDocument(XDocument.Parse(@"
<token>
    <source-address>
    </source-address>
    <source-metadata>
    </source-metadata>
    <messageflow-state>
    </messageflow-state>
    <messages>
    </messages>
</token>
"));
        }

        public Token Clone()
        {
            lock (syncLock) {
                Token result = new Token();
                result.Guid = Guid;
                result.Content = Content;
                return result;
            }
        }

        public void SaveMessageFlowState()
        {
            SaveMessageFlowState(MessageFlowState);
        }

        public void SaveMessageFlowState(MessageFlowState messageFlowState)
        {
            lock (syncLock) {
                XElement workFlowStateElement = Content.XDocument.XPathSelectElement("token/messageflow-state");
                XmlSerializer serializer = new XmlSerializer(typeof(MessageFlowState));
                XmlWriter xmlWriter = workFlowStateElement.CreateWriter();
                serializer.Serialize(xmlWriter, messageFlowState);
            }
        }

        public void SaveSourceAddress()
        {
            SaveSourceAddress(SourceAddress);
        }

        public void SaveSourceAddress(EndpointAddress sourceAddress)
        {
            lock (syncLock) {
                var sourceAddressElement = Content.XDocument.XPathSelectElement("token/source-address");
                if (sourceAddressElement == null) {
                    sourceAddressElement = new XElement("source-address");
                    Content.XDocument.XPathSelectElement("token").Add(sourceAddressElement);
                }
                sourceAddressElement.SetAttributeValue("gateway", sourceAddress.GatewayName);
                sourceAddressElement.SetAttributeValue("adapter", sourceAddress.AdapterName);
                sourceAddressElement.SetAttributeValue("endpoint", sourceAddress.EndPointName);
            }
        }

        public void SaveSourceMetadata(XDocument sourceMetadata)
        {
            lock (syncLock) {
                if (sourceMetadata == null) {
                    return;
                }

                XElement sourceMetadataElement = Content.XDocument.XPathSelectElement("token/source-metadata");
                sourceMetadataElement.Add(sourceMetadata.Root);
            }
        }

        public XDocument GetSourceMetadata()
        {
            lock (syncLock) {
                XElement sourceMetadataElement = Content.XDocument.XPathSelectElement("token/source-metadata");
                XDocument result = new XDocument();
                result.Add(sourceMetadataElement);
                return result;
            }
        }

        public void AddMessage(string name, XDocument message)
        {
            lock (syncLock) {
                var messagesElement = Content.XDocument.XPathSelectElement("token/messages");
                XElement messageElement = new XElement(XName.Get("message"), message);
                messageElement.SetAttributeValue(XName.Get("name"), name);
            }
        }

        public XElement GetMessage(string name)
        {
            lock (syncLock) {
                var message = Content.XDocument.XPathSelectElement("token/messages/message[@name=]" + name);
                return message;
            }
        }

        private string GetTokenAttribute(string name)
        {
            lock (syncLock) {
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
            lock (syncLock) {
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
