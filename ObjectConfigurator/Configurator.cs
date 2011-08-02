using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using ObjectConfigurator.ItemTypes;

namespace ObjectConfigurator
{
    /// <summary>
    /// Provides configuration of objects in a unified way to enable
    /// persistence and GUI editing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By configuration is meant the state of class instance fields and
    /// properties. Each such a field or property (further refered to as
    /// a configuration item) annotated by the [ConfigurationItem] attribute
    /// can processed by Configurator. Along with some lower-level information
    /// about the items the whole information about an item is called item
    /// metadata. It is captured by reflection.
    /// </para>
    /// <para>
    /// The [ConfigurationItem] attribute, besides marking which item to
    /// work with, can also hold the user-friendly information about the item,
    /// such as a name and description which can be shown within a GUI editor.
    /// </para>
    /// <para>
    /// Configurator can export the item metadata to XML and import it back.
    /// Item metadata can also be edited in a GUI editor.
    /// </para>
    /// </remarks>
    /// <see cref="ConfigurationItemAttribute"/>
    /// <see cref="ItemMetadata"/>
    public static class Configurator
    {
        internal static readonly XName XName_RootElement = XName.Get("objectConfig");
        internal static readonly XName XName_ItemElement = XName.Get("item");
        internal static readonly XName XName_ItemNameAttribute = XName.Get("name");

        private static List<ICustomConfigurationItemType> customItemTypes = new List<ICustomConfigurationItemType>();
        public static IList<ICustomConfigurationItemType> CustomItemTypes { get { return customItemTypes; } } 

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
            System.Diagnostics.Debug.Assert(config != null);
            System.Diagnostics.Debug.Assert(config.Root != null);

            Type targetType = targetObject.GetType();
            ClassMetadata classMetadata = new ClassMetadata(targetType);

            List<ItemMetadata> unsetItems = new List<ItemMetadata>(classMetadata.ConfigurableItems);

            var xItems = config.Root.Elements(XName_ItemElement);
            foreach (XElement xItem in xItems) {
                string name = xItem.Attribute(XName_ItemNameAttribute).Value;
                ItemMetadata item = classMetadata.ConfigurableItems.FirstOrDefault(i => i.Name == name);
                if (item != null) {
                    unsetItems.Remove(item);
                    object value = item.ReadFromXElement(xItem);
                    item.SetValue(targetObject, value);
                }
            }

            #region Set default values for unset items
            foreach (ItemMetadata item in unsetItems) {
                object defaultValue;

                defaultValue = item.GetDefaultValue();

                item.SetValue(targetObject, defaultValue);
            }
            #endregion
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
