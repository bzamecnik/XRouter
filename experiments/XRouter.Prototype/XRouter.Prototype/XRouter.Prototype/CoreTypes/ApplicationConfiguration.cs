using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Prototype.CoreTypes
{
    class ApplicationConfiguration
    {
        public XDocument Content { get; private set; }

        public ApplicationConfiguration(XDocument content)
        {
            Content = content;
        }

        public ApplicationConfiguration GetReducedConfiguration(XmlReduction reduction)
        {
            XDocument reducedContent = reduction.GetReducedXml(Content);
            var result = new ApplicationConfiguration(reducedContent);
            return result;
        }

        public string[] GetComponentNames()
        {
            var componentsConfigs = Content.XPathSelectElements("/configuration/components/gateway").Union(
                                    Content.XPathSelectElements("/configuration/components/processor"));
            var allComponentsNames = from cfg in componentsConfigs
                                     select cfg.Attribute(XName.Get("name")).Value;

            return allComponentsNames.ToArray();
        }

        public int GetLastWorkflowVersion()
        {
            var workflows = Content.XPathSelectElements("/configuration/workflows/workflow");
            var versions = from wf in workflows
                           select int.Parse(wf.Attribute(XName.Get("version")).Value);
            if (versions.Any()) {
                return versions.Max();
            } else {
                return 0;
            }
        }

        public TimeSpan GetNonRunningProcessorResponseTimeout()
        {
            string value = Content.XPathSelectElement("/configuration/dispatcher").Attribute(XName.Get("nonRunningProcessorResponseTimeout")).Value;
            TimeSpan result = TimeSpan.FromSeconds(int.Parse(value));
            return result;
        }

        public ComponentType GetComponentType(string componentName)
        {
            if (Content.XPathSelectElement(string.Format("/configuration/components/gateway[@name='{0}']", componentName)) != null) {
                return ComponentType.Gateway;
            }
            if (Content.XPathSelectElement(string.Format("/configuration/components/processor[@name='{0}']", componentName)) != null) {
                return ComponentType.Processor;
            }
            throw new ArgumentException("Cannot find component with give name.");
        }
    }
}
