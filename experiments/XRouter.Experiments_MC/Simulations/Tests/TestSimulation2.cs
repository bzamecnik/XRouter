using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.Experiments.Permanent.MC.Components.Version1;

namespace XRouter.Experiments_MC.Simulations.Tests
{
    class TestSimulation2 : Simulation
    {
        public TestSimulation2()
            : base("MC_TestSimulation2")
        {
            var sender1 = new ManualMessageSender("Sender1");
            var sender2 = new ManualMessageSender("Sender2");
            var display1 = new MessageDisplay("Display1");
            var display2 = new MessageDisplay("Display2");
            var workflowProcessor = new BroadcastingWorkflowProcessor("Broadcaster", 2, 2);

            var sender1_wfp = new Connector(sender1, workflowProcessor.ConnectableInputPoints[0]);
            var sender2_wfp = new Connector(sender2, workflowProcessor.ConnectableInputPoints[1]);
            var wfp_display1 = new Connector(workflowProcessor.ConnectableOutputPoints[0], display1);
            var wfp_display2 = new Connector(workflowProcessor.ConnectableOutputPoints[1], display2);

            //Components.Add(sender1_wfp);
            //Components.Add(sender2_wfp);
            //Components.Add(wfp_display1);
            //Components.Add(wfp_display2);
            Components.Add(sender1);
            Components.Add(sender2);
            Components.Add(display1);
            Components.Add(display2);
            Components.Add(workflowProcessor);
        }
    }
}
