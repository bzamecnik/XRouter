using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DaemonNT.Configuration;
using Xunit;

namespace DaemonNT.Test
{
    public class ConfigProviderTest
    {
        // TODO:
        // - good day:
        // - bad day:
        //   - make bad schemas according to asserts in config_schema.xml
        //     (about 65 asserts :) )
        //     - on the other hand we can trust SchemaTron

        #region Happy-day tests

        [Fact]
        public void LoadGoodFullConfigSingleService()
        {
            ServiceSettings settings = ConfigProvider.LoadServiceSettings(
                "TickingService",
                @"ConfigProviderFiles\GoodFullConfigSingleService.xml");

            Assert.NotNull(settings);
            Assert.Equal("DaemonNT.Test.TickingService", settings.TypeClass);
            Assert.Equal(@"DaemonNT.Test.dll", settings.TypeAssembly);

            #region Service settings

            var serviceSettings = settings.Settings;

            Assert.NotNull(serviceSettings);
            Assert.Equal("Service.1", serviceSettings.Parameters["SettingsParam"]);
            Assert.NotNull(serviceSettings["Section1"]);
            Assert.Equal("Service.2", serviceSettings["Section1"]
                .Parameters["Section1Param"]);
            Assert.NotNull(serviceSettings["Section1"]["InnerSection"]);
            Assert.Equal("Service.3", serviceSettings["Section1"]
                ["InnerSection"].Parameters["InnerSectionParam"]);

            #endregion

            #region Installer settings

            Assert.NotNull(settings.InstallerSettings);
            var installerSettings = settings.InstallerSettings;
            Assert.Equal("Service description", installerSettings.Description);
            Assert.Equal("User", installerSettings.Account);
            Assert.Equal("User name", installerSettings.User);
            Assert.Equal("Password", installerSettings.Password);
            Assert.Equal("Automatic", installerSettings.StartMode);

            Assert.NotNull(installerSettings.RequiredServices);
            var requiredServices = installerSettings.RequiredServices;
            Assert.Equal(3, requiredServices.Count());
            Assert.Contains("Service1", requiredServices);
            Assert.Contains("Service2", requiredServices);
            Assert.Contains("Service3", requiredServices);

            #endregion

            #region Trace-logger settings

            var loggerSettings = settings.TraceLoggerSettings;

            Assert.NotNull(loggerSettings);
            Assert.NotNull(loggerSettings.Storages);
            Assert.Equal(1, loggerSettings.Storages.Count);
            Assert.NotNull(loggerSettings.Storages[0]);

            var storageSettings = settings.TraceLoggerSettings.Storages[0].Settings;

            Assert.NotNull(storageSettings);
            Assert.Equal("TraceLoggerStorage.1", storageSettings.Parameters["SettingsParam"]);
            Assert.NotNull(storageSettings["Section1"]);
            Assert.Equal("TraceLoggerStorage.2", storageSettings["Section1"]
                .Parameters["Section1Param"]);
            Assert.NotNull(storageSettings["Section1"]["InnerSection"]);
            Assert.Equal("TraceLoggerStorage.3", storageSettings["Section1"]
                ["InnerSection"].Parameters["InnerSectionParam"]);

            #endregion
        }

        [Fact(Skip="No default file currently distributed.")]
        public void LoadGoodFullConfigFromDefaultFile()
        {
            // load from DaemonNT.xml
            // assumed to be in the base directory of the current appdomain
            ServiceSettings settings = ConfigProvider.LoadServiceSettings("MyServer");

            Assert.NotNull(settings);
        }

        [Fact]
        public void SerializeGoodFullConfigSingleService()
        {
            string origConfigFile = @"ConfigProviderFiles\GoodFullConfigSingleService.xml";
            string serializedConfigFile = @"ConfigProviderFiles\GoodFullConfigSingleService_serialized.xml";

            var origConfiguration = ConfigProvider.LoadConfiguration(origConfigFile);
            XDocument origConfigXml = XDocument.Load(origConfigFile);
            XDocument serializedConfigXml = ConfigProvider.ConfigurationToXML(origConfiguration);
            serializedConfigXml.Save(serializedConfigFile);

            Assert.True(ConfigProvider.IsValid(serializedConfigXml));

            Assert.Equal<XDocument>(origConfigXml, serializedConfigXml, new XmlHashCompare());
        }

        #endregion

        #region Bad-day tests

        [Fact]
        public void LoadConfigOfNonexistentService()
        {
            // should throw InvalidOperationException:
            // "The supplied service.name='BarService' is not specified in
            // ConfigProviderFiles\ConfigWithASingleEmptyService.xml."
            Assert.Throws<InvalidOperationException>(() =>
                ConfigProvider.LoadServiceSettings("BarService",
                    @"ConfigProviderFiles\ConfigWithASingleEmptyService.xml"));
        }

        [Fact]
        public void LoadNonexistentConfigFile()
        {
            Assert.Throws<System.IO.FileNotFoundException>(() =>
                ConfigProvider.LoadServiceSettings("FooService",
                    @"ConfigProviderFiles\NonexistentConfigFile.xml"));
        }

        #endregion

        #region Test helpers
        class XmlHashCompare : IEqualityComparer<XDocument>
        {
            public XmlHashCompare() { }

            public bool Equals(XDocument xdoc1, XDocument xdoc2)
            {
                return (GetXmlHash(xdoc1).SequenceEqual(GetXmlHash(xdoc2)));
            }

            public int GetHashCode(XDocument xdoc)
            {
                string s = Encoding.ASCII.GetString(GetXmlHash(xdoc));
                return s.GetHashCode();
            }

            public static byte[] GetXmlHash(XDocument xdoc)
            {
                XmlDsigC14NTransform xmlTransform = new XmlDsigC14NTransform(false);
                xmlTransform.LoadInput(ToXmlDocument(xdoc));
                return xmlTransform.GetDigestedOutput(new SHA1Managed());
            }

            private static XmlDocument ToXmlDocument(XDocument xDocument)
            {
                XmlDocument xmlDocument = new XmlDocument();
                using (XmlReader xmlReader = xDocument.CreateReader())
                {
                    xmlDocument.Load(xmlReader);
                }
                return xmlDocument;
            }
        }
        #endregion
    }
}
