using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRouter.Utils.ObjectExploring;
using XRouter.Utils.DataStructures;

namespace XRouter.ComponentWatching
{
    /// <summary>
    /// Interaction logic for ComponentWatcher.xaml
    /// </summary>
    public partial class ComponentWatcher : UserControl
    {
        private OrientedGraph<Component> ComponentGraph { get; set; }

        private IComponentsDataStorage ComponentsDataStorage { get; set; }

        public ComponentWatcher()
        {
            InitializeComponent();

            ComponentGraph = new OrientedGraph<Component>();
        }

        public void LoadComponents(object root, IComponentsDataStorage storage)
        {
            LoadComponents(new object[] { root }, storage);
        }

        public void LoadComponents(IEnumerable<object> roots, IComponentsDataStorage storage)
        {
            ComponentsDataStorage = storage;
            ComponentGraph = CreateComponentGraph(roots);
            RedrawComponents();
        }

        private OrientedGraph<Component> CreateComponentGraph(IEnumerable<object> roots)
        {            
            OrientedGraph<object> objectGraph = ObjectGraphBuilder.CreateGraph(roots);
            objectGraph.ContractNodes(obj => Component.IsComponent(obj));
            var componentGraph = objectGraph.Clone(obj => new Component(obj, ComponentsDataStorage));
            return componentGraph;
        }

        private void RedrawComponents()
        {
            uiCanvas.Children.Clear();
            foreach (Component component in ComponentGraph.Nodes) {
                uiCanvas.Children.Add(component.Representation);
            }
        }
    }
}
