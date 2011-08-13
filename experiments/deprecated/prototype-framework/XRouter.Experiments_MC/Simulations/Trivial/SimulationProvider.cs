using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using XRouter.ComponentWatching;
using XRouter.Simulator;

namespace XRouter.Experiments_MC.Simulations.Trivial
{
    public class SimulationProvider
    {
        public static Simulation GetSimulation()
        {
            Simulation result = new Simulation("MC_Trivial");
            
            Component1 component1 = new Component1();
            component1.component2.component3.component1 = component1;
            result.Components.Add(component1);

            return result;
        }

        [WatchableComponent("component1")]
        private class Component1
        {
            public Component2 component2 = new Component2();
        }

        [WatchableComponent("component2")]
        private class Component2
        {
            public Component3 component3 = new Component3();
            public Component4 component4 = new Component4();
        }
        
        private class Component3 : IWatchableComponent
        {
            public Component1 component1;

            string IWatchableComponent.ComponentName { get { return "component3"; } }

            FrameworkElement IWatchableComponent.CreateRepresentation()
            {
                var result = new Border {
                    Width = 200,
                    Height = 150,
                    Child = new Button {
                        Width=100,
                        Height=20,
                        Content = "Button"
                    }
                };
                return result;
            }
        }

        [WatchableComponent("component4")]
        private class Component4
        {
        }
    }
}
