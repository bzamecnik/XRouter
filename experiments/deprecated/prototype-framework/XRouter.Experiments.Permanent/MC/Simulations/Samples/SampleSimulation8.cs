using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using XRouter.Components.Core;
using MC = XRouter.Experiments.Permanent.MC;
using BZ = XRouter.Experiments.Permanent.BZ;
using PS = XRouter.Experiments.Permanent.PS;
using SB = XRouter.Experiments.Permanent.SB;
using TK = XRouter.Experiments.Permanent.TK;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.Experiments.Permanent.MC.Components.Samples;

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation8 : Simulation
    {
        public SampleSimulation8()
            : base("MC_SampleSimulation8")
        {
            var sender1 = new ManualMessageSender_v1("Sender1");
            var sender2 = new ManualMessageSender_v1("Sender2");
            var display1 = new MessageDisplay_v1("Display1");
            var display2 = new MessageDisplay_v1("Display2");

            var storage = new BasicMessageFlowManagementStorage_v1("Storage");
            var management = new BasicMessageFlowManagement_v1("Management", storage);
            var managementProxy = new BasicMessageFlowManagementProxy_v1(management);
            var broadcaster = new BroadcastingMessageFlowProcessor_v2("Broadcaster", 2, 2, managementProxy);

            var sender1_wfp = new Connector_v1(sender1, broadcaster.ConnectableInputPoints[0]);
            var sender2_wfp = new Connector_v1(sender2, broadcaster.ConnectableInputPoints[1]);
            var broadcaster_display1 = new Connector_v1(broadcaster.ConnectableOutputPoints[0], display1);
            var wbroadcaster_display2 = new Connector_v1(broadcaster.ConnectableOutputPoints[1], display2);

            Components.Add(sender1);
            Components.Add(sender2);
        }
    }
}
