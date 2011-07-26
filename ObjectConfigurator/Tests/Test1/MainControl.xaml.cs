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
using System.Xml.Linq;

namespace ObjectConfigurator.Tests.Test1
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private ConfigurationEditor editor;
        private XDocument savedConfig;

        public MainControl()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainControl_Loaded);
        }

        void MainControl_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                ConfigurableObject obj = new ConfigurableObject();
                obj.SetValues();

                savedConfig = Configurator.SaveConfiguration(obj);
                obj = new ConfigurableObject();
                Configurator.LoadConfiguration(obj, savedConfig);

                editor = Configurator.CreateEditor(typeof(ConfigurableObject));
                uiEditorContainer.Child = editor;
                editor.LoadConfiguration(savedConfig);
            } catch (Exception ex) {
                throw;
            }
        }

        private void uiLoad_Click(object sender, RoutedEventArgs e)
        {
            editor.LoadConfiguration(savedConfig);
        }

        private void uiSave_Click(object sender, RoutedEventArgs e)
        {
            savedConfig = editor.SaveConfiguration();
        }
    }
}
