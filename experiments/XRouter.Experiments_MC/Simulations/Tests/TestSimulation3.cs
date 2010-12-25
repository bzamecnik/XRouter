using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.Experiments.Permanent.MC.Components.Samples;

namespace XRouter.Experiments_MC.Simulations.Tests
{
    class TestSimulation3 : Simulation
    {
        public TestSimulation3()
            : base("MC_TestSimulation3")
        {
            var sender1 = new ManualMessageSender_v1("Sender1");
            var sender2 = new ManualMessageSender_v1("Sender2");
            var display1 = new MessageDisplay_v1("Display1");
            var display2 = new MessageDisplay_v1("Display2");
            var storage = new BasicMessageFlowManagementStorage_v1("Storage");
            var management = new BasicMessageFlowManagement_v1("Managemenr", storage);
            var managementProxy = new BasicMessageFlowManagementProxy_v1(management);
            var workflowProcessor = new BroadcastingMessageFlowProcessor_v2("Broadcaster", 2, 2, managementProxy);
            

            var sender1_wfp = new Connector_v1(sender1, workflowProcessor.ConnectableInputPoints[0]);
            var sender2_wfp = new Connector_v1(sender2, workflowProcessor.ConnectableInputPoints[1]);
            var wfp_display1 = new Connector_v1(workflowProcessor.ConnectableOutputPoints[0], display1);
            var wfp_display2 = new Connector_v1(workflowProcessor.ConnectableOutputPoints[1], display2);

            Components.Add(sender1);
            Components.Add(sender2);
        }
    }
}
