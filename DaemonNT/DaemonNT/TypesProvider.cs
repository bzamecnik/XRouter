namespace DaemonNT
{
    using System;
    using System.Reflection;
    using DaemonNT.Logging;

    // TODO: could be renamed to TypeProvider

    /// <summary>
    /// Provides dynamic loading of types and creating their instances.
    /// </summary>
    internal static class TypesProvider
    {
        /// <summary>
        /// Creates an instance of given service type from given assembly.
        /// </summary>
        /// <remarks>
        /// The type must be DaemonNT.Service or a type derived from it.
        /// </remarks>
        /// <param name="typeClass">Name of the type to be instantiated</param>
        /// <param name="typeAssembly">Path to the assembly where the type
        /// should be located.</param>
        /// <returns>Created instance of the service.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the instance cannot be created.
        /// </exception>
        /// <see cref="DaemonNT.Service"/>
        public static Service CreateService(string typeClass, string typeAssembly)
        {
            object instance = CreateTypeInstance(typeClass, typeAssembly);

            Service service = instance as Service;
            if (service == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Type '{0}' is not a service implementation.", typeClass));
            }

            return service;
        }

        /// <summary>
        /// Creates an instance of given trace logger storage type from given
        /// assembly.
        /// </summary>
        /// <remarks>
        /// The type must be DaemonNT.TraceLoggerStorage or a type derived
        /// from it.
        /// </remarks>
        /// <param name="typeClass">Type of trace logger storage
        /// implementation.</param>
        /// <param name="typeAssembly">Path to the assembly where the type
        /// should be located.</param>
        /// <returns>Created instance of the trace logger storage.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the instance cannot be created.
        /// </exception>
        /// <see cref="TraceLoggerStorage"/>
        public static TraceLoggerStorage CreateTraceLoggerStorage(
            string typeClass,
            string typeAssembly)
        {
            object instance = CreateTypeInstance(typeClass, typeAssembly);

            TraceLoggerStorage storage = instance as TraceLoggerStorage;
            if (storage == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Type '{0}' is not a trace logger storage implementation.",
                    typeClass));
            }

            return storage;
        }

        /// <summary>
        /// Creates an instance of given type from given assembly.
        /// </summary>
        /// <param name="typeClass">Name of the type to be instantiated.
        /// </param>
        /// <param name="typeAssembly">Path to the assembly where the type
        /// should be located.</param>
        /// <returns>The created instance of the type.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the type cannot be created.
        /// </exception>
        private static object CreateTypeInstance(string typeClass, string typeAssembly)
        {
            // TODO:
            // These methods can throw a lot of exceptions. Should they be
            // wrapped into InvalidOperationException?
            Assembly assembly = Assembly.LoadFrom(typeAssembly);

            object instance = assembly.CreateInstance(typeClass);
            if (instance == null)
            {
                throw new InvalidOperationException(
                    string.Format("Type '{0}' does not exist.", typeClass));
            }

            return instance;
        }
    }
}
