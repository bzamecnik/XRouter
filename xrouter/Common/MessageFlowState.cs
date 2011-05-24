using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    [Serializable]
    public class MessageFlowState
    {
        public Guid MessageFlowGuid { get; set; }

        public string NextNodeName { get; set; }

        public string AssignedProcessor { get; set; }

        public DateTime LastResponseFromProcessor { get; set; }

        public MessageFlowState()
        {
        }
    }
}
