using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.MessageFlowConfig;
using System.Xml.Serialization;
using XRouter.Common.Utils;
using System.Runtime.Serialization;

namespace XRouter.Common
{
    [Serializable]
    [DataContract]
    public class ApplicationConfiguration
    {
        [DataMember]
        public SerializableXDocument Content { get; private set; }

        public ApplicationConfiguration()
        {
        }

        public ApplicationConfiguration(XDocument content)
        {
            Content = new SerializableXDocument(content);
        }

        public ApplicationConfiguration GetReducedConfiguration(XmlReduction reduction)
        {
            XDocument reducedContent = reduction.GetReducedXml(Content);
            var result = new ApplicationConfiguration(reducedContent);
            return result;
        }

        #region General component operations

        public XElement GetComponentElements(ComponentType componentType)
        {
            string componentElementName = componentType.ToString().ToLower();
            var xComponents = System.Xml.XPath.Extensions.XPathSelectElements(Content, string.Format("/configuration/components/{0}", componentElementName));
            XElement xRoot = new XElement(XName.Get(componentElementName));
            foreach (var xComponenent in xComponents) {
                xRoot.Add(xComponenent);
            }
            return xRoot;
        }

        public void SaveComponentElements(ComponentType componentType, XElement xNewComponents)
        {
            string componentElementName = componentType.ToString().ToLower();
            var xOldComponents = System.Xml.XPath.Extensions.XPathSelectElements(Content, string.Format("/configuration/components/{0}", componentElementName));
            foreach (var xComponent in xOldComponents) {
                xComponent.Remove();
            }

            var xComponents = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/components");
            foreach (var xNewComponent in xNewComponents.Elements()) {
                xComponents.Add(xNewComponent);
            }
        }

        public void SaveComponentElement(ComponentType componentType, XElement xComponent)
        {
            string componentElementName = componentType.ToString().ToLower();
            string name = xComponent.Attribute(XName.Get("name")).Value;
            var xOldComponent = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/{0}[@name='{1}']", componentElementName, name));
            if (xOldComponent != null) {
                xOldComponent.Remove();
            }

            var xComponents = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/components");
            xComponents.Add(xComponent);
        }

        public void AddComponent(ComponentType componentType, string name)
        {
            XElement xComponent;
            switch (componentType) {
		        case ComponentType.Gateway:
                    xComponent = XElement.Parse(string.Format(@"
<gateway name='{0}'>
    <adapters>
    </adapters>
</gateway>
", name));
                    break;
                case ComponentType.Processor:
                    xComponent = XElement.Parse(string.Format(@"
<processor name='{0}' concurrent-threads='4'>
</processor>
", name));

                    break;
                default:
                    throw new ArgumentException("Unknown component type", "componentType");
	        }
            var xComponents = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/components");
            xComponents.Add(xComponent);
        }

        public void RemoveComponent(string name)
        {
            var xComponents = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/components");
            foreach (var xComponent in xComponents.Elements().ToArray()) {
                string componentName = xComponent.Attribute(XName.Get("name")).Value;
                if (componentName == name) {
                    xComponent.Remove();
                    return;
                }
            }
        }        

        public string[] GetComponentNames()
        {
            var componentsConfigs = System.Xml.XPath.Extensions.XPathSelectElements(Content, "/configuration/components/gateway").Union(
                                    System.Xml.XPath.Extensions.XPathSelectElements(Content, "/configuration/components/processor"));
            var allComponentsNames = from cfg in componentsConfigs
                                     select cfg.Attribute(XName.Get("name")).Value;

            return allComponentsNames.ToArray();
        }
        #endregion

        #region Adapter types

        public AdapterType[] GetAdapterTypes()
        {
            var xAdapterTypes = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/adapter-types");
            List<AdapterType> result = new List<AdapterType>();
            foreach (XElement xAdapterType in xAdapterTypes.Elements()) {
                string name = xAdapterType.Attribute(XName.Get("name")).Value;
                string assemblyAndClrType = xAdapterType.Attribute(XName.Get("clr-type")).Value;
                AdapterType adapterType = new AdapterType(name, assemblyAndClrType);
                result.Add(adapterType);
            }
            return result.ToArray();
        }

        public AdapterType GetAdapterType(string name)
        {
            var xAdapterType = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/adapter-types/adapter-type[@name='{0}']", name));
            string assemblyAndClrType = xAdapterType.Attribute(XName.Get("clr-type")).Value;
            AdapterType adapterType = new AdapterType(name, assemblyAndClrType);
            return adapterType;
        }

        public void AddAdapterType(AdapterType adapterType)
        {
            XElement xAdapterType = new XElement(XName.Get("adapter-type"));
            xAdapterType.SetAttributeValue(XName.Get("name"), adapterType.Name);
            xAdapterType.SetAttributeValue(XName.Get("clr-type"), adapterType.AssemblyAndClrType);

            var xAdapterTypes = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/adapter-types");
            xAdapterTypes.Add(xAdapterType);
        }

        public void RemoveAdapterType(string name)
        {
            var xAdapterType = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/adapter-types/adapter-type[@name='{0}']", name));
            xAdapterType.Remove();
        }

        #endregion

        #region Adapter configurations

        public XElement[] GetAdapterConfigurations(string gatewayName)
        {
            var xAdapters = System.Xml.XPath.Extensions.XPathSelectElements(Content, string.Format("/configuration/components/gateway[@name='{0}']/adapters/adapter", gatewayName));
            return xAdapters.ToArray();
        }

        public XElement GetAdapterConfiguration(string gatewayName, string adapterName)
        {
            var xAdapter = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/gateway[@name='{0}']/adapters/adapter[@name='{1}']", gatewayName, adapterName));
            return xAdapter;
        }

        public void SaveAdapterConfiguration(string gatewayName, XElement xAdapterConfiguration)
        {
            string adapterName = xAdapterConfiguration.Attribute(XName.Get("name")).Value;
            var xOldAdapter = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/gateway[@name='{0}']/adapters/adapter[@name='{1}']", gatewayName, adapterName));
            if (xOldAdapter != null) {
                xOldAdapter.Remove();
            }

            var xAdapters = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/gateway[@name='{0}']/adapters", gatewayName));
            xAdapters.Add(xAdapterConfiguration);
        }

        public void RemoveAdapterConfiguration(string gatewayName, string adapterName)
        {
            var xAdapter = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/gateway[@name='{0}']/adapters/adapter[@name='{1}']", gatewayName, adapterName));
            xAdapter.Remove();
        }

        #endregion

        public TimeSpan GetNonRunningProcessorResponseTimeout()
        {
            string value = System.Xml.XPath.Extensions.XPathSelectElement(Content, "/configuration/dispatcher").Attribute(XName.Get("nonrunning-processor-response-timeout")).Value;
            TimeSpan result = TimeSpan.FromSeconds(int.Parse(value));
            return result;
        }

        public ComponentType GetComponentType(string componentName)
        {
            if (System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/gateway[@name='{0}']", componentName)) != null) {
                return ComponentType.Gateway;
            }
            if (System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/processor[@name='{0}']", componentName)) != null) {
                return ComponentType.Processor;
            }
            throw new ArgumentException("Cannot find component with give name.");
        }

        public XElement GetComponentConfiguration(string componentName)
        {
            XElement result = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/gateway[@name='{0}']", componentName));
            if (result != null) { return result; }

            result = System.Xml.XPath.Extensions.XPathSelectElement(Content, string.Format("/configuration/components/processor[@name='{0}']", componentName));
            if (result != null) { return result; }

            throw new ArgumentException("Cannot find component with give name.");
        }

        public int GetConcurrentThreadsCountForProcessor(string componentName)
        {
            XElement processor = GetComponentConfiguration(componentName);
            int result = int.Parse(processor.Attribute(XName.Get("concurrent-threads")).Value);
            return result;
        }

        public Guid GetCurrentMessageFlowGuid()
        {
            XElement xMessageFlows = Content.XDocument.XPathSelectElement("/configuration/messageflows");
            String current = xMessageFlows.Attribute(XName.Get("current")).Value;
            Guid result = Guid.Parse(current);
            return result;
        }

        public void SetCurrentMessageFlowGuid(Guid guid)
        {
            XElement xMessageFlows = Content.XDocument.XPathSelectElement("/configuration/messageflows");
            xMessageFlows.SetAttributeValue(XName.Get("current"), guid.ToString());
        }

        public MessageFlowConfiguration GetMessageFlow(Guid guid)
        {
            string xpath = string.Format("/configuration/messageflows/messageflow[@guid='{0}']", guid);
            XElement xMessageFlow = Content.XDocument.XPathSelectElement(xpath);

            System.Diagnostics.Debug.Assert(xMessageFlow != null);

            var result = XSerializer.Deserialize<MessageFlowConfiguration>(xMessageFlow);
            if (result == null)
            {
                System.Diagnostics.Debug.Assert(xMessageFlow.Attribute("name") != null);
                System.Diagnostics.Debug.Assert(xMessageFlow.Attribute("version") != null);

                string name = xMessageFlow.Attribute("name").Value;
                int version = 0;
                bool versionParsedOk = Int32.TryParse(xMessageFlow.Attribute("version").Value, out version);
                
                System.Diagnostics.Debug.Assert(versionParsedOk);
                
                result = new MessageFlowConfiguration(name, version);
            }
            return result;
        }

        public void AddMessageFlow(MessageFlowConfiguration messageFlow)
        {
            XElement xMessageFlow = new XElement(XName.Get("messageflow"));
            XSerializer.Serializer(messageFlow, xMessageFlow);
            xMessageFlow.SetAttributeValue(XName.Get("guid"), messageFlow.Guid);

            Content.XDocument.XPathSelectElement("/configuration/messageflows").Add(xMessageFlow);
        }

        public XDocument GetXrmContent()
        {
            XElement xrmContent = Content.XDocument.XPathSelectElement("/configuration/xml-resource-storage");
            XDocument result = new XDocument();
            result.Add(xrmContent);
            return result;
        }

        public void SaveXrmContent(XElement xrmContent)
        {
            XElement oldXrmContent = Content.XDocument.XPathSelectElement("/configuration/xml-resource-storage");
            XElement xContainer = oldXrmContent.Parent;
            oldXrmContent.Remove();
            xContainer.Add(xrmContent);
        }
    }
}
