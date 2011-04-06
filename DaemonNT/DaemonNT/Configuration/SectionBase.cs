using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Configuration
{
    public abstract class SectionBase
    {
        private Dictionary<String, Section> sections = new Dictionary<String,Section>();

        private Param param = new Param();
                
        public Section this[String name]
        {
            internal set 
            { 
                sections[name] = value; 
            }
            get 
            { 
                Section result;
                if (this.sections.TryGetValue(name, out result))
                {
                    return result;
                }
                return null;             
            }
        }

        public Param Param 
        {
            get { return this.param; }
        }        
    }
}
