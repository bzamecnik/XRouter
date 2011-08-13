using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XRouter.Common.Utils
{
    /// <summary>
    /// Utilities for working with types, such as loading and creating
    /// instances.
    /// </summary>
    public class TypeUtils
    {
        /// <summary>
        /// Creates a new instance of a type specified by its type name and
        /// assembly where it is located. Default constructor is used.
        /// </summary>
        /// <typeparam name="T">the loaded type must be convertible to this
        /// type (it must extend or implement this type)
        /// </typeparam>
        /// <param name="typeAndAssembly">specification of the type which
        /// should be instantiated (CLR type name and assembly path, separated
        /// by a comma (optionally followed by a whitespace)</param>
        /// <exception cref="System.InvalidOperationException">if the type
        /// instance cannot be created or is not compatible with the result
        /// type T
        /// </exception>
        /// <returns></returns>
        public static T CreateTypeInstance<T>(string typeAndAssembly)
        {
            Type type = GetType(typeAndAssembly);
            return CreateTypeInstance<T>(type);
        }

        /// <summary>
        /// Creates a new instance of a type specified by its type name and
        /// assembly where it is located. Default constructor is used.
        /// </summary>
        /// <typeparam name="T">the loaded type must be convertible to this
        /// type (it must extend or implement this type)
        /// </typeparam>
        /// <param name="typeFullName">full name of the type</param>
        /// <param name="assemblyPath">assembly path where the type should be
        /// located</param>
        /// <exception cref="System.InvalidOperationException">if the type
        /// instance cannot be created or is not compatible with the result
        /// type T
        /// </exception>
        /// <returns></returns>
        public static T CreateTypeInstance<T>(string typeFullName, string assemblyPath)
        {
            Type type = GetType(typeFullName, assemblyPath);
            return CreateTypeInstance<T>(type);
        }

        private static T CreateTypeInstance<T>(Type type)
        {
            #region Create instance
            object instance;
            try
            {
                instance = Activator.CreateInstance(type, true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(
                    "Cannot create an instance of type '{0}' using the default constructor.",
                    type.FullName), ex);
            }
            #endregion

            if (!(instance is T))
            {
                throw new InvalidOperationException(string.Format(
                    "Type '{0}' must implement or extend '{1}'.", type.FullName, typeof(T).FullName));
            }

            return (T)instance;
        }

        /// <summary>
        /// Gets the Type information about a type specified by its name and
        /// assembly path within a single string.
        /// </summary>
        /// <remarks>
        /// If the assembly path is not given it tries to locate the type in
        /// assemblies currently loaded into the execution context of the
        /// current application domain.
        /// </remarks>
        /// <param name="typeAndAssembly">full type name and optional assembly
        /// path, separated by a comma and possible whitespace</param>
        /// <exception cref="System.InvalidOperationException">If the type and
        /// assembly is specified badly or the type cannot the loaded.
        /// </exception>
        /// <returns></returns>
        public static Type GetType(string typeAndAssembly)
        {
            string typeFullName;
            string assemblyPath;
            string[] parts = typeAndAssembly.Split(',');
            if (parts.Length == 2)
            {
                typeFullName = parts[0].Trim();
                assemblyPath = parts[1].Trim();
            }
            else if (parts.Length == 1)
            {
                typeFullName = parts[0].Trim();
                assemblyPath = null;
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Invalid type identification '{0}'", typeAndAssembly));
            }

            return GetType(typeFullName, assemblyPath);
        }

        /// <summary>
        /// Gets the Type information about a type specified by its name and
        /// assembly path.
        /// </summary>
        /// <remarks>
        /// If the assembly path is not given it tries to locate the type in
        /// assemblies currently loaded into the execution context of the
        /// current application domain.
        /// </remarks>
        /// <param name="typeFullName">Full name of the type</param>
        /// <param name="assemblyPath">Optional path to assembly where the type
        /// should be located, absolute or relative to current appdomain base
        /// directory. Can be null.</param>
        /// <exception cref="System.InvalidOperationException">If the type
        /// cannot be found or loaded.
        /// </exception>
        /// <returns></returns>
        public static Type GetType(string typeFullName, string assemblyPath)
        {
            if ((assemblyPath != null) && (!Path.IsPathRooted(assemblyPath)))
            {
                assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath);
            }

            #region Prepare type
            Type type;
            try
            {
                if (assemblyPath != null)
                {
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    type = assembly.GetType(typeFullName, true);
                }
                else
                {
                    type = AppDomain.CurrentDomain.GetAssemblies().Select(
                        a => a.GetType(typeFullName, false)).FirstOrDefault(t => t != null);
                    if (type == null)
                    {
                        throw new InvalidOperationException(string.Format(
                            "Cannot find type '{0}'.", typeFullName));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(
                    "Cannot access type '{0}'.", typeFullName), ex);
            }
            #endregion

            return type;
        }
    }
}
