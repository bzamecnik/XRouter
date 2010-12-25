using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation2 : Simulation
    {
        public SampleSimulation2() : base("MC_SampleSimulation2")
        {
            WatcherConfiguration.AllObjectsAreComponents = true;

            List<string> myList = new List<string>();
            myList.Add("a");
            myList.Add("b");
            myList.Add("c");

            Components.Add(myList);
        }
    }
}
