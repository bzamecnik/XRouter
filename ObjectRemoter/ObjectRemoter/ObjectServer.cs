using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    // TODO:
    // - is it useful and possible to "unpublish" an object, ie. remove it
    //   without shutting down the apllication?

    /// <summary>
    /// Allows to publish local objects to be used remotely.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In order to publish an object call the PublishObject() method on a
    /// remotely referable object (IRemotelyReferable) on a local host. Then
    /// use the returned address (RemoteObjectAddress) on remote hosts to
    /// identify the published object when accessing it using the
    /// RemoteObjectProxyProvider.
    /// </para>
    /// <para>
    /// The object server allows only to publish new objects which are then
    /// available indefinitely until the application exits. Currently there
    /// is no way to "unpublish" a previously published objects.
    /// </para>
    /// <para>
    /// The server act in a similar way to singleton. There are no instances
    /// only one static network server and storage of objects.
    /// </para>
    /// </remarks>
    /// <see cref="RemoteObjectProxyProvider"/>
    /// <see cref="ServiceRemoter"/>
    public static class ObjectServer
    {
        internal static readonly string COMMAND_INVOKE = "Invoke";

        internal static readonly int ObjectIDForAnyObjectOfGivenType = -1;

        internal static readonly ServerAddress ServerAddress;

        private static IServer server;

        private static bool isServerRunning = false;

        /// <summary>
        /// A cache of published object adresses referenced by the published
        /// objects itself.
        /// </summary>
        private static Dictionary<object, RemoteObjectAddress> publishedObjectAdresses = new Dictionary<object, RemoteObjectAddress>();

        /// <summary>
        /// A storage of published objects adressed by their identifiers.
        /// </summary>
        private static Dictionary<int, object> publishedObjectsByID = new Dictionary<int, object>();

        static ObjectServer()
        {
            DataLock = new object();

            // TODO: Now it is not possible to start the server at a
            // configured address like 127.0.0.1. For testing purposes it
            // could be configurable.
            server = new TcpServer();
            ServerAddress = server.Address;
            server.RequestReceived += OnRequestReceived;
        }

        internal static object DataLock { get; private set; }

        internal static Dictionary<int, object> PublishedObjects
        {
            get { return publishedObjectsByID; }// TODO: no test coverage
        }

        /// <summary>
        /// Publishes a local object and returns its address which can be then
        /// used for remote access.
        /// </summary>
        /// <param name="obj">Object to be published. Must not be null.</param>
        /// <returns>Address of the published remotely accessible object.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static RemoteObjectAddress PublishObject(IRemotelyReferable obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return InternalPublishObject(obj);
        }

        internal static RemoteObjectAddress InternalPublishObject(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (obj is IRemoteObjectProxy)
            {
                var proxy = (IRemoteObjectProxy)obj;
                return proxy.RemoteObjectAddress;
            }

            lock (DataLock)
            {
                if (publishedObjectAdresses.ContainsKey(obj))
                {
                    var result = publishedObjectAdresses[obj];
                    return result;
                }
            }

            if (!isServerRunning)
            {
                StartServer();
            }

            RemoteObjectAddress address;
            lock (DataLock)
            {
                // TODO: This assumes that the objects are never "unpublished"
                // (ie. removed from the collection). In such a case the
                // identifier created this way could collide with an identifier
                // another of another existing published object!
                int objectID = publishedObjectsByID.Count + 1;
                address = new RemoteObjectAddress(ServerAddress, objectID);

                publishedObjectsByID.Add(objectID, obj);
                publishedObjectAdresses.Add(obj, address);
            }

            return address;
        }

        private static void StartServer()
        {
            server.Start();
            isServerRunning = true;
        }

        private static string OnRequestReceived(string command, string[] data)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // invoke a method on the remote object and return the result in
            // a marshalled form
            if (command == COMMAND_INVOKE)
            {
                string targetInterfaceFullName = data[0];
                int objectID = int.Parse(data[1]);
                string methodName = data[2];

                var parameterTypesNames = data[3].Split('|').Where(typeName => !string.IsNullOrEmpty(typeName)).ToList();
                var parameterTypes = parameterTypesNames.Select(
                    typeName => TypeExtensions.GetType(assemblies, typeName)).ToList();

                Type targetInterface = TypeExtensions.GetType(assemblies, targetInterfaceFullName);
                object obj;
                lock (DataLock)
                {
                    if (objectID == ObjectIDForAnyObjectOfGivenType)
                    {
                        obj = publishedObjectsByID.Values.First(o => targetInterface.IsAssignableFrom(o.GetType()));
                    }
                    else
                    {
                        obj = publishedObjectsByID[objectID];
                    }
                }
                MethodInfo method = obj.GetType().GetMethod(methodName, parameterTypes.ToArray());

                object[] parameters = new object[parameterTypesNames.Count];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = Marshalling.Unmarshal(data[4 + i], parameterTypes[i]);
                }
                object resultObject = method.Invoke(obj, parameters);
                string result = Marshalling.Marshal(resultObject, method.ReturnType);
                return result;
            }

            return null;// TODO: no test coverage
        }
    }
}
