using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.BasicSystemTest.Server
{
    // This class is used for testing serialization
    [Serializable]
    public class Token
    {
        public string Name { get; private set; }

        public DateTime Created { get; private set; }

        public Token(string name)
        {
            Name = name;
            Created = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}   Created: {1}", Name, Created);
        }
    }
}
