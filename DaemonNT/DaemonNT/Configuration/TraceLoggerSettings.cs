using System.Collections.Generic;

namespace DaemonNT.Configuration
{
    using System;

    // TODO: rename to TraceLoggerConfig to avoid confusion with Settings

    /// <summary>
    /// Represents settings of a trace logger.
    /// </summary>
    /// <seealso cref="DaemonNT.TraceLogger"/>
    [Serializable]
    internal sealed class TraceLoggerSettings
    {
        public static readonly int DefaultBufferSize = 1000;

        /// <summary>
        /// Number of log records the buffer has capacity for.
        /// </summary>
        public int BufferSize { set; get; }

        /// <summary>
        /// Definitions of storages where the logger saves log records.
        /// </summary>
        /// <remarks>Must not be null.</remarks>
        public List<TraceLoggerStorageSettings> Storages { private set; get; }

        /// <summary>
        /// Creates a new instance of trace logger settings with default
        /// values and no storages.
        /// </summary>
        public TraceLoggerSettings()
        {
            this.BufferSize = DefaultBufferSize;
            this.Storages = new List<TraceLoggerStorageSettings>();
        }
    }
}
