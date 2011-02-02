using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Remoting.RemoteCommunication;
using System.Reflection;

namespace XRouter.Remoting
{
    public static class ObjectServer
    {
        internal static readonly string CommandInvoke = "Invoke";

        public static readonly string ServerGuid = Guid.NewGuid().ToString();

        public static readonly ServerAddress ServerAddress;

        private static IServer server;

        private static Dictionary<int, object> publishedObjects = new Dictionary<int, object>();
        internal static Dictionary<int, object> PublishedObjects {
            get { return publishedObjects; }
        }

        internal static object DataLock { get; private set; }
        private static Dictionary<object, RemoteObjectAddress> publishedObjectAdresses = new Dictionary<object, RemoteObjectAddress>();
        private static Dictionary<int, object> publishedObjectsByID = new Dictionary<int, object>();

        private static bool isServerRunning = false;

        static ObjectServer()
        {
            DataLock = new object();

            server = new TcpServer();
            ServerAddress = server.Address;
            server.RequestArrived += OnRequestArrived;
        }

        public static RemoteObjectAddress PublishObject(IRemotelyReferable obj)
        {
            return InternalPublishObject(obj);
        }

        internal static RemoteObjectAddress InternalPublishObject(object obj)
        {
            if (obj is IRemoteObjectProxy) {
                var proxy = (IRemoteObjectProxy)obj;
                return proxy.RemoteObjectAddress;
            }

            lock (DataLock) {
                if (publishedObjectAdresses.ContainsKey(obj)) {
                    var result = publishedObjectAdresses[obj];
                    return result;
                }
            }

            if (!isServerRunning) {
                StartServer();
            }

            RemoteObjectAddress address;
            lock (DataLock) {
                int objectID = publishedObjectsByID.Count + 1;
                address = new RemoteObjectAddress(ServerAddress, ServerGuid, objectID);

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

        private static string OnRequestArrived(string command, string[] data)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (command == CommandInvoke) {
                int objectID = int.Parse(data[0]);
                string methodName = data[1];

                string[] parameterTypesNames = data[2].Split('|');
                var parameterTypes = new List<Type>();
                foreach (string parameterTypeName in parameterTypesNames) {
                    string[] parts = parameterTypeName.Split('!');
                    string assemblyFullName = parts[0];
                    string typeFullName = parts[1];
                    Assembly assembly = assemblies.FirstOrDefault(a => a.FullName == assemblyFullName);
                    if (assembly == null) {
                        assembly = Assembly.Load(new AssemblyName(assemblyFullName));
                    }
                    Type type = assembly.GetType(typeFullName, true);
                    parameterTypes.Add(type);
                }

                object obj;
                lock (DataLock) {
                    obj = publishedObjectsByID[objectID];
                }
                MethodInfo method = obj.GetType().GetMethod(methodName, parameterTypes.ToArray());

                object[] parameters = new object[parameterTypesNames.Length];
                for (int i = 0; i < parameters.Length; i++) {
                    parameters[i] = Marshaling.Unmarshal(data[3 + i], parameterTypes[i]);
                }
                object resultObject = method.Invoke(obj, parameters);
                string result = Marshaling.Marshal(resultObject, method.ReturnType);
                return result;
            }

            return null;
        }
    }
}
