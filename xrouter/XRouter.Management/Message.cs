using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XRouter.Management
{
    public class Message
    {
        public InputEndpoint Source { get; private set; }

        public XDocument Content { get; private set; }

        public Message(XDocument content, InputEndpoint source)
        {
            Content = content;
            Source = source;
        }
    }
}
