using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.Reflection
{
    public static class ReflectionUtils
    {
        public static string ToGenericTypeString(this Type type)
        {
            if (!type.IsGenericType) {
                return type.Name;
            }

            string genericTypeName = type.Name;
            int backTickPos = genericTypeName.IndexOf('`');
            if (backTickPos >= 0) {
                genericTypeName = genericTypeName.Substring(0, backTickPos);
            }

            string genericArgs = string.Join(",", 
                type.GetGenericArguments().Select(ta => ToGenericTypeString(ta)).ToArray());

            string result = string.Format("{0}<{1}>", genericTypeName, genericArgs);
            return result;
        }
    }
}
