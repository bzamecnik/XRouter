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

namespace XRouter.Gui.CommonControls
{
    /// <summary>
    /// Interaction logic for TokenSelectionEditor.xaml
    /// </summary>
    public partial class TokenSelectionEditor : UserControl
    {
        private TokenSelection _selection;
        public TokenSelection Selection {
            get { return _selection; }
            set {
                _selection = value;
                uiPattern.Text = _selection.SelectionPattern;
            }
        }

        public TokenSelectionEditor()
        {
            InitializeComponent();
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
            Selection.SelectionPattern = pattern;
        }
    }
}
