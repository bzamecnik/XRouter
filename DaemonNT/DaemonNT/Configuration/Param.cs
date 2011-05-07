namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    // TODO: should be renamed to Parameter

    public sealed class Param
    {
        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        internal Param()
        {
        }

        public string this[string name]
        {
            get
            {
                string value = null;
                this.parameters.TryGetValue(name, out value);
                return value;
            }

            internal set
            {
                this.parameters[name] = value;
            }
        }
    }
}
