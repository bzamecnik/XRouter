using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using XRouter.Experiments.Permanent.MC.Components;
using XRouter.Experiments.Permanent.MC.Components.Common;

namespace XRouter.Experiments_MC.Simulations.Tests
{
    class TestSimulation1 : Simulation
    {
        public TestSimulation1()
            : base("MC_TestSimulation1")
        {
            var sender = new ManualMessageSender("Sender");
            var display = new MessageDisplay("Display");
            var connect_sender_display = new Connector(sender, display);
            

            Components.Add(sender);
            Components.Add(display);
            Components.Add(connect_sender_display);
        }
    }
}
