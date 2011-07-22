﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using ObjectConfigurator.ValueValidators;

namespace ObjectConfigurator
{
    public class ClassMetadata
    {
        public string ClrTypeFullName { get; private set; }

        public ReadOnlyCollection<ItemMetadata> ConfigurableItems { get; private set; }

        private Type cachedClrType;

        public ClassMetadata(Type clrType)
        {
            ClrTypeFullName = clrType.FullName;
            cachedClrType = clrType;

            ConfigurableItems = new ReadOnlyCollection<ItemMetadata>(GetItemsMetadata(clrType).ToArray());
        }

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
