using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace XRouter.Management
{
    public class XRouterManagerProvider
    {
        private static Dictionary<string, IXRouterManager> cachedManagers = new Dictionary<string, IXRouterManager>();

        public static IXRouterManager GetManager(string managerAddress)
        {
            string[] addressParts = managerAddress.Split(';');
            string normalizedAddress = string.Join(";", addressParts);
            if (cachedManagers.ContainsKey(normalizedAddress)) {
                var result = cachedManagers[normalizedAddress];
                return result;
            }

            string assemblyFile = addressParts[0].Trim();
            string typeFullName = addressParts[1].Trim();

            string[] parameters = new string[addressParts.Length - 2];
            for (int i = 0; i < parameters.Length; i++) {
                parameters[i] = addressParts[2 + i];
            }

            Type managerType = null;
            if (assemblyFile.Length > 0) {
                Assembly assembly = Assembly.LoadFile(assemblyFile);
                managerType = assembly.GetType(typeFullName, true);
            } else {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    managerType = assembly.GetType(typeFullName);
                    if (managerType != null) {
                        break;
                    }
                }
                if (managerType == null) {
                    throw new InvalidOperationException("Manager type cannot be found.");
                }
            }

            object managerObject;
            var constructor = managerType.GetConstructor(new Type[] { typeof(string[]) });
            if (constructor != null) {
                managerObject = constructor.Invoke(new object[] { parameters });
            } else {
                constructor = managerType.GetConstructor(Type.EmptyTypes);
                managerObject = constructor.Invoke(new object[0]);
            }

            var manager = (IXRouterManager)managerObject;
            cachedManagers.Add(normalizedAddress, manager);
            return manager;
        }
    }
}
