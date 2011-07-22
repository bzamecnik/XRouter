using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectConfigurator
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

        public abstract void WriteToXElement(XElement target, object value);
        public abstract object ReadFromXElement(XElement source);

        public static ItemType GetItemType(Type clrType)
        {
            if (BasicItemType.BasicClrTypes.Contains(clrType)) {
                return new BasicItemType(clrType);
            }
            if (clrType.IsEnum) {
                IEnumerable<string> valueNames = Enum.GetValues(clrType).OfType<object>().Select(v => v.ToString());
                return new EnumItemType(clrType, valueNames);
            }
            if (typeof(IDictionary<,>).IsAssignableFrom(clrType)) {
                Type dictionaryInterface = clrType.GetInterface(typeof(IDictionary<,>).FullName);
                Type[] genericArgs = dictionaryInterface.GetGenericArguments();
                Type keyClrType = genericArgs[0];
                Type valueClrType = genericArgs[1];
                ItemType keyItemType = GetItemType(keyClrType);
                ItemType valueItemType = GetItemType(valueClrType);
                return new DictionaryItemType(clrType, keyItemType, valueItemType);
            }
            if (typeof(ICollection<>).IsAssignableFrom(clrType)) {
                Type collectionInterface = clrType.GetInterface(typeof(ICollection<>).FullName);
                Type[] genericArgs = collectionInterface.GetGenericArguments();
                Type elementClrType = genericArgs[0];
                ItemType elementItemType = GetItemType(elementClrType);
                return new CollectionItemType(clrType, elementItemType);
            }
            throw new ArgumentException("Cannot convert CLR type to ItemType.", "clrType");
        }
    }
}
