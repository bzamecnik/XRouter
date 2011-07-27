using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using wcf = System.ServiceModel;

namespace XRouter.Test.Common
{
    public class ConfigurationManager
    {
        public string BasePath { get; set; }

        public ConfigurationManager()
        {
            BasePath = @"Data\";
        }

        public static IBrokerServiceForManagement GetBrokerServiceProxy()
        {
            // NOTE: code taken from XRouter.Gui.ConfigurationManager
            wcf.EndpointAddress endpointAddress = new wcf.EndpointAddress("net.pipe://localhost/XRouter.ServiceForManagement");
            var binding = new wcf.NetNamedPipeBinding(wcf.NetNamedPipeSecurityMode.None);
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxBytesPerRead = int.MaxValue, MaxArrayLength = int.MaxValue, MaxStringContentLength = int.MaxValue };
            wcf.ChannelFactory<IBrokerServiceForManagement> channelFactory = new wcf.ChannelFactory<IBrokerServiceForManagement>(binding, endpointAddress);
            return channelFactory.CreateChannel();
        }

        /// <summary>
        /// Modifies the current XRouter configuration with a custom message
        /// flow and XML resource storage.
        /// </summary>
        public static void ReplaceConfiguration(
            MessageFlowConfiguration messageFlow,
            XElement xrm)
        {
            IBrokerServiceForManagement broker = GetBrokerServiceProxy();
            ReplaceConfiguration(broker, messageFlow, xrm);
        }

        /// <summary>
        /// Modifies the current XRouter configuration with a custom message
        /// flow and XML resource storage.
        /// </summary>
        public static void ReplaceConfiguration(
            IBrokerServiceForManagement broker,
            MessageFlowConfiguration messageFlow,
            XElement xrm)
        {
            // load current configuration
            var configuration = broker.GetConfiguration();
            var xConfig = configuration.Content.XDocument;

            // remove all message flows
            xConfig.XPathSelectElement("/configuration/messageflows").RemoveNodes();

            // add current message flow
            configuration.AddMessageFlow(messageFlow);
            configuration.SetCurrentMessageFlowGuid(messageFlow.Guid);

            // remove all previous XRM items
            xConfig.XPathSelectElement("/configuration/xml-resource-storage").RemoveNodes();

            // add needed XRM items
            configuration.SaveXrmContent(xrm);

            // update the configuration
            broker.ChangeConfiguration(configuration);
        }

        /// <summary>
        /// Creates a XML resource storage and loads all requested items
        /// from files into the storage.
        /// </summary>
        /// <param name="resourceNames"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        public XElement LoadXrmItems(IEnumerable<string> resourceNames, string testName)
        {
            XElement storage = new XElement("xml-resource-storage");
            foreach (string resource in resourceNames)
            {
                XElement item = LoadXrmItemFromFile(resource, testName);
                storage.Add(item);
            }
            return storage;
        }

        /// <summary>
        /// Loads a XML file. First try test-specific location, if it
        /// is not available try a common location.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="subPath"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        private XDocument LoadXmlFile(string resourceName, string subPath, string testName)
        {
            if (!Regex.IsMatch(resourceName, @"[a-zA-Z0-9_\-]+"))
            {
                throw new ArgumentException();
            }
            string fileName = Path.Combine(BasePath, string.Format(@"{0}\{1}\{2}.xml", testName, subPath, resourceName));
            if (!File.Exists(fileName))
            {
                fileName = Path.Combine(BasePath, string.Format(@"Common\{0}\{1}.xml", subPath, resourceName));
            }
            return XDocument.Load(fileName);
        }

        /// <summary>
        /// Loads a XML resource item from a file.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        private XElement LoadXrmItemFromFile(string resourceName, string testName)
        {
            XDocument doc = LoadXmlFile(resourceName, @"Config\XRM", testName);
            XElement item = new XElement("item");
            item.SetAttributeValue("name", resourceName);
            item.Add(doc.Root);
            return item;
        }

        /// <summary>
        /// Loads a message flow configuration from a file.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        public XElement LoadMessageFlowFromFile(string resourceName, string testName)
        {
            XDocument doc = LoadXmlFile(resourceName, @"Config\MessageFlow", testName);
            return doc.Root;
        }
    }
}
