using System;

namespace DaemonNT.Configuration
{
    /// <summary>
    /// Represents settings of a trace logger storage.
    /// </summary>
    /// <seealso cref="DaemonNT.TraceLoggerStorage"/>
    [Serializable]
    internal sealed class TraceLoggerStorageSettings
    {
        public TraceLoggerStorageSettings()
        { }

        /// <summary>
        /// Name of the trace logger storage.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public String Name { set; get; }

        /// <summary>
        /// Identifier of the class defining the storage type.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public string TypeClass { get; set; }

        /// <summary>
        /// Path to and assembly where the storage type is located.
        /// </summary>
        /// <remarks>
        /// Can be both relative or absolute. Relative path is based on the
        /// current appdomain base directory.
        /// </remarks>
        public string TypeAssembly { get; set; }

        /// <summary>
        /// Settings of the strace logger storage (key-value pairs in a
        /// hierarchy of sections).
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public Settings Settings { get; set; }
    }
}
