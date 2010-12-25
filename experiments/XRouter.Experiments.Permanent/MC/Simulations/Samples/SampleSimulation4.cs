using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Simulator;
using XRouter.ComponentWatching;
using System.Windows;
using System.Windows.Controls;

namespace XRouter.Experiments.Permanent.MC.Simulations.Samples
{
    public class SampleSimulation4 : Simulation
    {
        public SampleSimulation4()
            : base("MC_SampleSimulation4")
        {
            WatcherConfiguration.AllObjectsAreComponents = true;

            MyElement root = new MyElement("Root");
            MyElement child1 = new MyElement("Child1");
            MyElement child2 = new MyElement("Child2");
            root.Children.Add(child1);
            root.Children.Add(child2);

            Components.Add(root);
        }

        //[WatchableComponent("MyElement")]
        private class MyElement
        {
            public string Name { get; set; }

            public List<MyElement> Children { get; private set; }

            public MyElement(string name)
            {
                Name = name;
                Children = new List<MyElement>();
            }
        }

        //private class MyElement : IWatchableComponent
        //{
        //    public string Name { get; set; }

        //    public List<MyElement> Children { get; private set; }

        //    public MyElement(string name)
        //    {
        //        Name = name;
        //        Children = new List<MyElement>();
        //    }

        //    #region Imeplementation of IWatchableComponent            
        //    string IWatchableComponent.ComponentName { get { return Name; } }

        //    FrameworkElement IWatchableComponent.CreateRepresentation()
        //    {
        //        return null;

        //        //return new Border {
        //        //    Width = 100,
        //        //    Height = 30,
        //        //    Child = new Button {
        //        //        Width = 80,
        //        //        Height = 20,
        //        //        Content = "Button"
        //        //    }
        //        //};
        //    }
        //    #endregion
        //}
    }
}
