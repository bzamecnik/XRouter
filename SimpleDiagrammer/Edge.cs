using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using SimpleDiagrammer.Support;

namespace SimpleDiagrammer
{
    class Edge
    {
        public object EdgeObject { get; private set; }
        public IInternalEdgePresenter Presenter { get; private set; }
        public Node Source { get; private set; }
        public Node Target { get; private set; }

        private Grid linkGrid;
        private RotateTransform linkGridRotateTransform;
        private Border linkLine;

        public FrameworkElement UILink { get; private set; }
        public FrameworkElement StartDecoration { get; private set; }
        public FrameworkElement EndDecoration { get; private set; }
        public FrameworkElement CenterDecoration { get; private set; }
        public Line LinkInnerLine { get; private set; }

        public Edge(IInternalEdgePresenter presenter, object edgeObject, Node source, Node target, FrameworkElement endDecoration = null, FrameworkElement centerDecoration = null, FrameworkElement startDecoration = null)
        {
            Presenter = presenter;
            EdgeObject = edgeObject;
            Source = source;
            Target = target;
            StartDecoration = startDecoration;
            EndDecoration = endDecoration;
            CenterDecoration = centerDecoration;

            #region Prepare grid
            linkGridRotateTransform = new RotateTransform();
            linkGrid = new Grid {
                ColumnDefinitions = { 
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                },
                RenderTransformOrigin = new Point(0, 0.5d),
                RenderTransform = linkGridRotateTransform
            };
            #endregion

            #region Prepare link line
            LinkInnerLine = new Line {
                StrokeThickness = 1d,
                Stroke = Brushes.Black,
            };
            linkLine = new Border {
                Height = 20,
                Background = Brushes.Transparent,
                Child = LinkInnerLine
            };

            linkLine.Width = double.NaN;
            linkLine.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(linkLine, 0);
            Grid.SetColumnSpan(linkLine, 5);
            linkGrid.Children.Add(linkLine);
            #endregion

            #region Prepare decorations
            if (StartDecoration != null) {
                StartDecoration.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(StartDecoration, 0);
                linkGrid.Children.Add(StartDecoration);
            }
            if (CenterDecoration != null) {
                CenterDecoration.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(CenterDecoration, 2);
                linkGrid.Children.Add(CenterDecoration);
            }
            if (EndDecoration != null) {
                EndDecoration.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(EndDecoration, 4);
                linkGrid.Children.Add(EndDecoration);
            }
            #endregion

            UILink = linkGrid;
            Canvas.SetZIndex(UILink, Presenter.ZIndex);
        }

        public void UpdatePosition()
        {
            Point sourceLocation = new Point(Canvas.GetLeft(Source.UIFrame), Canvas.GetTop(Source.UIFrame));
            Point targetLocation = new Point(Canvas.GetLeft(Target.UIFrame), Canvas.GetTop(Target.UIFrame));
            Point sourceEndPoint, targetEndPoint;
            MathUtils.ComputeLinkBetweenRectangles(new Rect(sourceLocation, Source.Size), new Rect(targetLocation, Target.Size), 0, out sourceEndPoint, out targetEndPoint);
            sourceEndPoint.Y -= UILink.ActualHeight / 2d;
            targetEndPoint.Y -= UILink.ActualHeight / 2d;

            Canvas.SetLeft(UILink, sourceEndPoint.X);
            Canvas.SetTop(UILink, sourceEndPoint.Y);

            Vector relativeTargetLocation = targetEndPoint - sourceEndPoint;

            LinkInnerLine.Y1 = LinkInnerLine.Y2 = UILink.ActualHeight / 2d;
            LinkInnerLine.X2 = relativeTargetLocation.Length;
            
            double lineLength = Math.Sqrt((relativeTargetLocation.X * relativeTargetLocation.X) + (relativeTargetLocation.Y * relativeTargetLocation.Y));
            double rotationAngle = Math.Atan2(relativeTargetLocation.Y, relativeTargetLocation.X) * 180d / Math.PI;
            linkGrid.Width = lineLength;
            linkGridRotateTransform.Angle = rotationAngle;
        }

        public static FrameworkElement CreateEndArrow()
        {
            double arrowLength = 20;
            double arrowWidth = 15;
            var result = new Polyline {
                Width = arrowLength,
                Height = arrowWidth,
                Fill = new SolidColorBrush(Color.FromArgb(0xD0, 0x00, 0x00, 0x00)),
                Points = {
                    new Point(0, 0),
                    new Point(arrowLength, arrowWidth / 2d),
                    new Point(0, arrowWidth),
                    new Point(arrowLength / 2d, arrowWidth / 2d)
                }
            };
            return result;
        }
    }
}
