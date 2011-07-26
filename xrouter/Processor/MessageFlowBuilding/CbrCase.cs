using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Processor.MessageFlowBuilding
{
    public class CbrCase
    {
        public string Schematron { get; private set; }
        public NodeConfiguration TargetNode { get; private set; }

        public CbrCase(string schematron, NodeConfiguration targetNode)
        {
            Schematron = schematron;
            TargetNode = targetNode;
        }
    }
}
