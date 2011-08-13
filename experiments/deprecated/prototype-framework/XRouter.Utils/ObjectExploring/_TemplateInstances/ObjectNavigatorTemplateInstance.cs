using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using XRouter.Utils.DesignTemplates.NavigatedExploring.Templating;

namespace XRouter.Utils.ObjectExploring
{
    class ObjectNavigatorTemplateInstance : NavigatorTemplate<ObjectInfo, ObjectLinkInfo>
    {
        internal override IEnumerable<LinkTarget<ObjectInfo, ObjectLinkInfo>> GetForwardLinkTargets()
        {
            switch (CurrentLocation.RealKind) {
                case TypeKind.Null:
                case TypeKind.Scalar:
                    yield break;

                case TypeKind.Array:
                    Array array = (Array)CurrentLocation.Object;
                    Type elementType = CurrentLocation.RealType.GetElementType();
                    foreach (int[] indices in array.GetAllIndices()) {
                        object targetObject = array.GetValue(indices);
                        var info = new ObjectInfo(targetObject);
                        var link = new ObjectLinkInfo(elementType, null, indices);
                        yield return new LinkTarget<ObjectInfo, ObjectLinkInfo>(info, link);
                    }
                    yield break;

                case TypeKind.Structured:
                    BindingFlags allInstanceFields = (BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) & (~BindingFlags.Static);
                    Type type = CurrentLocation.RealType;
                    while (type != typeof(object)) {
                        foreach (FieldInfo fieldInfo in type.GetFields(allInstanceFields)) {
                            Type fieldType = fieldInfo.FieldType;
                            object fieldValue = fieldInfo.GetValue(CurrentLocation.Object);
                            var info = new ObjectInfo(fieldValue);
                            var link = new ObjectLinkInfo(fieldType, fieldInfo, null);
                            yield return new LinkTarget<ObjectInfo, ObjectLinkInfo>(info, link);
                        }
                        type = type.BaseType;
                    }
                    yield break;
            }
        }
    }
}
