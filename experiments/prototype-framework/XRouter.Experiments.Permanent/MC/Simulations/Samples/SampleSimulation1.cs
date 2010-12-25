using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation1 : Simulation
    {
        public SampleSimulation1() : base("MC_SampleSimulation1")
        {
            WatcherConfiguration.AllObjectsAreComponents = true;

            var myObject = "text";
            Components.Add(myObject);
        }
    }
}
