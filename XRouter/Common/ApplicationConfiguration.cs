using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using ObjectConfigurator;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;

namespace XRouter.Common
{
    /// <summary>
    /// Represents a complete configuration of the whole XRouter application
    /// in a single XML document and also provides a set of operations to
    /// manipulate it.
    /// </summary>
    /// <remarks>
    /// A complete configuration includes components, adapter plug-in types
    /// and configurations, message flows and XML resource storage. However,
    /// each component can obtain a configuration reduced (filtered) only to
    /// parts of interest of that particular component (in order to save
    /// bandwidth in communication between components).
    /// </remarks>
    [Serializable]
    [DataContract]
    public class ApplicationConfiguration
    {
        public static readonly string InitialContent = @"
<configuration>
    <components>
        <gateway name='gateway'>
            <adapters>
            </adapters>
        </gateway>
        <processor name='processor' concurrent-threads='4'>
        </processor>
    </components>
    <dispatcher nonrunning-processor-response-timeout='60'>
    </dispatcher>
    <adapter-types>
    </adapter-types>
    <action-types>
    </action-types>
    <messageflows current='d77b24b8-9333-457b-92e8-dd1624f3e9ce'>
        <messageflow guid='d77b24b8-9333-457b-92e8-dd1624f3e9ce'>
          <MessageFlowConfiguration xmlns:i='http://www.w3.org/2001/XMLSchema-instance' xmlns:z='http://schemas.microsoft.com/2003/10/Serialization/' xmlns='http://schemas.datacontract.org/2004/07/XRouter.Common.MessageFlowConfig' z:Id='1'>
            <Guid>d77b24b8-9333-457b-92e8-dd1624f3e9ce</Guid>
            <Name z:Id='2'>default</Name>
            <Nodes z:Id='3' z:Size='1'>
              <NodeConfiguration z:Id='4' i:type='TerminatorNodeConfiguration'>
                <Location xmlns:d4p1='http://schemas.datacontract.org/2004/07/System.Windows'>
                  <d4p1:_x>0</d4p1:_x>
                  <d4p1:_y>0</d4p1:_y>
                </Location>
                <_name z:Id='5'>Terminator</_name>
                <IsReturningOutput>false</IsReturningOutput>
                <ResultMessageSelection z:Id='6'>
                  <SelectionPattern z:Id='7' />
                </ResultMessageSelection>
              </NodeConfiguration>
            </Nodes>
            <RootNode i:nil='true' />
            <Version>3</Version>
          </MessageFlowConfiguration>
        </messageflow>
    </messageflows>
    <xml-resource-storage>
    </xml-resource-storage>
</configuration>
";

        /// <summary>
        /// The configuration represented as a XML document.
        /// </summary>
        [DataMember]
        public SerializableXDocument Content { get; private set; }

        public ApplicationConfiguration()
        //:this(null)
        {
            // TODO: Content should probably be initialized with an empty XDocument
            // (care must be taken with deserialization).
        }

        public ApplicationConfiguration(XDocument content)
        {
            Content = new SerializableXDocument(content);
        }

        /// <summary>
        /// Gets a configuration reduced with given reduction.
        /// </summary>
        /// <param name="reduction">a filter to select only parts of the
        /// configuration</param>
        /// <returns>reduced configuration</returns>
        public ApplicationConfiguration GetReducedConfiguration(XmlReduction reduction)
        {
            XDocument reducedContent = reduction.GetReducedXml(Content);
            var result = new ApplicationConfiguration(reducedContent);
            return result;
        }

        #region General component operations

        /// <summary>
        /// Gets a representation of all components of a particular type.
        /// </summary>
        /// <param name="componentType">type of the components</param>
        /// <returns>An XML element which has the same name as the type of
        /// the component and contains the component representations as its
        /// children.</returns>
        public XElement GetComponentElements(ComponentType componentType)
        {
            string componentElementName = componentType.ToString().ToLower();
            var xComponents = Content.XDocument.XPathSelectElements(
                string.Format("/configuration/components/{0}", componentElementName));
            XElement xRoot = new XElement(XName.Get(componentElementName));
            // NOTE: sort elements in order to provide consistent result
            // eg. the first component of a type is always selected the same
            // for the same data
            foreach (var xComponenent in xComponents.OrderBy((comp) => comp.Name))
            {
                xRoot.Add(xComponenent);
            }
            return xRoot;
        }

        /// <summary>
        /// Sets a new representation of all components of a particular type.
        /// </summary>
        /// <param name="componentType">type of the components</param>
        /// <param name="xNewComponents">XML element containing the
        /// component representations as its children.</param>
        public void SaveComponentElements(ComponentType componentType, XElement xNewComponents)
        {
            string componentElementName = componentType.ToString().ToLower();
            var xOldComponents = Content.XDocument.XPathSelectElements(
                string.Format("/configuration/components/{0}", componentElementName));
            foreach (var xComponent in xOldComponents)
            {
                xComponent.Remove();
            }

            var xComponents = Content.XDocument.XPathSelectElement("/configuration/components");
            foreach (var xNewComponent in xNewComponents.Elements())
            {
                xComponents.Add(xNewComponent);
            }
        }

        /// <summary>
        /// Sets a new representation of a single component of a particular type.
        /// </summary>
        /// <param name="componentType">type of the component</param>
        /// <param name="xComponent">XML element containing the component
        /// representation</param>
        public void SaveComponentElement(ComponentType componentType, XElement xComponent)
        {
            string componentElementName = componentType.ToString().ToLower();
            string name = xComponent.Attribute(XName.Get("name")).Value;
            var xOldComponent = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/components/{0}[@name='{1}']", componentElementName, name));
            if (xOldComponent != null)
            {
                xOldComponent.Remove();
            }

            var xComponents = Content.XDocument.XPathSelectElement("/configuration/components");
            xComponents.Add(xComponent);
        }

        /// <summary>
        /// Add a new component with a specified name and component type.
        /// </summary>
        /// <remarks>
        /// Initializes the configuration of the component with a default value
        /// </remarks>
        /// <param name="componentType">type of the new component</param>
        /// <param name="name">name of the new component</param>
        public void AddComponent(ComponentType componentType, string name)
        {
            XElement xComponent;
            switch (componentType)
            {
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
                    throw new ArgumentException(string.Format(
                        "Cannot add a component named '{0}', unknown component type {1}.",
                        name, componentType.ToString()), "componentType");
            }
            var xComponents = Content.XDocument.XPathSelectElement("/configuration/components");
            xComponents.Add(xComponent);
        }

        /// <summary>
        /// Removes a representation of an existing component specified by its
        /// name.
        /// </summary>
        /// <param name="name">name of the component to be removed</param>
        public void RemoveComponent(string name)
        {
            var xComponents = Content.XDocument.XPathSelectElement("/configuration/components");

            // TODO: finding the component to be removed is a bit odd
            // Can't we just use XPath instead of iterating and finding manually?

            foreach (var xComponent in xComponents.Elements())
            {
                string componentName = xComponent.Attribute(XName.Get("name")).Value;
                if (componentName == name)
                {
                    xComponent.Remove();
                    return;
                }
            }
        }

        /// <summary>
        /// Gets names of all configured components.
        /// </summary>
        /// <returns></returns>
        public string[] GetComponentNames()
        {
            var gateway = Content.XDocument.XPathSelectElements(
                "/configuration/components/gateway");
            var processor = Content.XDocument.XPathSelectElements(
                "/configuration/components/processor");
            var componentsConfigs = gateway.Union(processor);
            var allComponentsNames = from cfg in componentsConfigs
                                     select cfg.Attribute(XName.Get("name")).Value;

            return allComponentsNames.ToArray();
        }

        /// <summary>
        /// Gets the component type of a component specified by its name.
        /// </summary>
        /// <param name="componentName">name of the component</param>
        /// <exception cref="System.ArgumentException">if there is no component
        /// with such a name</exception>
        /// <returns></returns>
        public ComponentType GetComponentType(string componentName)
        {
            if (Content.XDocument.XPathSelectElement(string.Format(
                "/configuration/components/gateway[@name='{0}']", componentName)) != null)
            {
                return ComponentType.Gateway;
            }
            if (Content.XDocument.XPathSelectElement(string.Format(
                "/configuration/components/processor[@name='{0}']", componentName)) != null)
            {
                return ComponentType.Processor;
            }
            throw new ArgumentException(string.Format(
                "Cannot find component named '{0}'.", componentName), "componentName");
        }

        #endregion

        #region Adapter types

        /// <summary>
        /// Get the information about types of all adapters.
        /// </summary>
        /// <returns></returns>
        public AdapterType[] GetAdapterTypes()
        {
            var xAdapterTypes = Content.XDocument.XPathSelectElement(
                "/configuration/adapter-types");
            List<AdapterType> result = new List<AdapterType>();
            foreach (XElement xAdapterType in xAdapterTypes.Elements())
            {
                string name = xAdapterType.Attribute(XName.Get("name")).Value;
                string assemblyAndClrType = xAdapterType.Attribute(XName.Get("clr-type")).Value;
                XElement xConfigurationMetadata = xAdapterType.Element(XName.Get("class-metadata"));
                ClassMetadata configurationMetadata = XSerializer.Deserialize<ClassMetadata>(xConfigurationMetadata);
                AdapterType adapterType = new AdapterType(name, assemblyAndClrType, configurationMetadata);
                result.Add(adapterType);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets the information about the type of an adapter specified by its
        /// name.
        /// </summary>
        /// <param name="name">name of the adapter</param>
        /// <exception cref="System.ArgumentException">if there is no adapter
        /// with such a name</exception>
        /// <returns></returns>
        public AdapterType GetAdapterType(string name)
        {
            var xAdapterType = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/adapter-types/adapter-type[@name='{0}']", name));

            if (xAdapterType == null)
            {
                throw new ArgumentException(string.Format("Cannot find adapter named '{0}'.", name), "name");
            }

            string assemblyAndClrType = xAdapterType.Attribute(XName.Get("clr-type")).Value;
            XElement xConfigurationMetadata = xAdapterType.Element(XName.Get("class-metadata"));
            ClassMetadata configurationMetadata = XSerializer.Deserialize<ClassMetadata>(xConfigurationMetadata);
            AdapterType adapterType = new AdapterType(name, assemblyAndClrType, configurationMetadata);
            return adapterType;
        }

        /// <summary>
        /// Adds a new type of adapter.
        /// </summary>
        /// <remarks>
        /// If there is any existing adapter type with this name it get replaced
        /// with the given adapter type.
        /// </remarks>
        /// <param name="adapterType">information about adapter type</param>
        public void AddAdapterType(AdapterType adapterType)
        {
            XElement xAdapterType = new XElement(XName.Get("adapter-type"));
            xAdapterType.SetAttributeValue(XName.Get("name"), adapterType.Name);
            xAdapterType.SetAttributeValue(XName.Get("clr-type"), adapterType.AssemblyAndClrType);

            XElement xConfigurationMetadata = new XElement(XName.Get("class-metadata"));
            XSerializer.Serializer(adapterType.ConfigurationMetadata, xConfigurationMetadata);
            xAdapterType.Add(xConfigurationMetadata);

            var xAdapterTypes = Content.XDocument.XPathSelectElement(
                "/configuration/adapter-types");

            // if there is already an adapter with the same name replace the CLR type quietly
            // this maintains the uniqueness of the adapter types
            var xExistingAdapterTypes = Content.XDocument.XPathSelectElements(
                string.Format("/configuration/adapter-types/adapter-type[@name='{0}']", adapterType.Name));
            foreach (var adapter in xExistingAdapterTypes)
            {
                adapter.Remove();
            }

            xAdapterTypes.Add(xAdapterType);
        }

        /// <summary>
        /// Removes an existing information about an adapter type with
        /// specified by its name.
        /// </summary>
        /// <param name="name">name of the adapter type to be removed</param>
        /// <exception cref="System.ArgumentException">if there is no adapter
        /// with such a name</exception>
        public void RemoveAdapterType(string name)
        {
            var xAdapterType = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/adapter-types/adapter-type[@name='{0}']", name));

            if (xAdapterType == null)
            {
                throw new ArgumentException(string.Format("Cannot find adapter named '{0}'.", name), "name");
            }

            xAdapterType.Remove();
        }

        #endregion

        #region Adapter configurations

        /// <summary>
        /// Gets configurations of all adapters of a gateway specified by its
        /// name.
        /// </summary>
        /// <param name="gatewayName">name of the gateway</param>
        /// <returns></returns>
        public AdapterConfiguration[] GetAdapterConfigurations(string gatewayName)
        {
            var xGateway = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/components/gateway[@name='{0}']", gatewayName));
            if (xGateway == null)
            {
                throw new ArgumentException(string.Format(
                    "Cannot find gateway named '{0}'.", gatewayName), "gatewayName");
            }

            var xAdapters = xGateway.Element(XName.Get("adapters")).Elements(XName.Get("adapter"));

            List<AdapterConfiguration> result = new List<AdapterConfiguration>();
            foreach (XElement xAdapter in xAdapters)
            {
                AdapterConfiguration adapterConfig = XSerializer.Deserialize<AdapterConfiguration>(xAdapter);
                result.Add(adapterConfig);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets a configuration of a particular adapter specified by its name and
        /// the name of the gateway it belongs to.
        /// </summary>
        /// <param name="gatewayName">name of the gateway</param>
        /// <param name="adapterName">name of the adapter</param>
        /// <exception cref="System.ArgumentException">if there is no gateway
        /// with a specified name or its adapter with a specified name
        /// </exception>
        /// <returns></returns>
        public AdapterConfiguration GetAdapterConfiguration(string gatewayName, string adapterName)
        {
            var xAdapter = Content.XDocument.XPathSelectElement(string.Format(
                "/configuration/components/gateway[@name='{0}']/adapters/adapter[@name='{1}']",
                gatewayName, adapterName));

            if (xAdapter == null)
            {
                throw new ArgumentException(string.Format(
                    "Cannot find gateway named '{0}' or its adapter named '{1}'.",
                    gatewayName, adapterName));
            }

            AdapterConfiguration result = XSerializer.Deserialize<AdapterConfiguration>(xAdapter);
            return result;
        }

        /// <summary>
        /// Sets a new configuration of a particular adapter specified by its
        /// name (inside the configuration) and the name of the gateway it
        /// belongs to.
        /// </summary>
        /// <remarks>
        /// The adapter need not to exist previously - a new adapter can be
        /// created or an existing updated.
        /// </remarks>
        /// <param name="gatewayName">name of the gateway</param>
        /// <param name="xAdapterConfiguration">new adapter configuration</param>
        public void SaveAdapterConfiguration(AdapterConfiguration adapterConfiguration, string originalName = null)
        {
            if (originalName == null)
            {
                originalName = adapterConfiguration.AdapterName;
            }

            var xOldAdapter = Content.XDocument.XPathSelectElement(string.Format(
                "/configuration/components/gateway[@name='{0}']/adapters/adapter[@name='{1}']",
                adapterConfiguration.GatewayName, originalName));
            if (xOldAdapter != null)
            {
                xOldAdapter.Remove();
            }

            var xAdapters = Content.XDocument.XPathSelectElement(string.Format(
                "/configuration/components/gateway[@name='{0}']/adapters", adapterConfiguration.GatewayName));

            if (xAdapters == null)
            {
                throw new ArgumentException(string.Format(
                    "Cannot find gateway named '{0}'.", adapterConfiguration.GatewayName), "gatewayName");
            }

            XElement xAdapter = new XElement(XName.Get("adapter"));
            xAdapter.SetAttributeValue(XName.Get("name"), adapterConfiguration.AdapterName);
            XSerializer.Serializer(adapterConfiguration, xAdapter);

            xAdapters.Add(xAdapter);
        }

        /// <summary>
        /// Removes a configuration of a particular adapter specified by its
        /// name and the name of the gateway it belongs to.
        /// </summary>
        /// <param name="gatewayName">name of the gateway</param>
        /// <param name="adapterName">name of the adapter</param>
        /// <exception cref="System.ArgumentException">if there is no gateway
        /// with a specified name or its adapter with a specified name
        /// </exception>
        public void RemoveAdapterConfiguration(AdapterConfiguration adapterConfiguration)
        {
            var xAdapter = Content.XDocument.XPathSelectElement(string.Format(
                "/configuration/components/gateway[@name='{0}']/adapters/adapter[@name='{1}']",
                adapterConfiguration.GatewayName, adapterConfiguration.AdapterName));

            if (xAdapter == null)
            {
                throw new ArgumentException(string.Format(
                    "Cannot find gateway named '{0}' or its adapter named '{1}'.",
                    adapterConfiguration.GatewayName, adapterConfiguration.AdapterName));
            }

            xAdapter.Remove();
        }

        #endregion

        #region Action types

        /// <summary>
        /// Get the information about types of all action types.
        /// </summary>
        /// <returns></returns>
        public ActionType[] GetActionTypes()
        {
            var xActionTypes = Content.XDocument.XPathSelectElement(
                "/configuration/action-types");
            List<ActionType> result = new List<ActionType>();
            foreach (XElement xActionType in xActionTypes.Elements())
            {
                string name = xActionType.Attribute(XName.Get("name")).Value;
                string assemblyAndClrType = xActionType.Attribute(XName.Get("clr-type")).Value;
                XElement xConfigurationMetadata = xActionType.Element(XName.Get("class-metadata"));
                ClassMetadata configurationMetadata = XSerializer.Deserialize<ClassMetadata>(xConfigurationMetadata);
                ActionType actionType = new ActionType(name, assemblyAndClrType, configurationMetadata);
                result.Add(actionType);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets the information about the type of an action specified by its
        /// name.
        /// </summary>
        /// <param name="name">name of the action</param>
        /// <exception cref="System.ArgumentException">if there is no action
        /// with such a name</exception>
        /// <returns></returns>
        public ActionType GetActionType(string name)
        {
            var xActionType = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/action-types/action-type[@name='{0}']", name));

            if (xActionType == null)
            {
                throw new ArgumentException(string.Format("Cannot find action type named '{0}'.", name), "name");
            }

            string assemblyAndClrType = xActionType.Attribute(XName.Get("clr-type")).Value;
            XElement xConfigurationMetadata = xActionType.Element(XName.Get("class-metadata"));
            ClassMetadata configurationMetadata = XSerializer.Deserialize<ClassMetadata>(xConfigurationMetadata);
            ActionType actionType = new ActionType(name, assemblyAndClrType, configurationMetadata);
            return actionType;
        }

        /// <summary>
        /// Adds a new type of action.
        /// </summary>
        /// <remarks>
        /// If there is any existing action type with this name it get replaced
        /// with the given action type.
        /// </remarks>
        /// <param name="actionType">information about action type</param>
        public void AddActionType(ActionType actionType)
        {
            XElement xActionType = new XElement(XName.Get("action-type"));
            xActionType.SetAttributeValue(XName.Get("name"), actionType.Name);
            xActionType.SetAttributeValue(XName.Get("clr-type"), actionType.ClrTypeAndAssembly);

            XElement xConfigurationMetadata = new XElement(XName.Get("class-metadata"));
            XSerializer.Serializer(actionType.ConfigurationMetadata, xConfigurationMetadata);
            xActionType.Add(xConfigurationMetadata);

            var xActionTypes = Content.XDocument.XPathSelectElement(
                "/configuration/action-types");

            // if there is already an action with the same name replace the CLR type quietly
            // this maintains the uniqueness of the action types
            var xExistingActionTypes = Content.XDocument.XPathSelectElements(
                string.Format("/configuration/action-types/action-type[@name='{0}']", actionType.Name));
            foreach (var action in xExistingActionTypes)
            {
                action.Remove();
            }

            xActionTypes.Add(xActionType);
        }

        /// <summary>
        /// Removes an existing information about an action type with
        /// specified by its name.
        /// </summary>
        /// <param name="name">name of the action type to be removed</param>
        /// <exception cref="System.ArgumentException">if there is no action
        /// with such a name</exception>
        public void RemoveActionType(string name)
        {
            var xActionType = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/action-types/action-type[@name='{0}']", name));

            if (xActionType == null)
            {
                throw new ArgumentException(string.Format("Cannot find action type named '{0}'.", name), "name");
            }

            xActionType.Remove();
        }

        #endregion

        #region Processor

        /// <summary>
        /// Gets the timeout such that if a processor does not respond within
        /// this time limit it is treated as being unresponsible.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetNonRunningProcessorResponseTimeout()
        {
            var dispatcher = Content.XDocument.XPathSelectElement(
                "/configuration/dispatcher");
            string value = dispatcher.Attribute(XName.Get("nonrunning-processor-response-timeout")).Value;
            TimeSpan result = TimeSpan.FromSeconds(int.Parse(value));
            return result;
        }

        /// <summary>
        /// Gets a configuration of a component specified by its name.
        /// </summary>
        /// <remarks>
        /// The only supported component types are gateway and processor.
        /// </remarks>
        /// <param name="componentName">name of the component</param>
        /// <exception cref="System.ArgumentException">if there is no component
        /// with such a name</exception>
        /// <returns></returns>
        public XElement GetComponentConfiguration(string componentName)
        {
            XElement result = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/components/gateway[@name='{0}']", componentName));
            if (result != null) { return result; }

            result = Content.XDocument.XPathSelectElement(
                string.Format("/configuration/components/processor[@name='{0}']", componentName));
            if (result != null) { return result; }

            throw new ArgumentException(string.Format(
                "Cannot find component named '{0}'.", componentName), "componentName");
        }

        /// <summary>
        /// Gets the number of processing threads which are allowed to run
        /// concurrently in a processor component.
        /// </summary>
        /// <param name="componentName">name of the processor component</param>
        /// <returns></returns>
        public int GetConcurrentThreadsCountForProcessor(string componentName)
        {
            XElement processor = GetComponentConfiguration(componentName);
            int result = int.Parse(processor.Attribute(XName.Get("concurrent-threads")).Value);
            return result;
        }

        public void SetConcurrentThreadsCountForProcessor(string componentName, int concurrentThreads)
        {
            XElement processor = GetComponentConfiguration(componentName);
            processor.SetAttributeValue(XName.Get("concurrent-threads"), concurrentThreads);
        }

        #endregion

        #region Message flow

        /// <summary>
        /// Gets the GUID of the current message flow.
        /// </summary>
        /// <returns></returns>
        public Guid GetCurrentMessageFlowGuid()
        {
            XElement xMessageFlows = Content.XDocument.XPathSelectElement("/configuration/messageflows");
            String current = xMessageFlows.Attribute(XName.Get("current")).Value;
            Guid result = Guid.Parse(current);
            return result;
        }

        /// <summary>
        /// Sets the GUID of the current message flow.
        /// </summary>
        /// <param name="guid">GUID of the new message flow to be set as the
        /// current one</param>
        public void SetCurrentMessageFlowGuid(Guid guid)
        {
            XElement xMessageFlows = Content.XDocument.XPathSelectElement("/configuration/messageflows");
            xMessageFlows.SetAttributeValue(XName.Get("current"), guid.ToString());
        }

        /// <summary>
        /// Gets a configuration of a message flow specified by its GUID.
        /// </summary>
        /// <param name="guid">GUID of the new message flow</param>
        /// <returns></returns>
        public MessageFlowConfiguration GetMessageFlow(Guid guid)
        {
            string xpath = string.Format("/configuration/messageflows/messageflow[@guid='{0}']", guid);
            XElement xMessageFlow = Content.XDocument.XPathSelectElement(xpath);

            // TODO: check for the solution of a situation when there is no
            // message flow at all

            //if (xMessageFlow == null)
            //{
            //    throw new ArgumentException(string.Format(
            //        "Cannot find message flow with GUID '{0}'.", guid), "guid");
            //}

            var result = XSerializer.Deserialize<MessageFlowConfiguration>(xMessageFlow);
            if (result == null)
            {
                // the message was empty
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

        /// <summary>
        /// Adds a configuration of a new message flow.
        /// </summary>
        /// <param name="messageFlow">new message flow configuration</param>
        public void AddMessageFlow(MessageFlowConfiguration messageFlow)
        {
            XElement xMessageFlow = new XElement(XName.Get("messageflow"));
            XSerializer.Serializer(messageFlow, xMessageFlow);
            xMessageFlow.SetAttributeValue(XName.Get("guid"), messageFlow.Guid);

            // TODO: what about the situation when there already exists a
            // message flow with this GUID

            Content.XDocument.XPathSelectElement("/configuration/messageflows").Add(xMessageFlow);
        }

        #endregion

        #region XML resource management

        /// <summary>
        /// Gets the content of the whole XML resource storage.
        /// </summary>
        /// <returns></returns>
        public XDocument GetXrmContent()
        {
            XElement xrmContent = Content.XDocument.XPathSelectElement("/configuration/xml-resource-storage");
            XDocument result = new XDocument();
            result.Add(xrmContent);
            return result;
        }

        /// <summary>
        /// Sets the content of the whole XML resource storage replacing the
        /// previous content.
        /// </summary>
        /// <returns></returns>
        public void SaveXrmContent(XElement xrmContent)
        {
            XElement oldXrmContent = Content.XDocument.XPathSelectElement("/configuration/xml-resource-storage");
            XElement xContainer = oldXrmContent.Parent;
            oldXrmContent.Remove();
            xContainer.Add(xrmContent);
        }

        #endregion
    }
}
