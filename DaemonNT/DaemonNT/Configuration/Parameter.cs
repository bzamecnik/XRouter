namespace DaemonNT.Configuration
{
    using System.Collections.Generic;

    public sealed class Parameter
    {
        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        internal Parameter()
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
