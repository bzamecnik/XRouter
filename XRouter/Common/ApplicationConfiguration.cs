using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.MessageFlowConfig;
using System.Xml.Serialization;
using XRouter.Common.Utils;

namespace XRouter.Common
{
    [Serializable]
    public class ApplicationConfiguration
    {
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

        public string[] GetComponentNames()
        {
            var componentsConfigs = System.Xml.XPath.Extensions.XPathSelectElements(Content, "/configuration/components/gateway").Union(
                                    System.Xml.XPath.Extensions.XPathSelectElements(Content, "/configuration/components/processor"));
            var allComponentsNames = from cfg in componentsConfigs
                                     select cfg.Attribute(XName.Get("name")).Value;

            return allComponentsNames.ToArray();
        }

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

            var result = XSerializer.Deserialize<MessageFlowConfiguration>(xMessageFlow);
            return result;
        }

        public void AddMessageFlow(MessageFlowConfiguration messageFlow)
        {
            XElement xMessageFlow = new XElement(XName.Get("messageflow"));
            XSerializer.Serializer(messageFlow, xMessageFlow);
            xMessageFlow.SetAttributeValue(XName.Get("guid"), messageFlow.Guid);

            Content.XDocument.XPathSelectElement("/configuration/messageflows").Add(xMessageFlow);
        }
    }
}
