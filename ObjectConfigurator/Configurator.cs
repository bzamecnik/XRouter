using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace ObjectConfigurator
{
    public static class Configurator
    {
        internal static readonly XName XName_RootElement = XName.Get("objectConfig");
        internal static readonly XName XName_ItemElement = XName.Get("item");
        internal static readonly XName XName_ItemNameAttribute = XName.Get("name");

        public static XDocument SaveConfiguration(object sourceObject)
        {
            Type targetType = sourceObject.GetType();
            ClassMetadata classMetadata = new ClassMetadata(targetType);

            XElement xConfig = new XElement(XName_RootElement);

            foreach (ItemMetadata item in classMetadata.ConfigurableItems) {
                XElement xItem = new XElement(XName_ItemElement);
                xItem.SetAttributeValue(XName_ItemNameAttribute, item.Name);
                object value = item.GetValue(sourceObject);
                item.WriteToXElement(xItem, value);
                xConfig.Add(xItem);
            }

            XDocument result = new XDocument();
            result.Add(xConfig);
            return result;
        }

        public static void LoadConfiguration(object targetObject, XDocument config)
        {
            Type targetType = targetObject.GetType();
            ClassMetadata classMetadata = new ClassMetadata(targetType);

            var xItems = config.Root.Elements(XName_ItemElement);
            foreach (XElement xItem in xItems) {
                string name = xItem.Attribute(XName_ItemNameAttribute).Value;
                ItemMetadata item = classMetadata.ConfigurableItems.FirstOrDefault(i => i.Name == name);
                if (item != null) {
                    object value = item.ReadFromXElement(xItem);
                    item.SetValue(targetObject, value);
                }
            }
        }

        public static ConfigurationEditor CreateEditor(Type targetType)
        {
            ClassMetadata classMetadata = new ClassMetadata(targetType);
            return CreateEditor(classMetadata);
        }

        public static ConfigurationEditor CreateEditor(ClassMetadata classMetadata)
        {
            var result = new ConfigurationEditor(classMetadata);
            return result;
        }
    }
}
