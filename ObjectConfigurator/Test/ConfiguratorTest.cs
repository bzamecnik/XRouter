using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ObjectConfigurator;
using XRouter.Gateway;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ObjectConfigurator.Test
{
    public class ConfiguratorTest
    {
        [Fact]
        public void ConcurrentDictionaryWithDefaultValues()
        {
            var instance = new ConfigurableClass();
            Configurator.LoadConfiguration(instance, XDocument.Parse("<objectConfig />"));
            
            Assert.Equal(new[] { "OutA", "OutB", "OutC" }, CollectionToSortedArray(instance.endpointDict.Keys));
            Assert.Equal(new[] {
                @"C:\XRouterTest\OutA",
                @"C:\XRouterTest\OutB",
                @"C:\XRouterTest\OutC"
                }, CollectionToSortedArray(instance.endpointDict.Values));
        }

        private T[] CollectionToSortedArray<T>(ICollection<T> collection)
        {
            List<T> list = collection.ToList();
            list.Sort();
            return list.ToArray();
        }

        class ConfigurableClass
        {
            [ConfigurationItem("Output directories", null, new[] { 
            "OutA", @"C:\XRouterTest\OutA",
            "OutB", @"C:\XRouterTest\OutB",
            "OutC", @"C:\XRouterTest\OutC"
            })]
            public ConcurrentDictionary<string, string> endpointDict = new ConcurrentDictionary<string, string>();
        }
    }
}
