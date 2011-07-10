using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public class AdapterType
    {
        public string Name { get; private set; }

        public string AssemblyAndClrType { get;  private set; }

        public AdapterType(string name, string assemblyAndClrType)
        {
            Name = name;
            AssemblyAndClrType = assemblyAndClrType;
        }
    }
}
