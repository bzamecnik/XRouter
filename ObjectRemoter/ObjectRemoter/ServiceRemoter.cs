using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    public static class ServiceRemoter
    {
        private static object stateLock = new object();

        public static Uri PublishService<T>(T service)
            where T : IRemotelyReferable
        {
            RemoteObjectAddress objectAddress = ObjectServer.PublishObject(service);
            Type serviceType = typeof(T);
            Uri serviceUri = objectAddress.ServerAddress.Url;
            return serviceUri;
        }

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
