using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDiagrammer;
using XRouter.Common.MessageFlowConfig;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowNodePresenter : NodePresenter<NodeConfiguration>
    {
        public MessageflowNodePresenter(NodeConfiguration node)
            : base(node)
        {
            Label label = new Label {
                Content = node.Name,
                Foreground = Brushes.Black
            };
            Border border = new Border {
                Child = label,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.Yellow,
                Padding = new Thickness(5),
                CornerRadius = new CornerRadius(5)
            };

            Content = border;
        }
    }
}
