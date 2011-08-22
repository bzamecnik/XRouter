using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator;
using XRouter.Common.Utils;

namespace XRouter.Common.MessageFlowConfig
{
    /// <summary>
    /// Represents a type of an action - its symbolic name and an
    /// associated CLR type.
    /// </summary>
    public class ActionType
    {
         /// <summary>
        /// Symbolic name of the action type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Action type identified by path to its assembly and CLR type
        /// description.
        /// </summary>
        public string ClrTypeAndAssembly { get; private set; }

        public ClassMetadata ConfigurationMetadata { get; private set; }

        public ActionType(string name, string assemblyAndClrType, ClassMetadata configurationMetadata)
        {
            Name = name;
            ClrTypeAndAssembly = assemblyAndClrType;
            ConfigurationMetadata = configurationMetadata;
        }
    }
}
