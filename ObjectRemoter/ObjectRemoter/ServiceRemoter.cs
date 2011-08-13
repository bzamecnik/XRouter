using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    /// <summary>
    /// Allows to publish a service object and then use it remotely via
    /// a proxy. The remote object is identified by a URI.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To publish an object call the PublishService() method on a local host.
    /// It provides a URI which identified the published object. Then on a
    /// remote host obtain the proxy by calling the GetServiceProxy()
    /// method with the provided URI. The proxy can then be used in a similar
    /// way as the original object.
    /// </para>
    /// <para>
    /// It is a front-end to ObjectServer and RemoteObjectProxyProvider.
    /// The difference is that in the method signature service objects are
    /// strogly typed. Another difference is that the GetServiceProxy()
    /// method take any published instance of derised type.
    /// TODO: this might be undesirable behavior.
    /// </para>
    /// </remarks>
    /// <see cref="ObjectRemoter"/>
    /// <see cref="RemoteObjectProxyProvider"/>
    public static class ServiceRemoter
    {
        /// <summary>
        /// Publish a service object identified by a URI.
        /// </summary>
        /// <typeparam name="T">Type of the service object.</typeparam>
        /// <param name="service">Service object. Must not be null.</param>
        /// <returns>URI identifying the service object.</returns>
        public static Uri PublishService<T>(T service)
            where T : IRemotelyReferable
        {
            RemoteObjectAddress objectAddress = ObjectServer.PublishObject(service);
            Type serviceType = typeof(T);
            Uri serviceUri = objectAddress.ServerAddress.Url;
            return serviceUri;
        }

        /// <summary>
        /// Get a proxy to a service object identified by a URI.
        /// </summary>
        /// <typeparam name="T">Type of the service object.</typeparam>
        /// <param name="serviceUri">Service URI. Must not be null.</param>
        /// <returns>Proxy to the service object.</returns>
        public static T GetServiceProxy<T>(Uri serviceUri)
            where T : IRemotelyReferable
        {
            ServerAddress serverAddress = new ServerAddress(serviceUri);
            RemoteObjectAddress objectAddress = new RemoteObjectAddress(serverAddress, ObjectServer.ObjectIDForAnyObjectOfGivenType);
            T proxy = RemoteObjectProxyProvider.GetProxy<T>(objectAddress);
            return proxy;
        }
    }
}
