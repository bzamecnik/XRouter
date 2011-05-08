using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;

namespace XRouter.Common
{
    //! Token je vytvařen na adapteru, ktery zada prvni msg a inputEndpoit, doplni specificke metadata (pres property)
    // GW doplni ostatni obecna metadata

    // V metadatech pridat adresu input endpoint, tri casti -> serializovat do stringu ( nastavi GW )

    [Serializable]
    public class Token
    {
        public Guid Guid { get; private set; }

        public SerializableXDocument Content { get; private set; }

        public WorkflowState WorkFlowState
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string GatewayName
        {
            get
            {
                return GetTokenAttribute("gatewayName");
            }
            set
            {
                SetTokenAttribute("gatewayName", value);
            }
        }

        public string ResultMessageName
        {
            get
            {
                return GetMetedataAttribute("resultMessageName");
            }
            set
            {
                SetMetedataAttribute("resultMessageName", value);
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

        public WorkflowState WorkflowState
        {
            get
            {
                throw new NotImplementedException(); // Vytahnout z XML content
            }
            set
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException(); // Vytahnout z XML content
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Token()
        {
            Guid = Guid.NewGuid();
            Content = new SerializableXDocument(XDocument.Parse(@"
<token>
  <metaData>
  </metaData>
  <workFlowState>
    <currentNodes>
    </currentNodes>
    <processorData>
    </processorData>
    <varibles>
    </varibles>
  </workFlowState>
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

        public void SetInputAdapterVariable(string name, string value)
        {
            throw new NotImplementedException();
        }

        public string GetInputAdapterVariable(string name)
        {
            throw new NotImplementedException();
        }

        public void AddMessage(XDocument message)
        {
            AddMessage(message, Guid.NewGuid().ToString());
        }

        public void AddMessage(XDocument message, string name)
        {
            var messagesElement = Content.XDocument.XPathSelectElement("token/messages");
            XElement messageElement = new XElement(XName.Get("message"), message);
            messageElement.SetAttributeValue(XName.Get("name"), name);
        }

        public void SetResaultMessageName(string resultMessageName)
        {
            SetMetedataAttribute("resultMessageName", resultMessageName);
        }

        public string GetResultMessage()
        {
            string resultMessageName = ResultMessageName;
            if (resultMessageName != null) {
                var messageElement = GetMessage(resultMessageName).Elements().FirstOrDefault();
                if (messageElement != null) {
                    return messageElement.ToString();
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        public XElement GetMessage(string name)
        {
            throw new NotImplementedException();
        }

        private string GetMetedataAttribute(string name)
        {
            throw new NotImplementedException();
        }

        private void SetMetedataAttribute(string name, string value)
        {
            throw new NotImplementedException();
        }

        private string GetTokenAttribute(string name)
        {
            throw new NotImplementedException();
        }

        private void SetTokenAttribute(string name, string value)
        {
            throw new NotImplementedException();
        }
    }
}
