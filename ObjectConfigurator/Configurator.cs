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
            Type type = sourceObject.GetType();
            XElement xConfig = new XElement(XName_RootElement);

            IEnumerable<ItemMetadata> items = GetItemsMetadata(type);
            foreach (ItemMetadata item in items) {
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
            Type type = targetObject.GetType();
            IEnumerable<ItemMetadata> items = GetItemsMetadata(type);

            var xItems = config.Root.Elements(XName_ItemElement);
            foreach (XElement xItem in xItems) {
                string name = xItem.Attribute(XName_ItemNameAttribute).Value;
                ItemMetadata item = items.FirstOrDefault(i => i.Name == name);
                if (item != null) {
                    object value = item.ReadFromXElement(xItem);
                    item.SetValue(targetObject, value);
                }
            }
        }

        public static ConfigurationEditor CreateEditor(Type targetType)
        {
            var result = new ConfigurationEditor(targetType);
            return result;
        }

        internal static IEnumerable<ItemMetadata> GetItemsMetadata(Type type)
        {
            var result = new List<ItemMetadata>();

            var instanceMembers = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo member in instanceMembers) {
                var attributes = (ConfigurationItemAttribute[])member.GetCustomAttributes(typeof(ConfigurationItemAttribute), true);
                var attribute = attributes.SingleOrDefault();
                if (attribute != null) {
                    if (member is FieldInfo) {
                        var field = (FieldInfo)member;
                        var itemMetadata = new ItemMetadata(field, attribute);
                        result.Add(itemMetadata);
                    } else if (member is PropertyInfo) {
                        var property = (PropertyInfo)member;
                        var itemMetadata = new ItemMetadata(property, attribute);
                        result.Add(itemMetadata);
                    }
                }
            }

            return result;
        }
    }
}
