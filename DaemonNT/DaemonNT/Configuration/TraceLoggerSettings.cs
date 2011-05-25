using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Configuration
{
    /// <summary>
    /// Represents settings of a trace logger.
    /// </summary>
    /// <see cref="DaemonNT.TraceLogger"/>
    internal sealed class TraceLoggerSettings
    {
        public readonly int DefaultBufferSize = 1000;

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
