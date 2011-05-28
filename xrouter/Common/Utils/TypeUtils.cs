using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace XRouter.Common.Utils
{
    public class TypeUtils
    {
        public static T CreateTypeInstance<T>(string typeAndAssembly)
        {
            string typeFullName;
            string assemblyPath;
            string[] parts = typeAndAssembly.Split(',');
            if (parts.Length == 2) {
                typeFullName = parts[0].Trim();
                assemblyPath = parts[1].Trim();
            } else if (parts.Length == 1) {
                typeFullName = parts[0].Trim();
                assemblyPath = null;
            } else {
                throw new InvalidOperationException(string.Format("Invalid type identification: '{0}'", typeAndAssembly));
            }

            return CreateTypeInstance<T>(typeFullName, assemblyPath);
        }

        public static T CreateTypeInstance<T>(string typeFullName, string assemblyPath)
        {
            if ((assemblyPath != null) && (!Path.IsPathRooted(assemblyPath))) {
                assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath);
            }

            #region Prepare type
            Type type;
            try {
                if (assemblyPath != null) {
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    type = assembly.GetType(typeFullName, true);
                } else {
                    type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(typeFullName, false)).FirstOrDefault(t => t != null);
                    if (type == null) {
                        throw new InvalidOperationException(string.Format("Cannot find type '{0}'.", typeFullName));
                    }
                }
            } catch (Exception ex) {
                throw new InvalidOperationException(string.Format("Cannot access type '{0}'.", typeFullName), ex);
            }
            #endregion

            #region Create instance
            object instance;
            try {
                instance = Activator.CreateInstance(type, true);
            } catch (Exception ex) {
                throw new InvalidOperationException(string.Format("Cannot create instance of type '{0}' using default constructor.", typeFullName), ex);
            }
            #endregion

            if (!(instance is T)) {
                throw new InvalidOperationException(string.Format("Type '{0}' does not implement/extend '{1}'.", typeFullName, typeof(T).FullName));
            }

            return (T)instance;
        }
    }
}
