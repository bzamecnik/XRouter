using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Prototype.CoreTypes
{
    class Token
    {
        public Guid Guid { get; private set; }
        
        public XDocument Content { get; set; }

        public WorkflowState WorkflowState { get; private set; }
                     
        public TokenState State {
            get { 
                TokenState result;
                Enum.TryParse<TokenState>(GetCommonMetadata("state"), out result);
                return result;
            }
            set {
                SetCommonMetadata("state", value.ToString());
            }
        }

        public Token()
        {
            Guid = Guid.NewGuid();
            Content = XDocument.Parse(@"
<token>
    <metadata>
        <common>
        </common>
    </metdata>
    <messages>
    </messages>
</token>
");
            WorkflowState = new WorkflowState();
        }

        public Token Clone()
        {
            Token result = new Token();
            result.Guid = Guid;
            result.Content = Content;
        
            return result;
        }

        private string GetCommonMetadata(string name)
        {
            XElement element = Content.XPathSelectElement(string.Format("/token/metadata/common/value[@name='{0}']", name));
            if (element != null) {
                return element.Value;
            } else {
                return string.Empty;
            }
        }

        private void SetCommonMetadata(string name, string value)
        {
            XElement commonMetadata = Content.XPathSelectElement(string.Format("/token/metadata/common", name));
            XElement element = commonMetadata.XPathSelectElement(string.Format("value[@name='{0}']", name));
            if (element == null) {
                element = new XElement(XName.Get("value"));
                commonMetadata.Add(element);
            }
            element.SetAttributeValue(XName.Get(name), value);
        }

        /// <summary>
        /// Pouzilo by se pro citilive nody. Tj. nody, ktere nesmi token 
        /// projit 2x. Takze token by si drzel (persistentni) kolekci takovych 
        /// nodu. 
        /// </summary>
        /// <param name="id"></param>
        public void SaveVisitedNode(Int32 id)
        {


        }

        public Boolean IsNodeVisited(Int32 id)
        {
            return false;
        }

    }
}
