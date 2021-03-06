﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    /// <summary>
    /// Provides configuration of objects in a unified way to enable
    /// persistence, injection and GUI editing.
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
    /// <seealso cref="ConfigurationItemAttribute"/>
    /// <seealso cref="ItemMetadata"/>
    public static class Configurator
    {
        internal static readonly XName XName_RootElement = XName.Get("objectConfig");
        internal static readonly XName XName_ItemElement = XName.Get("item");
        internal static readonly XName XName_ItemNameAttribute = XName.Get("name");

        private static List<ICustomConfigurationItemType> customItemTypes = new List<ICustomConfigurationItemType>();

        /// <summary>
        /// List of registered custom configurable item types.
        /// To register new type, just add a new item to this list.
        /// </summary>
        public static IList<ICustomConfigurationItemType> CustomItemTypes { get { return customItemTypes; } } 

        /// <summary>
        /// Gets values of configration items from object and serialize them into xml document.
        /// </summary>
        /// <param name="sourceObject">Object which values will be saved.</param>
        /// <returns>An xml document containing serialized values of configuration items.</returns>
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

        /// <summary>
        /// Deserializes values of configuration items from xml document and inject them into an object.
        /// </summary>
        /// <param name="targetObject">A target object where values will be set.</param>
        /// <param name="config">Source xml document.</param>
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

        /// <summary>
        /// Creates a WPF control for editing configuration items of given type.
        /// </summary>
        /// <param name="targetType">A type which configuration items will be edited.</param>
        /// <returns>A new item editor.</returns>
        public static ConfigurationEditor CreateEditor(Type targetType)
        {
            ClassMetadata classMetadata = new ClassMetadata(targetType);
            return CreateEditor(classMetadata);
        }

        /// <summary>
        /// Creates a WPF control for editing configuration items of given type.
        /// </summary>
        /// <param name="classMetadata">A description of a type which configuration items will be edited.</param>
        /// <returns>A new item editor.</returns>
        public static ConfigurationEditor CreateEditor(ClassMetadata classMetadata)
        {
            var result = new ConfigurationEditor(classMetadata);
            return result;
        }
    }
}
