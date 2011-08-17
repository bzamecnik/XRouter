using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDiagrammer;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowEdgePresenter : EdgePresenter<NodeConfiguration, Tuple<NodeConfiguration, NodeConfiguration>>
    {
        public MessageflowEdgePresenter(Tuple<NodeConfiguration, NodeConfiguration> edge, NodeConfiguration source, NodeConfiguration target)
            : base(edge, source, target)
        {
        }
    }
}
