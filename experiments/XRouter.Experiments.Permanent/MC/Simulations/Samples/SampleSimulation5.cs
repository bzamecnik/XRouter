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
    public class SampleSimulation5 : Simulation
    {
        public SampleSimulation5()
            : base("MC_SampleSimulation5")
        {
            IMessageProducent producent = new MC.Components.Common.ManualMessageSender_v1("Producent");
            IMessageConsument consument = new MC.Components.Common.MessageDisplay_v1("Consument");

            producent.MessageSent += consument.Send;

            Components.Add(producent);
        }
    }
}
