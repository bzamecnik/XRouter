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
using XRouter.ComponentWatching;

namespace XRouter.Simulator
{
    /// <summary>
    /// Interaction logic for SimulatorControl.xaml
    /// </summary>
    public partial class SimulatorControl : UserControl
    {
        public Simulation Simulation { get; private set; }

        public SimulatorControl(Simulation simulation)
        {
            InitializeComponent();

            Simulation = simulation;

            Loaded += SimulatorControl_Loaded;
        }

        void SimulatorControl_Loaded(object sender, RoutedEventArgs e)
        {
            ComponentWatcherControl uiWatcher = new ComponentWatcherControl();
            uiComponentWatcherContainer.Child = uiWatcher;

            uiWatcher.LoadComponents(Simulation.Components, Simulation.Storage);
        }
    }
}
