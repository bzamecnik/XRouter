namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public abstract class SectionBase
    {
        private Dictionary<string, Section> sections = new Dictionary<string, Section>();

        private Param parameter = new Param();

        // TODO: Rename to Parameters as it contains a dictionary of parameters
        // not a single one. This is very confusing!

        public Param Parameter
        {
            get { return this.parameter; }
        }

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
