using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    /// <summary>
    /// A provider of proxies for remote objects.
    /// </summary>
    public static class RemoteObjectProxyProvider
    {
        private static object dataLock = new object();
        private static Dictionary<RemoteObjectAddress, object> cache = new Dictionary<RemoteObjectAddress, object>();

        private static Castle.DynamicProxy.ProxyGenerator proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();

        /// <summary>
        /// Creates a proxy for given remote object.
        /// </summary>
        /// <remarks>
        /// If a target object is on the same process, a real object is returned instead of proxy.
        /// </remarks>
        /// <typeparam name="T">An interface which proxy will implement.</typeparam>
        /// <param name="address">Address of a target remote object.</param>
        /// <param name="requiredInterface">An interface which proxy will implement.</param>
        /// <returns>Proxy to a remote object which implements given interface.</returns>
        public static T GetProxy<T>(RemoteObjectAddress address, Type requiredInterface = null)
            where T : IRemotelyReferable
        {
            if (requiredInterface == null) {
                requiredInterface = typeof(T);
            }
            if ((!requiredInterface.IsInterface) || (!typeof(T).IsInterface)) {
                throw new InvalidOperationException("Parameter requiredInterface and type argument T must be interface.");
            }

            object proxyObject = InternalGetProxy(address, requiredInterface);
            if (!requiredInterface.IsAssignableFrom(proxyObject.GetType())) {
                throw new ArgumentException("Required interface does not match.", "requiredInterface");
            }

            T proxy = (T)proxyObject;
            return proxy;
        }

        private static object InternalGetProxy(RemoteObjectAddress address, Type requiredInterface)
        {
            if (address.ServerGuid == ObjectServer.ServerGuid) {
                lock (ObjectServer.DataLock) {
                    object localObject = ObjectServer.PublishedObjects[address.ObjectID];
                    return localObject;
                }
            }

            lock (dataLock) {
                if (cache.ContainsKey(address)) {
                    object cachedProxy = cache[address];
                    return cachedProxy;
                }
            }

            var interceptor = new ProxyInterceptor(address);
            object proxy;
            lock (dataLock) {
                proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget(requiredInterface, interceptor);
            }

            lock (dataLock) {
                cache.Add(address, proxy);
            }
            return proxy;
        }

        private class ProxyInterceptor : Castle.DynamicProxy.IInterceptor
        {
            public RemoteObjectAddress TargetAddress { get; private set; }

            private IClient Client { get; set; }

            public ProxyInterceptor(RemoteObjectAddress targetAddress)
            {
                TargetAddress = targetAddress;
                Client = new TcpClient(targetAddress.ServerAddress);
            }

            public void Intercept(Castle.DynamicProxy.IInvocation invocation)
            {
                var data = new List<string>();

                data.Add(TargetAddress.ObjectID.ToString());
                data.Add(invocation.Method.Name);

                var parametersInfo = invocation.Method.GetParameters();
                var parameterTypes = new List<string>();
                foreach (var parameter in parametersInfo) {
                    string parameterType = string.Format("{0}!{1}", parameter.ParameterType.Assembly.FullName, parameter.ParameterType.FullName);
                    parameterTypes.Add(parameterType);
                }
                data.Add(string.Join("|", parameterTypes));

                for (int i = 0; i < invocation.Arguments.Length; i++) {
                    string marshaledArgument = Marshalling.Marshal(invocation.Arguments[i], parametersInfo[i].ParameterType);
                    data.Add(marshaledArgument);
                }

                string marshaledResult;
                try {
                    marshaledResult = Client.Request(ObjectServer.CommandInvoke, data.ToArray());
                } catch (Exception ex) {
                    throw new ObjectRemoterException("Cannot communicate with remote object. A remote object might be unaccessible.", ex);
                }
                object result = Marshalling.Unmarshal(marshaledResult, invocation.Method.ReturnType);
                invocation.ReturnValue = result;
            }
        }
    }
}
