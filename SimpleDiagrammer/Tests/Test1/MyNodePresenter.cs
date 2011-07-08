using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace SimpleDiagrammer.Tests.Test1
{
    class MyNodePresenter : NodePresenter<Node>
    {
        private MyGraphPresenter graphPresenter;

        public MyNodePresenter(Node node, MyGraphPresenter graphPresenter)
            : base(node)
        {
            this.graphPresenter = graphPresenter;

            Border border = new Border {
                Padding = new Thickness(10),
                Background = Brushes.Yellow,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                CornerRadius = new CornerRadius(10),
                Child = new TextBlock {
                    FontSize = 14,
                    Text = node.Content.ToString()
                }
            };

            border.MouseRightButtonUp += border_MouseRightButtonUp;
            border.MouseUp += border_MouseUp;

            Content = border;
        }

        void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle) {
                graphPresenter.RemoveNode(Node);
            }
        }

        void border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Node newNode = new Node();
            newNode.Content = Node.Content.ToString() + "_child";
            graphPresenter.AddNode(newNode);
            graphPresenter.AddEdge(Node, newNode);
        }
    }
}
