using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Management
{
    public class ErrorEvent
    {
        public DateTime Raised { get; private set; }
        public string Type { get; private set; }
        public string Content { get; private set; }

        public ErrorEvent(DateTime raised, string type, string content)
        {
            Raised = raised;
            Type = type;
            Content = content;
        }
    }
}
