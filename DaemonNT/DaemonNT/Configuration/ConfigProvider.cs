using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.XPath;
using System.Xml.Linq;
using SchemaTron;

namespace DaemonNT.Configuration
{
    /// <summary>
    /// Poskytuje pristup ke konfiguraci.
    /// 
    /// TODO: Je velmi pravdepodobne, ze se bude konfigurace sluzby rozsirovat (mozna logovani, 
    /// ale urcite watchdog apod.)
    /// </summary>
    internal static class ConfigProvider
    {
        private static readonly String configFile = "DaemonNT.xml";

        /// <summary>
        /// Ze vstupniho XML souboru a dane service-name vraci jeji nastaveni jako objekt. 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ServiceSetting LoadServiceSetting(String serviceName)
        {         
            String configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            XDocument xConfig = XDocument.Load(configPath, LoadOptions.SetLineInfo);

            Validation(xConfig);
            
            // service
            XElement xService = xConfig.XPathSelectElement(String.Format("/config/service[@name='{0}']", serviceName));
            if (xService == null)
            {
                throw new InvalidOperationException(String.Format("The supplied service.name='{0}' is not specified in {1}.", serviceName, configFile));
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
                sb.Append(String.Format("{0} is invalid. ", configFile));
                Int32 i = 1;
                foreach (AssertionInfo info in results.ViolatedAssertions)
                {
                    sb.Append(String.Format("{0}. At the LineNumber={1}, LinePosition={2}, XPathLocation={3} '{4}' ", i, info.LineNumber, info.LinePosition,
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
            String typeClass;
            String typeAssembly;
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
                    result.Pwd = xPassword.Value.Trim();
                }
            }

            // depended-on
            XElement xDependedOn = xInstaller.XPathSelectElement("./depended-on");            
            if (xDependedOn != null)
            {
                List<String> dependedNames = new List<String>();
                String[] tokens = xDependedOn.Value.Split(',');                
                for (Int32 i = 0; i < tokens.Length; i++)
                {
                    String name = tokens[i].Trim();
                    if (!String.IsNullOrEmpty(name))
                    {
                        dependedNames.Add(name);
                    }
                }
                if (dependedNames.Count > 0)
                {
                    result.DependentOn = dependedNames.ToArray();
                }
            }

            return result;
        }

        private static void DeserializeType(XElement xEle, out String ClassName, out String AssemblyName)
        {
            ClassName = null;
            AssemblyName = null;

            String type = xEle.Attribute(XName.Get("type")).Value;

            // resolve type value
            String[] tokens = type.Split(',');
            if (tokens.Length == 2)
            {
                ClassName = tokens[0].Trim();
                AssemblyName = tokens[1].Trim();
            }
            else
                if (tokens.Length == 1)
                {
                    ClassName = tokens[0].Trim();
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Attribute '{0}' is invalid.", type));
                }
        }

        private static Setting DeserializeSetting(XElement xSetting)
        {
            Setting setting = new Setting();
            foreach (var node in xSetting.XPathSelectElements("child::*"))
            {
                XElement xEle = (XElement)node;
                if (xEle.Name == "section")
                {
                    Section section = new Section();
                    setting[xEle.Attribute(XName.Get("name")).Value] = section;
                    DeserializeSection(section, xEle);
                }
                else
                {
                    setting.Param[xEle.Attribute(XName.Get("name")).Value] = xEle.Value;
                }
            }
            return setting;
        }

        private static void DeserializeSection(Section section, XElement xEle)
        {
            foreach (var node in xEle.XPathSelectElements("child::*"))
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
