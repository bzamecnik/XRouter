using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ObjectRemoter
{
    internal static class TypeExtensions
    {
        internal static Type GetType(IEnumerable<Assembly> assemblies, string typeAndAssemblyFullName)
        {
            if (typeAndAssemblyFullName == null)
            {
                throw new ArgumentNullException("typeAndAssemblyFullName");
            }
            string[] parts = typeAndAssemblyFullName.Split('!');
            if (parts.Length != 2)
            {
                throw new ArgumentException("typeAndAssemblyFullName");
            }
            string assemblyFullName = parts[0];
            string typeFullName = parts[1];
            Assembly assembly = assemblies.FirstOrDefault(a => a.FullName == assemblyFullName);
            if (assembly == null)
            {
                assembly = Assembly.Load(new AssemblyName(assemblyFullName));
            }
            Type type = assembly.GetType(typeFullName, true);
            return type;
        }
    }
}
