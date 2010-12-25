using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.ObjectExploring
{
    public class ObjectInfo
    {
        private Object obj;

        public Type RealType { get; private set; }
        public TypeKind RealKind { get; private set; }

        public object Object
        {
            get { return obj; }
            internal set
            {
                obj = value;
                RealType = (obj != null) ? obj.GetType() : null;
                RealKind = TypeKindResolver.GetTypeKind(RealType);
            }
        }

        internal ObjectInfo(object obj)
        {
            Object = obj;
        }
    }
}
