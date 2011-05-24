using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.MessageFlowConfig;
using System.Xml.Serialization;

namespace XRouter.Common
{
    [Serializable]
    public class ApplicationConfiguration
    {
        public SerializableXDocument Content { get; private set; }

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

        public Uri GetComponentAddress(string componentName)
        {
            XElement config = GetComponentConfiguration(componentName);
            string address = config.Attribute(XName.Get("address")).Value;
            Uri result = new Uri(address);
            return result;
        }

        public Uri GetComponentControllerAddress(string componentName)
        {
            XElement config = GetComponentConfiguration(componentName);
            string address = config.Attribute(XName.Get("controller-address")).Value;
            Uri result = new Uri(address);
            return result;
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
            int result = int.Parse((string)Content.XDocument.XPathEvaluate(".@concurrentThreads"));
            return result;
        }

        public Guid GetCurrentMessageFlowGuid()
        {
            Guid result = Guid.Parse((string)Content.XDocument.XPathEvaluate("/configuration/messageflows@current-guid"));
            return result;
        }

        public MessageFlowConfiguration GetMessageFlow(Guid guid)
        {
            XElement messageFlowElement = Content.XDocument.XPathSelectElement(string.Format("/configuration/messageflows/messageflow[@guid='{0}']", guid));
            XmlSerializer serializer = new XmlSerializer(typeof(MessageFlowConfiguration));
            var result = (MessageFlowConfiguration)serializer.Deserialize(messageFlowElement.CreateReader());
            return result;
        }

        public void AddMessageFlow(MessageFlowConfiguration messageFlow)
        {
            XElement messageFlowElement = new XElement(XName.Get("messageflow"));
            messageFlowElement.SetAttributeValue(XName.Get("guid"), messageFlow.Guid);
            XmlSerializer serializer = new XmlSerializer(typeof(MessageFlowConfiguration));
            serializer.Serialize(messageFlowElement.CreateWriter(), messageFlow);
            Content.XDocument.XPathSelectElement("/configuration/messageflows").Add(messageFlowElement);
        }
    }
}
