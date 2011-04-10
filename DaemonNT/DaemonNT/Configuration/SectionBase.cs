namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public abstract class SectionBase
    {
        private Dictionary<string, Section> sections = new Dictionary<string, Section>();

        private Parameter parameter = new Parameter();

        public Parameter Parameter
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
