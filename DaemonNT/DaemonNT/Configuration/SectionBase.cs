namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public abstract class SectionBase
    {
        private Dictionary<string, Section> sections = new Dictionary<string, Section>();

        private Param param = new Param();

        public Param Param
        {
            get { return this.param; }
        }

        public Section this[string name]
        {
            get
            {
                Section result;
                if (this.sections.TryGetValue(name, out result))
                {
                    return result;
                }

                return null;
            }

            internal set
            {
                this.sections[name] = value;
            }
        }
    }
}
