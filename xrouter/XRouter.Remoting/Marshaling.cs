using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Concurrent;

namespace XRouter.Remoting
{
    class Marshaling
    {
        internal static string Marshal(object obj, Type type)
        {
            if (type.IsPrimitive) {
                return obj.ToString();
            }
            if (type == typeof(string)) {
                return (string)obj;
            }

            if (typeof(IRemotelyCloneable).IsAssignableFrom(type)) {
                var clonable = (IRemotelyCloneable)obj;
                string result = clonable.SerializeClone();
                return result;
            }

            if (typeof(IRemotelyReferable).IsAssignableFrom(type)) {
                if (!type.IsInterface) {
                    throw new ArgumentException("Formal type of remotely referable object must be interface.");
                }
                var remotelyReferable = (IRemotelyReferable)obj;
                var address = ObjectServer.PublishObject(remotelyReferable);
                string result = address.Serialize();
                return result;
            }

            if (typeof(Delegate).IsAssignableFrom(type)) {
                var target = (Delegate)obj;
                var invocable = new Invocable(target);
                var address = ObjectServer.PublishObject(invocable);
                string result = address.Serialize();
                return result;
            }

            throw new ArgumentException("Cannot marshal given type.");
        }

        internal static object Unmarshal(string marshaled, Type type)
        {
            if (type.IsPrimitive) {
                var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
                var result = parseMethod.Invoke(null, new object[] { marshaled });
                return result;
            }
            if (type == typeof(string)) {
                return marshaled;
            }

            if (typeof(IRemotelyCloneable).IsAssignableFrom(type)) {
                var result = (IRemotelyCloneable)FormatterServices.GetUninitializedObject(type);
                result.DeserializeClone(marshaled);
                return result;
            }

            if (typeof(IRemotelyReferable).IsAssignableFrom(type)) {
                var address = RemoteObjectAddress.Deserialize(marshaled);
                var result = RemoteObjectProxyProvider.GetProxy<IRemotelyReferable>(address, type);
                return result;
            }

            if (typeof(Delegate).IsAssignableFrom(type)) {
                var address = RemoteObjectAddress.Deserialize(marshaled);
                Invocable invocable = RemoteObjectProxyProvider.GetProxy<Invocable>(address);

                Delegate result = CreateDynamicDelegate(type, delegate(object[] arguments) {
                    object res = invocable.Invoke(arguments);
                    return res;
                });
                return result;
            }

            throw new ArgumentException("Cannot unmarshal given type.");
        }

        #region Reflection support

        internal static ConcurrentDictionary<int, object> delegateTargets = new ConcurrentDictionary<int, object>();

        private static Delegate CreateDynamicDelegate(Type delegateType, Func<object[], object> body)
        {
            int delegateTargetID = delegateTargets.Count + 1;
            while (!delegateTargets.TryAdd(delegateTargetID, body.Target)) {
                delegateTargetID++;
            }

            MethodInfo signature = delegateType.GetMethod("Invoke");
            Type[] parameterTypes = signature.GetParameters().Select(p => p.ParameterType).ToArray();
            DynamicMethod dynamicMethod = new DynamicMethod("DynamicMethod", signature.ReturnType, parameterTypes, DynamicMethod.GetCurrentMethod().Module);

            ILGenerator il = dynamicMethod.GetILGenerator();

            // Push delegate target object
            il.Emit(OpCodes.Ldsfld, typeof(Marshaling).GetField("delegateTargets", BindingFlags.NonPublic | BindingFlags.Static));
            il.Emit(OpCodes.Ldc_I4, delegateTargetID);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Callvirt, typeof(ConcurrentDictionary<int, object>).GetMethod("GetOrAdd"));

            // Push arguments
            il.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            for (int i = 0; i < parameterTypes.Length; i++) {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i);
                if (parameterTypes[i].IsValueType) {
                    il.Emit(OpCodes.Box, parameterTypes[i]);
                }
                il.Emit(OpCodes.Stelem_Ref);
            }

            // Call body
            il.Emit(OpCodes.Callvirt, body.Method);
            il.Emit(OpCodes.Ret);

            Delegate result = dynamicMethod.CreateDelegate(delegateType);
            return result;
        }

        #endregion

        private class Invocable : IRemotelyReferable
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
