using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using XRouter.Experiments.Permanent.MC.Components.Common;

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation2 : Simulation
    {
        public SampleSimulation2()
            : base("MC_SampleSimulation2")
        {
            var sender = new ManualMessageSender_v1("Sender");
            var display = new MessageDisplay_v1("Display");
            var connect_sender_display = new Connector_v1(sender, display);
            
            Components.Add(sender);
        }
    }
}
