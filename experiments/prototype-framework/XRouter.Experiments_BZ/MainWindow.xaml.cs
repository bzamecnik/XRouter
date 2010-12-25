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
using XRouter.Simulator;
using MC = XRouter.Experiments.Permanent.MC;
using BZ = XRouter.Experiments.Permanent.BZ;
using PS = XRouter.Experiments.Permanent.PS;
using SB = XRouter.Experiments.Permanent.SB;
using TK = XRouter.Experiments.Permanent.TK;

namespace XRouter.Experiments_BZ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Simulation simulation = new MC.Simulations.Samples.SampleSimulation1();

            var uiSimulator = new SimulatorControl(simulation);
            uiMainContainer.Child = uiSimulator;
        }
    }
}
