namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public abstract class SectionBase
    {
        private Dictionary<string, Section> sections = new Dictionary<string, Section>();

        private Param parameter = new Param();

        // TODO: Rename to Parameters as it contains a dictionary of parameters
        // not a single one. This is very confusing!

        /// <summary>
        /// A set of key-value-pair parameters in the current section.
        /// </summary>
        /// <remarks>It is never null.</remarks>
        public Param Parameter
        {
            get { return this.parameter; }
        }

        // TODO: rename to Sections for less confusion

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
        public Section this[string name]
        {
            get
            {
                Section result = null;
                this.sections.TryGetValue(name, out result);
                return result;
            }

            internal set
            {
                this.sections[name] = value;
            }
        }
    }
}
