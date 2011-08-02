namespace DaemonNT.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using SchemaTron;

    /// <summary>
    /// Provides service configuration.
    /// </summary>
    internal static class ConfigProvider
    {        
        public static readonly string DefaultConfigFileName = "DaemonNT.xml";

        /// <summary>
        /// Loads the configuration for the specified service from a default
        /// XML configuration file and provides it in an object-model form.
        /// </summary>
        /// <remarks>
        /// The default file is searched in the current working directory.
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        /// <returns>Service configuration converted to an object model.
        /// </returns>
        /// <exception cref="InvalidOperationException" />
        public static ServiceSettings LoadServiceSettings(string serviceName)
        {
            return LoadServiceSettings(serviceName, null);
        }

        /// <summary>
        /// Loads the configuration for the specified service from a specified
        /// XML configuration file and provides it in an object-model form.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="configFile">Configuration file path (relative or
        /// absolute).</param>
        /// <returns>Service configuration converted to an object model.
        /// </returns>
        /// <exception cref="InvalidOperationException" />
        public static ServiceSettings LoadServiceSettings(string serviceName, string configFile)
        {
            XDocument xConfig = LoadRawConfiguration(configFile);

            // service
            XElement xService = xConfig.XPathSelectElement(string.Format("/config/service[@name='{0}']", serviceName));
            if (xService == null)
            {
                throw new InvalidOperationException(string.Format("The supplied service.name='{0}' is not specified in the configuration file.", serviceName));
            }

            ServiceSettings serviceSettings = DeserializeServiceSettings(xService);

            return serviceSettings;
        }

        /// <summary>
        /// Loads the configuration from a specified XML configuration file
        /// and provides it in an object-model form.
        /// </summary>
        /// <param name="configFile">Configuration file path (relative or
        /// absolute).</param>
        /// <returns>Configuration of all services converted to the object
        /// model.
        /// </returns>
        /// <exception cref="InvalidOperationException" />
        public static XDocument LoadRawConfiguration(string configFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {                
                configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigFileName);
            }
            XDocument xConfig = XDocument.Load(configFile, LoadOptions.SetLineInfo);

            ValidateConfiguration(xConfig, configFile);
            return xConfig;
        }

        public static Configuration LoadConfiguration(string configFile)
        {
            XDocument xmlConfig = LoadRawConfiguration(configFile);
            return ConfigurationFromXML(xmlConfig);
        }

        public static XDocument ConfigurationToXML(Configuration config)
        {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            XElement xConfig = new XElement("config");
            foreach (ServiceSettings service in config.Services)
            {
                xConfig.Add(SerializeServiceSettings(service));
            }
            XDocument xdoc = new XDocument();
            xdoc.Add(xConfig);

            return xdoc;
        }

        public static Configuration ConfigurationFromXML(XDocument xmlConfig)
        {
            var xmlServices = xmlConfig.XPathSelectElements("/config/service");
            List<ServiceSettings> services = new List<ServiceSettings>();
            foreach (var xmlService in xmlServices)
            {
                services.Add(DeserializeServiceSettings(xmlService));
            }
            Configuration config = new Configuration() { Services = services };
            return config;
        }

        /// <summary>
        /// Validates the input XML service configuration file.
        /// </summary>
        /// <param name="xConfig">XML service configuration file</param>
        public static bool IsValid(XDocument xConfig)
        {
            ValidatorResults results;
            IsValid(xConfig, out results);
            return results.IsValid;
        }

        /// <summary>
        /// Validates the input XML service configuration file providing
        /// detailed validation results.
        /// </summary>
        /// <param name="xConfig">XML service configuration file</param>
        /// <param name="results">Results of validation</param>
        public static bool IsValid(XDocument xConfig, out ValidatorResults results)
        {
            XDocument xSchema = Resources.Provider.LoadConfigSchema();

            // validation
            Validator validator = Validator.Create(xSchema);
            results = validator.Validate(xConfig, true);

            return results.IsValid;
        }

        /// <summary>
        /// Validates the input XML service configuration file.
        /// </summary>
        /// <remarks>
        /// If the file is invalid it throws an InvalidOperationException,
        /// otherwise return nothing.
        /// </remarks>
        /// <param name="xConfig">XML service configuration file</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ValidateConfiguration(XDocument xConfig, string configFile)
        {
            ValidatorResults results;
            if (!IsValid(xConfig, out results))
            {
                // create report
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("{0} is invalid. ", configFile));
                int i = 1;
                foreach (AssertionInfo info in results.ViolatedAssertions)
                {
                    sb.Append(string.Format(
                        "{0}. At the LineNumber={1}, LinePosition={2}, XPathLocation={3} '{4}' ",
                        i, info.LineNumber, info.LinePosition,
                        info.Location, info.UserMessage.Trim()));
                    i++;
                }

                throw new InvalidOperationException(sb.ToString());
            }
        }

        #region Deserialization

        private static ServiceSettings DeserializeServiceSettings(XElement xService)
        {
            ServiceSettings result = new ServiceSettings();
            // service.@name
            result.Name = xService.Attribute(XName.Get("name")).Value;

            // service.@type
            string typeClass;
            string typeAssembly;
            DeserializeType(xService, out typeClass, out typeAssembly);
            result.TypeClass = typeClass;
            result.TypeAssembly = typeAssembly;

            // service.settings
            XElement xSettings = xService.XPathSelectElement("./settings");
            result.Settings = DeserializeSettings(xSettings);

            // service.installer
            XElement xInstaller = xService.XPathSelectElement("./installer");
            result.InstallerSettings = DeserializeInstallerSettings(xInstaller);

            // service.trace-logger
            XElement xTraceLogger = xService.XPathSelectElement("./trace-logger");
            result.TraceLoggerSettings = DeserializeTraceLoggerSettings(xTraceLogger);

            return result;
        }

        private static InstallerSettings DeserializeInstallerSettings(XElement xInstaller)
        {
            InstallerSettings result = new InstallerSettings();

            if (xInstaller == null)
            {
                return result;
            }

            // description
            XElement xDescription = xInstaller.XPathSelectElement("./description");
            if (xDescription != null)
            {
                result.Description = xDescription.Value.Trim();
            }

            // startup-type
            XElement xStartType = xInstaller.XPathSelectElement("./start-type");
            if (xStartType != null)
            {
                result.StartMode = xStartType.Attribute(XName.Get("value")).Value.Trim();
            }

            // account
            XElement xAccount = xInstaller.XPathSelectElement("./account");
            if (xAccount != null)
            {
                result.Account = xAccount.Attribute(XName.Get("value")).Value.Trim();

                if (result.Account == "User")
                {
                    // username
                    XElement xUsername = xAccount.XPathSelectElement("./username");
                    result.User = xUsername.Value.Trim();

                    // password
                    XElement xPassword = xAccount.XPathSelectElement("./password");
                    result.Password = xPassword.Value.Trim();
                }
            }

            // depended-on
            XElement xDependedOn = xInstaller.XPathSelectElement("./depended-on");
            if (xDependedOn != null)
            {
                List<string> dependedNames = new List<string>();
                string[] tokens = xDependedOn.Value.Split(',');
                for (int i = 0; i < tokens.Length; i++)
                {
                    string name = tokens[i].Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        dependedNames.Add(name);
                    }
                }

                if (dependedNames.Count > 0)
                {
                    result.RequiredServices = dependedNames;
                }
            }

            return result;
        }

        private static TraceLoggerSettings DeserializeTraceLoggerSettings(XElement xTraceLogger)
        {
            TraceLoggerSettings result = new TraceLoggerSettings();

            if (xTraceLogger == null)
            {
                return result;
            }

            // @buffer-size
            XAttribute xTraceBufferSize = xTraceLogger.Attribute(XName.Get("buffer-size"));
            if (xTraceBufferSize != null)
            {
                result.BufferSize = Convert.ToInt32(xTraceBufferSize.Value);
            }

            // storage
            foreach (XElement xStorage in xTraceLogger.XPathSelectElements("./storage"))
            {
                TraceLoggerStorageSettings traceLogger = new TraceLoggerStorageSettings();

                // @name
                traceLogger.Name = xStorage.Attribute(XName.Get("name")).Value;

                // @type
                string className;
                string assemblyName;
                DeserializeType(xStorage, out className, out assemblyName);
                traceLogger.TypeClass = className;
                traceLogger.TypeAssembly = assemblyName;

                // settings
                XElement xSettings = xStorage.XPathSelectElement("./settings");
                traceLogger.Settings = DeserializeSettings(xSettings);

                result.Storages.Add(traceLogger);
            }

            return result;
        }

        private static void DeserializeType(XElement xElement, out string className, out string assemblyName)
        {
            className = null;
            assemblyName = null;

            string type = xElement.Attribute(XName.Get("type")).Value;

            // TODO:
            // - class name and assembly name shouldn't be squashed into a
            //   single XML attribute: <service type="FooType, FooAssembly.dll" />
            //   - instead an element inside <service> with two attributes
            //     could be used:
            // <service>
            //   <type class="FooType" assembly="FooAssembly.dll" />
            // </service>

            // resolve type value
            string[] tokens = type.Split(',');
            if (tokens.Length == 2)
            {
                className = tokens[0].Trim();
                assemblyName = tokens[1].Trim();
            }
            else if (tokens.Length == 1)
            {
                className = tokens[0].Trim();
                // NOTE: assemblyName is null!
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attribute '{0}' is invalid.", type));
            }
        }

        // TODO: refactor common code similarly to SerializeSectionBase()

        private static Settings DeserializeSettings(XElement xSettings)
        {
            Settings settings = new Settings();

            if (xSettings == null)
            {
                return settings;
            }

            foreach (var node in xSettings.XPathSelectElements("child::*"))
            {
                XElement xElement = (XElement)node;
                if (xElement.Name == "section")
                {
                    Sections section = new Sections();
                    settings[xElement.Attribute(XName.Get("name")).Value] = section;
                    DeserializeSection(section, xElement);
                }
                else
                {
                    settings.Parameters[xElement.Attribute(XName.Get("name")).Value] = xElement.Value;
                }
            }

            return settings;
        }

        private static void DeserializeSection(Sections section, XElement xElement)
        {
            // TODO: looks like the code is almost the same as in DeserializeSettings()
            foreach (var node in xElement.XPathSelectElements("child::*"))
            {
                XElement nextEle = (XElement)node;
                if (nextEle.Name == "section")
                {
                    Sections newSection = new Sections();
                    section[nextEle.Attribute(XName.Get("name")).Value] = newSection;
                    DeserializeSection(newSection, nextEle);
                }
                else
                {
                    section.Parameters[nextEle.Attribute(XName.Get("name")).Value] = nextEle.Value;
                }
            }
        }

        #endregion

        #region Serialization

        private static XElement SerializeServiceSettings(ServiceSettings service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            XElement xService = new XElement("service");

            xService.SetAttributeValue("name", service.Name);
            xService.SetAttributeValue("type", SerializeType(service.TypeClass, service.TypeAssembly));

            xService.Add(SerializeSettings(service.Settings));
            xService.Add(SerializeInstallerSettings(service.InstallerSettings));
            xService.Add(SerializeTraceLoggerSettings(service.TraceLoggerSettings));

            return xService;
        }

        private static XElement SerializeInstallerSettings(InstallerSettings installer)
        {
            XElement xInstaller = new XElement("installer");
            if (installer == null)
            {
                return xInstaller;
            }

            {
                XElement xDescription = new XElement("description");
                xDescription.Value = installer.Description;
                xInstaller.Add(xDescription);
            }

            {
                XElement xStartType = new XElement("start-type");
                xStartType.SetAttributeValue("value", installer.StartMode);
                xInstaller.Add(xStartType);
            }

            XElement xAccount = new XElement("account");
            xAccount.SetAttributeValue("value", installer.Account);
            if (installer.Account == "User")
            {
                XElement xUserName = new XElement("username");
                xUserName.Value = installer.User;
                xAccount.Add(xUserName);

                XElement xPassword = new XElement("password");
                xPassword.Value = installer.Password;
                xAccount.Add(xPassword);
            }
            xInstaller.Add(xAccount);

            {
                XElement xDependedOn = new XElement("depended-on");
                xDependedOn.Value = string.Join(",", installer.RequiredServices);
                xInstaller.Add(xDependedOn);
            }

            return xInstaller;
        }

        private static XElement SerializeTraceLoggerSettings(TraceLoggerSettings traceLogger)
        {
            XElement xTraceLogger = new XElement("trace-logger");
            if (traceLogger == null)
            {
                return xTraceLogger;
            }
            
            xTraceLogger.SetAttributeValue("buffer-size", traceLogger.BufferSize);
            foreach (var storage in traceLogger.Storages)
            {
                XElement xStorage = new XElement("storage");
                xStorage.SetAttributeValue("name", storage.Name);
                xStorage.SetAttributeValue("type",
                    SerializeType(storage.TypeClass, storage.TypeAssembly));

                XElement xSettings = SerializeSettings(storage.Settings);
                xStorage.Add(xSettings);
                xTraceLogger.Add(xStorage);
            }

            return xTraceLogger;
        }

        private static XElement SerializeSettings(Settings settings)
        {
            XElement xSettings = new XElement("settings");
            if (settings != null)
            {
                SerializeSectionBase(settings, xSettings);
            }
            return xSettings;
        }

        private static XElement SerializeSection(Sections section, string name)
        {
            XElement xSection = new XElement("section");
            xSection.SetAttributeValue("name", name);
            SerializeSectionBase(section, xSection);
            return xSection;
        }

        private static XElement SerializeSectionBase(SectionBase section, XElement xSectionBase)
        {
            foreach (string subSectionName in section.Keys)
            {
                Sections subSection = section[subSectionName];
                if (subSection != null)
                {
                    XElement xSubSection = SerializeSection(subSection, subSectionName);
                    xSectionBase.Add(xSubSection);
                }
            }

            foreach (string parameterKey in section.Parameters.Keys)
            {
                XElement xParam = new XElement("param");
                xParam.SetAttributeValue("name", parameterKey);
                xParam.Value = section.Parameters[parameterKey];
                xSectionBase.Add(xParam);
            }

            return xSectionBase;
        }

        private static string SerializeType(string className, string assemblyName)
        {
            className = (className != null) ? className.Trim() : null;
            assemblyName = (assemblyName != null) ? assemblyName.Trim() : null;
            return string.Join(",", className, assemblyName);
        }

        #endregion
    }
}
