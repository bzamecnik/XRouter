namespace DaemonNT
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Poskytuje dynamicke nacitani typu a vytvareni jejich instanci. 
    /// </summary>
    internal static class TypesProvider
    {
        public static Service CreateService(string typeClass, string typeAssembly)
        {
            object obj = CreateTypeInstance(typeClass, typeAssembly);

            Service service = obj as Service;
            if (service == null)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' is not service implementation.", typeClass));
            }

            return service;
        }

        private static object CreateTypeInstance(string typeClass, string typeAssembly)
        {
            Assembly currAssembly = Assembly.LoadFrom(typeAssembly);

            object obj = currAssembly.CreateInstance(typeClass);
            if (obj == null)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' does not exist.", typeClass));
            }

            return obj;
        }
    }
}
