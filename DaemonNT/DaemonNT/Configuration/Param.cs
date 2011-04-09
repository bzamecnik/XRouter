namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public sealed class Param
    {
        private Dictionary<string, string> @params = new Dictionary<string, string>();

        internal Param()
        {
        }

        public string this[string name]
        {
            get
            {
                string value;
                if (this.@params.TryGetValue(name, out value))
                {
                    return value;
                }

                return null;
            }

            internal set
            {
                this.@params[name] = value;
            }
        }
    }
}
