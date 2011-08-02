namespace DaemonNT.Configuration
{
    using System;

    // TODO: rename to Section

    /// <summary>
    /// Represents a single section of hierarchical settings.
    /// </summary>
    /// <remarks>
    /// Each section can contain key-value parameters and inner sections
    /// thus creating a hierarchy.
    /// </remarks>
    [Serializable]
    public sealed class Sections : SectionBase
    {
        internal Sections()
        {
        }
    }
}
