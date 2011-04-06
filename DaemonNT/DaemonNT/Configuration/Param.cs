using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Configuration
{
    public sealed class Param
    {
        internal Param()
        { }

        private Dictionary<String, String> @params = new Dictionary<String, String>();

        public String this[String name]
        {
            internal set 
            { 
                this.@params[name] = value; 
            }
            get 
            {
                String value;
                if (this.@params.TryGetValue(name, out value))
                {
                    return value;
                }
                return null;             
            }
        }
    }
}
