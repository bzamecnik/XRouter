using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SchematronVal
{
    /// <summary>
    /// Returns information about the schema syntax errors.
    /// </summary>
    public sealed class SyntaxException : Exception
    {               
        public String[] UserMessages { private set; get; }

        internal SyntaxException(String[] messages)
        {
            this.UserMessages = messages;
        }
    }
}