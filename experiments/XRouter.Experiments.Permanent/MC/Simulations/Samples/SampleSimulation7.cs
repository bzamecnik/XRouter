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
    public class SampleSimulation7 : Simulation
    {
        public SampleSimulation7()
            : base("MC_SampleSimulation7")
        {
            var sender1 = new ManualMessageSender_v1("Sender1");
            var sender2 = new ManualMessageSender_v1("Sender2");
            var display1 = new MessageDisplay_v1("Display1");
            var display2 = new MessageDisplay_v1("Display2");                        
            var broadcaster = new BroadcastingMessageFlowProcessor_v1("Broadcaster", 2, 2);
            //var broadcaster = new Broadcaster("Broadcaster", 2, 2);

            var sender1_wfp = new Connector_v1(sender1, broadcaster.ConnectableInputPoints[0]);
            var sender2_wfp = new Connector_v1(sender2, broadcaster.ConnectableInputPoints[1]);
            var broadcaster_display1 = new Connector_v1(broadcaster.ConnectableOutputPoints[0], display1);
            var broadcaster_display2 = new Connector_v1(broadcaster.ConnectableOutputPoints[1], display2);

            Components.Add(sender1);
            Components.Add(sender2);
        }

        private class Broadcaster : MesageFlowProcessor_v1
        {
            public Broadcaster(string name, int inputPoints, int outputPoints)
                : base(name, inputPoints, outputPoints)
            {
            }

            //Design pattern: Template method
            protected override bool HandleMessage(Message message, MesageFlowProcessor_v1.InputPoint source)
            {
                foreach (var outputPoint in OutputPoints) {
                    outputPoint.Send(message);
                }
                return true;
            }
        }
    }
}
