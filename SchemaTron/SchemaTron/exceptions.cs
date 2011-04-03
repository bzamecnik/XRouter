using System;

namespace SchemaTron
{
    /// <summary>
    /// Represents one or more schema syntax errors and contains user
    /// information about them.
    /// </summary>
    public sealed class SyntaxException : Exception
    {
        // TODO: is it better to store the messages in a list or an array?

        /// <summary>
        /// Gets or sets user messages concerned with each of the syntax
        /// errors.
        /// </summary>
        public String[] UserMessages { private set; get; }

        /// <summary>
        /// Creates an instance of a SyntaxException containing one or more
        /// user messages about the syntax errors.
        /// </summary>
        /// <param name="messages">user messages on the syntax errors</param>
        internal SyntaxException(String[] messages)
        {
            this.UserMessages = messages;
        }
    }
}