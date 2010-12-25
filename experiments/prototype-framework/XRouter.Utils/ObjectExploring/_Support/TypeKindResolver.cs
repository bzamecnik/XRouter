using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.ObjectExploring
{
    public static class TypeKindResolver
    {
        public static TypeKind GetTypeKind(Type type)
        {
            if (type == null) {
                return TypeKind.Null;
            }

            if ((type.IsPrimitive) || (type == typeof(string))) {
                return TypeKind.Scalar;
            }

            if (type.IsArray) {
                return TypeKind.Array;
            }

            if ((type.IsClass) || (type.IsInterface) || (type.IsValueType)) {
                return TypeKind.Structured;
            }

            throw new Exception("Cannot determine type kind: " + type.ToString());
        }
    }
}
