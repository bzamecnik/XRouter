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
using XRouter.Gui.Utils;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    /// <summary>
    /// Interaction logic for TargetNodeSelector.xaml
    /// </summary>
    public partial class TargetNodeSelector : UserControl
    {
        private NodeSelectionManager selectionManger;
        private NodeConfiguration source;
        private Func<NodeConfiguration> targetGetter;
        private Action<NodeConfiguration> targetSetter;

        public TargetNodeSelector()
        {
            InitializeComponent();
        }

        internal void Initialize(NodeSelectionManager selectionManger, NodeConfiguration source, Func<NodeConfiguration> targetGetter, Action<NodeConfiguration> targetSetter)
        {
            this.selectionManger = selectionManger;
            this.source = source;
            this.targetGetter = targetGetter;
            this.targetSetter = targetSetter;
        }

        private void uiSelect_Click(object sender, RoutedEventArgs e)
        {
            if (uiSelect.IsChecked == true) {
                selectionManger.NodeSelecting += SelectNode;
            } else {
                selectionManger.NodeSelecting -= SelectNode;
            }
        }

        private void SelectNode(object sender, NodeSelectingEventArgs e)
        {
            e.IsCancelled = true;
            uiSelect.IsChecked = false;
            selectionManger.NodeSelecting -= SelectNode;

            selectionManger.MessageflowGraphPresenter.HighlightNodes();
            targetSetter(e.NewSelectedNode);
            selectionManger.MessageflowGraphPresenter.RaiseGraphChanged();
            selectionManger.MessageflowGraphPresenter.HighlightNodes(source, targetGetter());
            ThreadUtils.InvokeLater(TimeSpan.FromSeconds(0.5), delegate {
                selectionManger.MessageflowGraphPresenter.HighlightNodes();
            });
        }

        private void uiSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            selectionManger.MessageflowGraphPresenter.HighlightNodes(source, targetGetter());
        }

        private void uiSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            if (uiSelect.IsChecked == false) {
                selectionManger.MessageflowGraphPresenter.HighlightNodes();
            }
        }
    }
}
