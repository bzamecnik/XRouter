using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Manager;

namespace XRouter.Test.Common
{
    /// <summary>
    /// Provides a way for loading configuration for tests from data files
    /// into an XRouter instance.
    /// </summary>
    /// <remarks>Configuration inludes message flows and XRM items.</remarks>
    public class ConfigurationManager
    {
        /// <summary>
        /// Path from which to load test-specific or shared configuration files.
        /// </summary>
        /// <remarks>Relative to the test working directory.</remarks>
        public string BasePath { get; set; }

        private IConsoleServer consoleServer;

        public ConfigurationManager(IConsoleServer consoleServer)
        {
            BasePath = @"..\XRouter\Test.Integration\Data\";
            this.consoleServer = consoleServer;
        }

        /// <summary>
        /// Modifies the current XRouter configuration with a custom message
        /// flow and XML resource storage.
        /// </summary>
        public void ReplaceConfiguration(
            MessageFlowConfiguration messageFlow,
            XElement xrm)
        {
            ReplaceConfiguration(consoleServer, messageFlow, xrm, null);
        }

        public void ReplaceConfiguration(
            MessageFlowConfiguration messageFlow,
            XElement xrm,
            AdapterConfiguration[] adapters)
        {
            ReplaceConfiguration(consoleServer, messageFlow, xrm, adapters);
        }

        /// <summary>
        /// Modifies the current XRouter configuration with a custom message
        /// flow and XML resource storage.
        /// </summary>
        public void ReplaceConfiguration(
            IConsoleServer consoleServer,
            MessageFlowConfiguration messageFlow,
            XElement xrm,
            AdapterConfiguration[] adapters)
        {
            // load current configuration
            var configuration = consoleServer.GetConfiguration();
            var xConfig = configuration.Content.XDocument;

            if (adapters != null)
            {
                // remove all adapters
                xConfig.XPathSelectElement("/configuration/components/gateway/adapters").RemoveNodes();

                var gateway = xConfig.XPathSelectElements("/configuration/components/gateway").FirstOrDefault();
                if (gateway != null)
                {
                    string gatewayName = gateway.Attribute("name").Value;
                    foreach (var adapter in adapters)
                    {
                        configuration.SaveAdapterConfiguration(adapter);
                    }
                }
            }

            // remove all message flows
            xConfig.XPathSelectElement("/configuration/messageflows").RemoveNodes();

            // update current message flow
            configuration.UpdateMessageFlow(messageFlow);

            // remove all previous XRM items
            xConfig.XPathSelectElement("/configuration/xml-resource-storage").RemoveNodes();

            // add needed XRM items
            configuration.SaveXrmContent(xrm);

            // update the configuration
            consoleServer.ChangeConfiguration(configuration);
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
