using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace XRouter.Utils.ObjectExploring
{
    public class ObjectLinkInfo
    {
        public Type ContainerType { get; private set; }

        public FieldInfo ContainerField { get; private set; }

        public int[] ContainerArrayIndices { get; private set; }

        public bool IsInField { get { return ContainerField != null; } }
        public bool IsInArray { get { return ContainerArrayIndices != null; } }

        internal ObjectLinkInfo(Type containerType, FieldInfo containerField, int[] containerArrayIndices)
        {
            ContainerType = containerType;
            ContainerField = containerField;
            ContainerArrayIndices = containerArrayIndices;
        }
    }
}
