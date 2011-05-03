using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// Internal support for marshalling objects (ie. serialization).
    /// </summary>
    internal class Marshalling
    {
        private static readonly string ArrayElementSeparator = "[5g#v&4k_x]";

        /// <summary>
        /// Converts an object of a given formal type to string
        /// representation which can be unmarshalled (converted back to
        /// object) on a target computer.
        /// </summary>
        /// <remarks>
        /// It supports objects of following (informal) categories/types:
        /// primitive, string, IRemotelyCloneable, IRemotelyReferable,
        /// delegate, System.Void, Serializable and array. A null object also
        /// can be marshalled.
        /// </remarks>
        /// <param name="obj">An object to be marshalled. Can be null.
        /// </param>
        /// <param name="type">Formal type of the object (only this type will
        /// be available on a target computer). Must not be null.</param>
        /// <returns>Marshalled string representation of given object.
        /// </returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException">If the object type is not
        /// supported for marshalling.</exception>
        internal static string Marshal(object obj, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // TODO: should the parameter 'type' with null value be supported?
            // If so it should represent the following automatic type
            // determination.
            if (type == typeof(object))
            {
                type = DetermineFormalTypeFromObject(obj);
            }

            if (type.IsPrimitive)
            {
                return obj.ToString();
            }

            if (type == typeof(string))
            {
                return (string)obj;
            }

            if (typeof(IRemotelyCloneable).IsAssignableFrom(type))
            {
                var clonable = (IRemotelyCloneable)obj;
                string result = clonable.SerializeClone();
                return result;
            }

            if (typeof(IRemotelyReferable).IsAssignableFrom(type))
            {
                if (!type.IsInterface)
                {
                    throw new ArgumentException("Formal type of remotely referable object must be interface.");
                }

                var remotelyReferable = (IRemotelyReferable)obj;
                var address = ObjectServer.PublishObject(remotelyReferable);
                string result = address.Serialize();
                return result;
            }

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                var target = (Delegate)obj;
                var invocable = new Invocable(target);
                var address = ObjectServer.PublishObject(invocable);
                string result = address.Serialize();
                return result;
            }

            if ((type.FullName == "System.Void") || (obj == null))
            {
                return string.Empty;
            }

            if (type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(ms, obj);
                    byte[] bytes = ms.GetBuffer();
                    int length = (int)ms.Length;
                    string result = length.ToString() + ":" + Convert.ToBase64String(bytes, 0, length);
                    return result;
                }
            }

            if (type.IsArray)
            {
                Array array = (Array)obj;
                if (array.Rank == 1)
                {
                    Type elementType = type.GetElementType();
                    StringBuilder result = new StringBuilder();
                    for (int i = 0; i < array.Length; i++)
                    {
                        object element = array.GetValue(i);
                        Type elementFormalType = DetermineFormalTypeFromObject(element);

                        result.Append(elementFormalType.ToString());
                        result.Append(ArrayElementSeparator);
                        result.Append(Marshal(element, elementType));
                        if (i < array.Length - 1)
                        {
                            result.Append(ArrayElementSeparator);
                        }
                    }

                    return result.ToString();
                }
            }

            throw new ArgumentException("Cannot marshal given type.");
        }

        /// <summary>
        /// Creates an object of a given type from its marshalled string representation.
        /// </summary>
        /// <param name="marshaled">Marshaled string</param>
        /// <param name="type">Type of object to create</param>
        /// <returns></returns>
        internal static object Unmarshal(string marshaled, Type type)
        {
            if (type.IsPrimitive)
            {
                var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
                var result = parseMethod.Invoke(null, new object[] { marshaled });
                return result;
            }

            if (type == typeof(string))
            {
                return marshaled;
            }

            if (typeof(IRemotelyCloneable).IsAssignableFrom(type))
            {
                var result = (IRemotelyCloneable)FormatterServices.GetUninitializedObject(type);
                result.DeserializeClone(marshaled);
                return result;
            }

            if (typeof(IRemotelyReferable).IsAssignableFrom(type))
            {
                var address = RemoteObjectAddress.Deserialize(marshaled);
                var result = RemoteObjectProxyProvider.GetProxy<IRemotelyReferable>(address, type);
                return result;
            }

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                var address = RemoteObjectAddress.Deserialize(marshaled);
                IInvocable invocable = RemoteObjectProxyProvider.GetProxy<IInvocable>(address);

                Delegate result = CreateDynamicDelegate(type,
                    delegate(object[] arguments)
                    {
                        object res = invocable.Invoke(arguments);
                        return res;
                    });
                return result;
            }

            if ((type.FullName == "System.Void") || ((type == typeof(object)) && (marshaled.Length == 0)))
            {
                return null;
            }

            if (type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
            {
                int colonPos = marshaled.IndexOf(':');
                int length = int.Parse(marshaled.Substring(0, colonPos));
                string base64 = marshaled.Substring(colonPos + 1);
                byte[] bytes = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream(bytes, 0, length))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    object result = binaryFormatter.Deserialize(ms);
                    return result;
                }
            }

            if (type.IsArray)
            {
                string[] marshaledElements = marshaled.Split(new[] { ArrayElementSeparator }, StringSplitOptions.None);
                object[] result = new object[marshaledElements.Length / 2];
                for (int i = 0; i < result.Length; i++)
                {
                    Type formalType = Type.GetType(marshaledElements[(i * 2) + 0]);
                    string marshaledElement = marshaledElements[(i * 2) + 1];
                    result[i] = Unmarshal(marshaledElements[i + 1], formalType);
                }

                return result;
            }

            throw new ArgumentException("Cannot unmarshal given type.");
        }

        private static Type DetermineFormalTypeFromObject(object obj)
        {
            Type result = typeof(object);
            if (obj != null)
            {
                Type realType = obj.GetType();
                if (typeof(IRemotelyReferable).IsAssignableFrom(realType))
                {
                    result = typeof(IRemotelyReferable);
                }
                else if (typeof(IRemotelyCloneable).IsAssignableFrom(realType))
                {
                    result = typeof(IRemotelyCloneable);
                }
                else if (realType.IsPrimitive || (realType == typeof(string)))
                {
                    result = realType;
                }
                else if (typeof(Delegate).IsAssignableFrom(realType))
                {
                    result = realType;
                }
            }

            return result;
        }

        #region Reflection support

        internal static ConcurrentDictionary<int, Func<object[], object>> delegatesBodies = new ConcurrentDictionary<int, Func<object[], object>>();

        private static Delegate CreateDynamicDelegate(Type delegateType, Func<object[], object> body)
        {
            int delegateTargetID = delegatesBodies.Count + 1;
            while (!delegatesBodies.TryAdd(delegateTargetID, body))
            {
                delegateTargetID++;
            }

            MethodInfo signature = delegateType.GetMethod("Invoke");
            Type[] parameterTypes = signature.GetParameters().Select(p => p.ParameterType).ToArray();
            DynamicMethod dynamicMethod = new DynamicMethod("DynamicMethod", signature.ReturnType, parameterTypes, DynamicMethod.GetCurrentMethod().Module);

            ILGenerator il = dynamicMethod.GetILGenerator();

            // Push delegate body:
            //   delegatesBodies.GetOrAdd(delegateTargetID, null)
            FieldInfo delegatesField = typeof(Marshalling).GetField("delegatesBodies", BindingFlags.NonPublic | BindingFlags.Static);
            il.Emit(OpCodes.Ldsfld, delegatesField);
            il.Emit(OpCodes.Ldc_I4, delegateTargetID);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Callvirt, typeof(ConcurrentDictionary<int, Func<object[], object>>).GetMethod("GetOrAdd", new[] { typeof(int), typeof(Func<object[], object>) }));

            // Push arguments
            il.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i);
                if (parameterTypes[i].IsValueType)
                {
                    il.Emit(OpCodes.Box, parameterTypes[i]);
                }
                il.Emit(OpCodes.Stelem_Ref);
            }

            // Call body
            il.Emit(OpCodes.Callvirt, typeof(Func<object[], object>).GetMethod("Invoke"));

            if (signature.ReturnType.FullName == "System.Void")
            {
                il.Emit(OpCodes.Pop);
            }

            il.Emit(OpCodes.Ret);

            Delegate result = dynamicMethod.CreateDelegate(delegateType);
            return result;
        }

        #endregion

        private class Invocable : IInvocable
        {
            private Delegate target;

            public Invocable(Delegate target)
            {
                this.target = target;
            }

            public object Invoke(object[] arguments)
            {
                object result = target.DynamicInvoke(arguments);
                return result;
            }
        }
    }
}
