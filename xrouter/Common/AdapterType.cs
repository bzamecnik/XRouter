using ObjectConfigurator;
namespace XRouter.Common
{
    /// <summary>
    /// Represents a type of an adapter - its symbolic name and an
    /// associated CLR type.
    /// </summary>
    public class AdapterType
    {
        /// <summary>
        /// Symbolic name of the adapter type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Adapter type identified by path to its assembly and CLR type
        /// description.
        /// </summary>
        public string AssemblyAndClrType { get; private set; }

        public ClassMetadata ConfigurationMetadata { get; private set; }

        public AdapterType(string name, string assemblyAndClrType, ClassMetadata configurationMetadata)
        {
            Name = name;
            AssemblyAndClrType = assemblyAndClrType;
            ConfigurationMetadata = configurationMetadata;
        }
    }
}
