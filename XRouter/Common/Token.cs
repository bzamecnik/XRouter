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

        public SerializableXDocument Content { get; private set; }

        private MessageFlowState _messageFlowState;
        public MessageFlowState MessageFlowState {
            get {
                if (_messageFlowState == null) {
                    XElement workFlowStateElement = Content.XDocument.XPathSelectElement("token/messageflow-state");
                    XmlSerializer serializer = new XmlSerializer(typeof(MessageFlowState));
                    XmlReader xmlReader = workFlowStateElement.CreateReader();
                    _messageFlowState = (MessageFlowState)serializer.Deserialize(xmlReader);
                }
                return _messageFlowState;

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

        public string SourceGatewayName
        {
            get
            {
                return GetTokenAttribute("source-gateway");
            }
            set
            {
                SetTokenAttribute("source-gateway", value);
            }
        }

        public TokenState State
        {
            get
            {
                TokenState result;
                Enum.TryParse<TokenState>(GetTokenAttribute("state"), out result);
                return result;
            }
            set
            {
                SetTokenAttribute("state", value.ToString());
            }
        }

        public DateTime Timeout
        {
            get
            {
                return DateTime.Parse(GetTokenAttribute("timeout"), CultureInfo.InvariantCulture);
            }
            set
            {
                SetTokenAttribute("timeout", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public EndpointAddress SourceAddress
        {
            get
            {
                var sourceAddressElement = Content.XDocument.XPathSelectElement("token/metadata/source-address");
                string gateway = sourceAddressElement.Attribute(XName.Get("gateway")).Value;
                string adapter = sourceAddressElement.Attribute(XName.Get("adapter")).Value;
                string endpoint = sourceAddressElement.Attribute(XName.Get("endpoint")).Value;

                return new EndpointAddress(gateway, adapter, endpoint);
            }
            set
            {
                var sourceAddressElement = Content.XDocument.XPathSelectElement("token/metadata/source-address");
                if (sourceAddressElement == null) {
                    sourceAddressElement = new XElement("source-address");
                    Content.XDocument.XPathSelectElement("token/metadata").Add(sourceAddressElement);
                }
                sourceAddressElement.SetAttributeValue("gateway", value.GatewayName);
                sourceAddressElement.SetAttributeValue("adapter", value.AdapterName);
                sourceAddressElement.SetAttributeValue("endpoint", value.EndPointName);
            }
        }

        public Token()
        {
            Guid = Guid.NewGuid();
            Content = new SerializableXDocument(XDocument.Parse(@"
<token>
  <metadata>
  </metadata>
  <messageflow-state>
  </messageflow-state>
  <messages>
  </messages>
</token>
                "));
        }

        public Token Clone()
        {
            Token result = new Token();
            result.Guid = Guid;
            result.Content = Content;
            return result;
        }

        public void SaveMessageFlowState()
        {
            SaveMessageFlowState(MessageFlowState);
        }

        public void SaveMessageFlowState(MessageFlowState messageFlowState)
        {
            XElement workFlowStateElement = Content.XDocument.XPathSelectElement("token/messageflow-state");
            XmlSerializer serializer = new XmlSerializer(typeof(MessageFlowState));
            XmlWriter xmlWriter = workFlowStateElement.CreateWriter();
            serializer.Serialize(xmlWriter, messageFlowState);
        }

        public void SetInputAdapterVariable(string name, string value)
        {
            var inputAdapterVariables = Content.XDocument.XPathSelectElement("token/metadata/source-adapter-variables");
            var inputAdapterVariable = inputAdapterVariables.XPathSelectElement("variable[@name=]" + name);
            if (inputAdapterVariable == null) {
                inputAdapterVariable = new XElement("variable");
                inputAdapterVariable.SetAttributeValue("name", name);
                inputAdapterVariables.Add(inputAdapterVariable);
            }
            inputAdapterVariable.SetAttributeValue("value", value);
        }

        public string GetInputAdapterVariable(string name)
        {
            var inputAdapterVariable = Content.XDocument.XPathSelectElement("token/metadata/source-adapter-variables/variable[@name=]" + name);
            return inputAdapterVariable.Attribute(XName.Get("value")).Value;
        }

        public void AddMessage(XDocument message, string name)
        {
            var messagesElement = Content.XDocument.XPathSelectElement("token/messages");
            XElement messageElement = new XElement(XName.Get("message"), message);
            messageElement.SetAttributeValue(XName.Get("name"), name);
        }

        public XElement GetMessage(string name)
        {
            var message = Content.XDocument.XPathSelectElement("token/messages/message[@name=]" + name);
            return message;
        }

        private string GetMetadataAttribute(string name)
        {
            var metedataAttribute = Content.XDocument.XPathSelectElement("token/metaData").Attribute(XName.Get(name));
            if (metedataAttribute != null) {
                return metedataAttribute.Value;
            } else {
                return null;
            }
        }

        private void SetMetadataAttribute(string name, string value)
        {
            Content.XDocument.XPathSelectElement("token/metaData").SetAttributeValue(name, value);
        }

        private string GetTokenAttribute(string name)
        {
            var tokenAttribute = Content.XDocument.XPathSelectElement("token").Attribute(XName.Get(name));
            if (tokenAttribute != null) {
                return tokenAttribute.Value;
            } else {
                return null;
            }
        }

        private void SetTokenAttribute(string name, string value)
        {
            Content.XDocument.XPathSelectElement("token").SetAttributeValue(name, value);
        }
    }
}
