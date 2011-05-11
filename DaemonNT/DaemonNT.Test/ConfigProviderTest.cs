using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT.Configuration;
using Xunit;

namespace DaemonNT.Test
{
    public class ConfigProviderTest
    {
        // TODO:
        // - good day:
        //   - default config file
        // - bad day:
        //   - make bad schemas according to asserts in config_schema.xml
        //     (about 65 asserts :) )
        //   - non-existent config file

        #region Happy-day tests

        [Fact]
        public void LoadGoodFullConfigSingleService()
        {
            ServiceSettings settings = ConfigProvider.LoadServiceSettings(
                "FooService",
                @"ConfigProviderFiles\GoodFullConfigSingleService.xml");

            Assert.NotNull(settings);
            Assert.Equal("DaemonNT.FooService", settings.TypeClass);
            Assert.Equal(@"..\..\..\DaemonNT.Test\bin\Debug\DaemonNT.FooService.dll",
                settings.TypeAssembly);

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
    }
}
