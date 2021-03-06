﻿using System;

namespace DaemonNT.Configuration
{
    /// <summary>
    /// Represents hierarchical settings of key-value parameters located in
    /// sections.
    /// </summary>
    /// <remarks>
    /// The Settings class itself represents a top-level section.
    /// </remarks>
    [Serializable]
    public sealed class Settings : SectionBase
    {
        internal Settings()
        {
        }
    }
}
