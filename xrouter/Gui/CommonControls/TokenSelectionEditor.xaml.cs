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
using XRouter.Common.MessageFlowConfig;
using ObjectConfigurator;
using System.Xml.Linq;

namespace XRouter.Gui.CommonControls
{
    /// <summary>
    /// Interaction logic for TokenSelectionEditor.xaml
    /// </summary>
    public partial class TokenSelectionEditor : UserControl, ICustomConfigurationValueEditor
    {
        public event Action ValueChanged = delegate { };

        FrameworkElement ICustomConfigurationValueEditor.Representation { get { return this; } }

        private TokenSelection _selection;
        public TokenSelection Selection {
            get { return _selection; }
            set {
                _selection = value;
                uiPattern.Text = _selection.SelectionPattern;
                CheckValue();
            }
        }

        public TokenSelectionEditor()
        {
            InitializeComponent();

            Selection = new TokenSelection();
        }

        private void uiPattern_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangePattern(uiPattern.Text);
        }

        private void uiPattern_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                ChangePattern(uiPattern.Text);
            }
            if (e.Key == Key.Escape) {
                uiPattern.Text = Selection.SelectionPattern;
            }
        }

        private void ChangePattern(string pattern)
        {
            if (TokenSelection.IsPatternValid(pattern)) {
                Selection.SelectionPattern = pattern;
                ValueChanged();
            }
        }

        bool ICustomConfigurationValueEditor.WriteToXElement(XElement target)
        {
            target.Value = Selection.SelectionPattern;
            return true;
        }

        void ICustomConfigurationValueEditor.ReadFromXElement(XElement source)
        {
            Selection.SelectionPattern = source.Value;
            uiPattern.Text = Selection.SelectionPattern;
            CheckValue();
        }

        private void uiPattern_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValue();
        }

        private void CheckValue()
        {
            bool isValid = TokenSelection.IsPatternValid(uiPattern.Text);
            if (isValid) {
                uiPattern.Background = Brushes.White;
            } else {
                uiPattern.Background = Brushes.LightSalmon;
            }
        }
    }
}
