using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using System.Xml.Linq;

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation3 : Simulation
    {
        public SampleSimulation3() : base("MC_SampleSimulation3")
        {
            WatcherConfiguration.AllObjectsAreComponents = true;
            //WatcherConfiguration.HidePrimitiveTypes = true;
            //WatcherConfiguration.HideValueTypes = true;
            //WatcherConfiguration.CustomVisibilityFilter = obj => (!(obj is Delegate)) && (!(obj is Array));


            XDocument xDoc = XDocument.Parse(@"
<Root>
    <Child Name='Child1'/>
    <Child Name='Child2'>text</Child>
</Root>");

            Components.Add(xDoc);
        }
    }
}
