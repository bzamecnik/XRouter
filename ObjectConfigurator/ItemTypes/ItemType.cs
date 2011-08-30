using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemTypes
{
    /// <summary>
    /// Describes type of configuration item.
    /// </summary>
    [DataContract]
    public abstract class ItemType
    {
        /// <summary>
        /// Full name of CLR type.
        /// </summary>
        [DataMember]
        public string ClrTypeFullName { get; private set; }

        private Type clrTypeCache;

        protected ItemType(Type clrType)
        {
            ClrTypeFullName = clrType.FullName;

            clrTypeCache = clrType;
        }

        /// <summary>
        /// Provides CLR type of configuration item.
        /// </summary>
        /// <returns>CLR type.</returns>
        public Type GetClrType()
        {
            if (clrTypeCache == null) {
                clrTypeCache = ClassMetadata.GetClrTypeByFullName(ClrTypeFullName);
            }
            return clrTypeCache;
        }

        internal object CreateInstance()
        {
            Type clrType = GetClrType();
            if (clrType == typeof(string)) {
                return string.Empty;
            }

            object result = Activator.CreateInstance(clrType, true);
            return result;
        }

        /// <summary>
        /// Writes given value into a content of an xml element.
        /// </summary>
        /// <param name="target">Target xml element.</param>
        /// <param name="value">Value to be written in xml element.</param>
        public abstract void WriteToXElement(XElement target, object value);

        /// <summary>
        /// Reads a value from a content of an xml element.
        /// </summary>
        /// <param name="source">Source xml element.</param>
        /// <returns>Read value.</returns>
        public abstract object ReadFromXElement(XElement source);

        /// <summary>
        /// Writes default value of this configuration item type into a content of xml element.
        /// </summary>
        /// <param name="target">Target xml element.</param>
        public abstract void WriteDefaultValueToXElement(XElement target);

        /// <summary>
        /// Creates a concrete configuration item type description of given CLR type.
        /// </summary>
        /// <param name="clrType">CLR type.</param>
        /// <returns>Configuration item type description.</returns>
        public static ItemType GetItemType(Type clrType)
        {
            ICustomConfigurationItemType customType = Configurator.CustomItemTypes.FirstOrDefault(r => r.AcceptType(clrType.FullName));
            if (customType != null) {
                return new CustomItemType(clrType);
            }

            if (BasicItemType.BasicClrTypes.Contains(clrType)) {
                return new BasicItemType(clrType);
            }
            if (clrType.IsEnum) {
                IEnumerable<string> valueNames = Enum.GetValues(clrType).OfType<object>().Select(v => v.ToString());
                return new EnumItemType(clrType, valueNames);
            }
            {
                Type dictionaryInterface = clrType.GetInterface(typeof(IDictionary<,>).FullName);
                if (dictionaryInterface != null) {
                    Type[] genericArgs = dictionaryInterface.GetGenericArguments();
                    Type keyClrType = genericArgs[0];
                    Type valueClrType = genericArgs[1];
                    ItemType keyItemType = GetItemType(keyClrType);
                    ItemType valueItemType = GetItemType(valueClrType);
                    return new DictionaryItemType(clrType, keyItemType, valueItemType);
                }
            }
            {
                Type collectionInterface = clrType.GetInterface(typeof(ICollection<>).FullName);
                if (collectionInterface != null) {
                    Type[] genericArgs = collectionInterface.GetGenericArguments();
                    Type elementClrType = genericArgs[0];
                    ItemType elementItemType = GetItemType(elementClrType);
                    return new CollectionItemType(clrType, elementItemType);
                }
            }
            throw new ArgumentException(string.Format("Cannot convert CLR type {0} to ItemType.", clrType.FullName), "clrType");
        }
    }
}
