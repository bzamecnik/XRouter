using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemTypes
{
    public abstract class ItemType
    {
        public string ClrTypeFullName { get; private set; }

        private Type clrTypeCache;

        protected ItemType(Type clrType)
        {
            ClrTypeFullName = clrType.FullName;

            clrTypeCache = clrType;
        }

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

        public abstract void WriteToXElement(XElement target, object value);
        public abstract object ReadFromXElement(XElement source);
        public abstract void WriteDefaultValueToXElement(XElement target);

        public static ItemType GetItemType(Type clrType)
        {
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
            throw new ArgumentException("Cannot convert CLR type to ItemType.", "clrType");
        }
    }
}
