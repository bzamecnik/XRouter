using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ObjectConfigurator.ValueValidators;

namespace ObjectConfigurator
{
    /// <summary>
    /// Describes configurable items of a certain class.
    /// </summary>
    [DataContract]
    public class ClassMetadata
    {
        /// <summary>
        /// Full name of a class which is described by this instance.
        /// </summary>
        [DataMember]
        public string ClrTypeFullName { get; private set; }

        /// <summary>
        /// Descriptions of all configurable items.
        /// </summary>
        [DataMember]
        public ReadOnlyCollection<ItemMetadata> ConfigurableItems { get; private set; }

        private Type cachedClrType;

        /// <summary>
        /// Construct a metadata description for given class.
        /// </summary>
        /// <param name="clrType">A class to be described by constructed instance.</param>
        public ClassMetadata(Type clrType)
        {
            ClrTypeFullName = clrType.FullName;
            cachedClrType = clrType;

            ConfigurableItems = new ReadOnlyCollection<ItemMetadata>(GetItemsMetadata(clrType).ToArray());
        }

        /// <summary>
        /// Provides CLR type object of class described by this instance.
        /// </summary>
        /// <returns>CLR type object of class described by this instance.</returns>
        public Type GetClrType()
        {
            if (cachedClrType == null) {
                cachedClrType = GetClrTypeByFullName(ClrTypeFullName);
            }
            return cachedClrType;
        }

        private IEnumerable<ItemMetadata> GetItemsMetadata(Type type)
        {
            var result = new List<ItemMetadata>();
            var instanceMembers = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo member in instanceMembers) {
                var configItemAttribute = (ConfigurationItemAttribute)member.GetCustomAttributes(typeof(ConfigurationItemAttribute), true).SingleOrDefault();
                if (configItemAttribute != null) {
                    var validatorAttributes = (ValueValidatorAttribute[])member.GetCustomAttributes(typeof(ValueValidatorAttribute), true);
                    if (member is FieldInfo) {
                        var field = (FieldInfo)member;
                        var itemMetadata = new ItemMetadata(this, field, configItemAttribute, validatorAttributes);
                        result.Add(itemMetadata);
                    } else if (member is PropertyInfo) {
                        var property = (PropertyInfo)member;
                        var itemMetadata = new ItemMetadata(this, property, configItemAttribute, validatorAttributes);
                        result.Add(itemMetadata);
                    }
                }
            }
            return result;
        }

        internal static Type GetClrTypeByFullName(string clrTypeFullName)
        {
            Type result = Type.GetType(clrTypeFullName, true);
            return result;
        }
    }
}
