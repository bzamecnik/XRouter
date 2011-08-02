namespace DaemonNT.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a set of key-value parameters within a single section of
    /// settings.
    /// </summary>
    [Serializable]
    public sealed class Parameters
    {
        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        internal Parameters()
        {
        }

        /// <summary>
        /// Gets or sets a value of parameter specified by its name (key).
        /// </summary>
        /// <remarks>
        /// In case of getting of non-existent parameter value null is
        /// returned. In case of setting a non-existent parameter value a new
        /// parameter with the specified name is created.
        /// </remarks>
        /// <param name="name">Name of the parameter.</param>
        /// <returns>Value of a parameter with the specified name or null if
        /// such a parameter is not present.</returns>
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

        public ICollection<string> Keys
        {
            get
            {
                return parameters.Keys;
            }
        }

        public void Remove(string key)
        {
            parameters.Remove(key);
        }
    }
}
