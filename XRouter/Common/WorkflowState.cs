using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    [Serializable]
    public class WorkflowState
    {
        public int WorkflowVersion { get; set; }

        public List<int> CurrentNodes { get; private set; }

        public string AssignedProcessor { get; set; }

        public DateTime LastResponseFromProcessor { get; set; }

        public WorkflowState()
        {
            CurrentNodes = new List<int>();
        }
    }
}
