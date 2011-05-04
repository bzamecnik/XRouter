using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    /// <summary>
    /// Allows to publish local objects to be used remotely.
    /// </summary>
    public static class ObjectServer
    {
        internal static readonly string CommandInvoke = "Invoke";

        internal static readonly int ObjectIDForAnyObjectOfGivenType = -1;

        internal static readonly ServerAddress ServerAddress;

        private static IServer server;

        private static bool isServerRunning = false;

        private static Dictionary<int, object> publishedObjects = new Dictionary<int, object>();

        private static Dictionary<object, RemoteObjectAddress> publishedObjectAdresses = new Dictionary<object, RemoteObjectAddress>();

        private static Dictionary<int, object> publishedObjectsByID = new Dictionary<int, object>();

        static ObjectServer()
        {
            DataLock = new object();

            server = new TcpServer();
            ServerAddress = server.Address;
            server.RequestReceived += OnRequestReceived;
        }

        internal static object DataLock { get; private set; }

        internal static Dictionary<int, object> PublishedObjects
        {
            get { return publishedObjects; }
        }

        /// <summary>
        /// Publishes a local object and returns its address which can be
        /// used for remote access.
        /// </summary>
        /// <param name="obj">Object to be published.</param>
        /// <returns>Address of the published remote object.</returns>
        public static RemoteObjectAddress PublishObject(IRemotelyReferable obj)
        {
            return InternalPublishObject(obj);
        }

        internal static RemoteObjectAddress InternalPublishObject(object obj)
        {
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

            if (command == CommandInvoke)
            {
                string targetInterfaceFullName = data[0];
                int objectID = int.Parse(data[1]);
                string methodName = data[2];

                string[] parameterTypesNames = data[3].Split('|');
                var parameterTypes = new List<Type>();
                foreach (string parameterTypeName in parameterTypesNames)
                {
                    Type type = TypeExtensions.GetType(assemblies, parameterTypeName);
                    parameterTypes.Add(type);
                }

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

                object[] parameters = new object[parameterTypesNames.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = Marshalling.Unmarshal(data[4 + i], parameterTypes[i]);
                }
                object resultObject = method.Invoke(obj, parameters);
                string result = Marshalling.Marshal(resultObject, method.ReturnType);
                return result;
            }

            return null;
        }
    }
}
