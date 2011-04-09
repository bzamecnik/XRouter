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
    /// Poskytuje pristup ke konfiguraci.
    /// 
    /// TODO: Je velmi pravdepodobne, ze se bude konfigurace sluzby rozsirovat (mozna logovani, 
    /// ale urcite watchdog apod.)
    /// </summary>
    internal static class ConfigProvider
    {
        private static readonly string configFile = "DaemonNT.xml";

        /// <summary>
        /// Ze vstupniho XML souboru a dane service-name vraci jeji nastaveni jako objekt. 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ServiceSetting LoadServiceSetting(string serviceName)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            XDocument xConfig = XDocument.Load(configPath, LoadOptions.SetLineInfo);

            Validation(xConfig);

            // service
            XElement xService = xConfig.XPathSelectElement(string.Format("/config/service[@name='{0}']", serviceName));
            if (xService == null)
            {
                throw new InvalidOperationException(string.Format("The supplied service.name='{0}' is not specified in {1}.", serviceName, configFile));
            }

            ServiceSetting serviceSetting = DeserializeServiceSetting(xService);

            return serviceSetting;
        }

        /// <summary>
        /// Provede validaci vstupniho XML souboru. 
        /// 
        /// V projektu je definovano Schematron schema, ktere specifikuje pozadavky na dany XML soubor. 
        /// </summary>
        /// <param name="xConfig"></param>
        private static void Validation(XDocument xConfig)
        {
            XDocument xSchema = Resources.Provider.LoadConfigSchema();

            // validation
            Validator validator = Validator.Create(xSchema);
            ValidatorResults results = validator.Validate(xConfig, true);

            if (!results.IsValid)
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

        private static ServiceSetting DeserializeServiceSetting(XElement xService)
        {
            ServiceSetting result = new ServiceSetting();

            // service.@type
            string typeClass;
            string typeAssembly;
            DeserializeType(xService, out typeClass, out typeAssembly);
            result.TypeClass = typeClass;
            result.TypeAssembly = typeAssembly;

            // service.setting
            XElement xSetting = xService.XPathSelectElement("./setting");
            if (xSetting == null)
            {
                result.Setting = new Setting();
            }
            else
            {
                result.Setting = DeserializeSetting(xSetting);
            }

            // service.installer
            XElement xInstaller = xService.XPathSelectElement("./installer");
            if (xInstaller == null)
            {
                result.InstallerSetting = new InstallerSetting();
            }
            else
            {
                result.InstallerSetting = DeserializeInstallerSetting(xInstaller);
            }

            return result;
        }

        private static InstallerSetting DeserializeInstallerSetting(XElement xInstaller)
        {
            InstallerSetting result = new InstallerSetting();

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

        private static void DeserializeType(XElement xElement, out string className, out string assemblyName)
        {
            className = null;
            assemblyName = null;

            string type = xElement.Attribute(XName.Get("type")).Value;

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
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attribute '{0}' is invalid.", type));
            }
        }

        private static Setting DeserializeSetting(XElement xSetting)
        {
            Setting setting = new Setting();
            foreach (var node in xSetting.XPathSelectElements("child::*"))
            {
                XElement xElement = (XElement)node;
                if (xElement.Name == "section")
                {
                    Section section = new Section();
                    setting[xElement.Attribute(XName.Get("name")).Value] = section;
                    DeserializeSection(section, xElement);
                }
                else
                {
                    setting.Param[xElement.Attribute(XName.Get("name")).Value] = xElement.Value;
                }
            }

            return setting;
        }

        private static void DeserializeSection(Section section, XElement xElement)
        {
            foreach (var node in xElement.XPathSelectElements("child::*"))
            {
                XElement nextEle = (XElement)node;
                if (nextEle.Name == "section")
                {
                    Section newSection = new Section();
                    section[nextEle.Attribute(XName.Get("name")).Value] = newSection;
                    DeserializeSection(newSection, nextEle);
                }
                else
                {
                    section.Param[nextEle.Attribute(XName.Get("name")).Value] = nextEle.Value;
                }
            }
        }
    }
}
