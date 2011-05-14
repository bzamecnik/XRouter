namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public abstract class SectionBase
    {
        private Dictionary<string, Sections> sections = new Dictionary<string, Sections>();

        private Parameters parameters = new Parameters();

        /// <summary>
        /// A set of key-value-pair parameters in the current section.
        /// </summary>
        /// <remarks>It is never null.</remarks>
        public Parameters Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets or sets a section specified by its name (key).
        /// </summary>
        /// <remarks>
        /// In case of getting of non-existent section null is returned. In
        /// case of setting a non-existent section such a parameter is created.
        /// For a leaf section this contains no elements.
        /// </remarks>
        /// <param name="name">Name of the section.</param>
        /// <returns>Section with the specified name or null if such a section
        /// is not present.</returns>
        public Sections this[string name]
        {
            get
            {
                Sections result = null;
                this.sections.TryGetValue(name, out result);
                return result;
            }

            internal set
            {
                this.sections[name] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return sections.Keys;
            }
        }
    }
}
