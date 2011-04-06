using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using DaemonNT.Logging;

namespace DaemonNT
{
    /// <summary>
    /// Poskytuje dynamicke nacitani typu a vytvareni jejich instanci. 
    /// </summary>
    internal static class TypesProvider
    {        
        private static Object CreateTypeInstance(String typeClass, String typeAssembly)
        {
            Assembly currAssembly = Assembly.LoadFrom(typeAssembly);

            Object obj = currAssembly.CreateInstance(typeClass);
            if (obj == null)
            {
                throw new InvalidOperationException(String.Format("Type '{0}' does not exist.", typeClass));
            }

            return obj;
        }
                     
        public static Service CreateService(String typeClass, String typeAssembly)
        {
            Object obj = CreateTypeInstance(typeClass, typeAssembly);
            
            Service service = obj as Service;
            if (service == null)
            {
                throw new InvalidOperationException(String.Format("Type '{0}' is not service implementation.", typeClass));
            }

            return service;
        }    
    }
}
