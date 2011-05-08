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

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation6 : Simulation
    {
        public SampleSimulation6()
            : base("MC_SampleSimulation6")
        {
            IMessageProducent producent = new MC.Components.Common.ManualMessageSender_v1("Producent");
            IMessageConsument consument = new MC.Components.Common.MessageDisplay_v1("Consument");

            var connector = new MC.Components.Common.Connector_v1(producent, consument);

            Components.Add(producent);
        }
    }
}
