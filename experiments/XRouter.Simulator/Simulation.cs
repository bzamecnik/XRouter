using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.ComponentWatching;

namespace XRouter.Simulator
{
    public class Simulation
    {
        public string Name { get; protected set; }

        public ICollection<object> Components { get; private set; }

        public IComponentsDataStorage Storage { get; private set; }

        public WatcherConfiguration WatcherConfiguration { get; private set; }

        public Simulation(string name)
        {
            Name = name;

            Storage = new FileComponentsDataStorage(this);
            Components = new List<object>();
            WatcherConfiguration = new WatcherConfiguration();
        }
    }
}
