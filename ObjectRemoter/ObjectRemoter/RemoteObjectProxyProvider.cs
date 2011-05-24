using System;
using System.Collections.Generic;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    /// <summary>
    /// A provider of proxies for remote objects.
    /// </summary>
    /// <see cref="ObjectRemoter"/>
    /// <see cref="ServiceRemoter"/>
    public static class RemoteObjectProxyProvider
    {
        private static object dataLock = new object();
        private static Dictionary<RemoteObjectAddress, object> cache = new Dictionary<RemoteObjectAddress, object>();

        private static Castle.DynamicProxy.ProxyGenerator proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();

        /// <summary>
        /// Creates a proxy for a remote object specified by its address.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a target object is on the same process, the referred object is
        /// returned directly instead of its proxy. In case of a remote object
        /// the usage of the proxy may result in the ObjectRemoterException.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">Interface the proxy must implement.
        /// </typeparam>
        /// <param name="address">Address of the target remote object. Must not
        /// be null.</param>
        /// <param name="requiredInterface">A constraint on the interface which
        /// the proxy must implement. Can be null, in which case the type
        /// parameter T is used as a default value.</param>
        /// <returns>Proxy to a remote object which implements given interface.
        /// </returns>
        /// <seealso cref="ObjectRemoterException"/>
        public static T GetProxy<T>(RemoteObjectAddress address, Type requiredInterface = null)
            where T : IRemotelyReferable
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (requiredInterface == null)
            {
                requiredInterface = typeof(T);
            }

            if ((!requiredInterface.IsInterface) || (!typeof(T).IsInterface))
            {
                // TODO: Isn't ArgumentException more appropriate here?
                // If so fix the unit tests as well.
                // TODO: no test coverage
                throw new InvalidOperationException("Parameter requiredInterface and type argument T must be an interface.");
            }

            // TODO: should we in fact compare typeof(T) and
            // proxyObject.GetType() or requiredInterface?
            object proxyObject = InternalGetProxy(address, requiredInterface);
            if (!typeof(T).IsAssignableFrom(proxyObject.GetType()))
            {
                throw new ArgumentException("Required interface does not match with type argument T.", "requiredInterface");
            }

            T proxy = (T)proxyObject;
            return proxy;
        }

        private static object InternalGetProxy(RemoteObjectAddress address, Type requiredInterface)
        {
            if (address.ServerAddress.IsLocal)
            {
                // TODO: no test coverage
                lock (ObjectServer.DataLock)
                {
                    object localObject = ObjectServer.PublishedObjects[address.ObjectID];
                    return localObject;
                }
            }

            lock (dataLock)
            {
                if (cache.ContainsKey(address))
                {
                    object cachedProxy = cache[address];
                    return cachedProxy;
                }
            }

            var interceptor = new ProxyInterceptor(address, requiredInterface);
            object proxy;
            lock (dataLock)
            {
                proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget(requiredInterface, interceptor);
            }

            lock (dataLock)
            {
                cache.Add(address, proxy);
            }

            return proxy;
        }

        private class ProxyInterceptor : Castle.DynamicProxy.IInterceptor
        {
            public Type ProvidedInterface { get; private set; }

            public ProxyInterceptor(RemoteObjectAddress targetAddress, Type providedInterface)
            {
                TargetAddress = targetAddress;
                ProvidedInterface = providedInterface;
                Client = new TcpClient(targetAddress.ServerAddress);
            }

            public RemoteObjectAddress TargetAddress { get; private set; }

            private IClient Client { get; set; }

            public void Intercept(Castle.DynamicProxy.IInvocation invocation)
            {
                var data = new List<string>();

                data.Add(string.Format("{0}!{1}", ProvidedInterface.Assembly.FullName, ProvidedInterface.FullName));
                data.Add(TargetAddress.ObjectID.ToString());
                data.Add(invocation.Method.Name);

                var parametersInfo = invocation.Method.GetParameters();
                var parameterTypes = new List<string>();
                foreach (var parameter in parametersInfo)
                {
                    string parameterType = string.Format("{0}!{1}", parameter.ParameterType.Assembly.FullName, parameter.ParameterType.FullName);
                    parameterTypes.Add(parameterType);
                }

                data.Add(string.Join("|", parameterTypes));

                for (int i = 0; i < invocation.Arguments.Length; i++)
                {
                    string marshalledArgument = Marshalling.Marshal(invocation.Arguments[i], parametersInfo[i].ParameterType);
                    data.Add(marshalledArgument);
                }

                string marshalledResult;
                try
                {
                    marshalledResult = Client.Request(ObjectServer.CommandInvoke, data.ToArray());
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    // TODO: no test coverage
                    throw new ObjectRemoterException("Cannot communicate with the remote object. It might be unaccessible.", ex);
                }
                object result = Marshalling.Unmarshal(marshalledResult, invocation.Method.ReturnType);
                invocation.ReturnValue = result;
            }
        }
    }
}
