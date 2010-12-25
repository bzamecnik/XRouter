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
    public partial class ComponentWatcherControl : UserControl
    {
        private OrientedGraph<Component> ComponentGraph { get; set; }

        private IComponentsDataStorage ComponentsDataStorage { get; set; }

        private List<FrameworkElement> edgesElements = new List<FrameworkElement>();

        public ComponentWatcherControl()
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
            Func<ObjectInfo, bool> obejctFilter = objectInfo => true;
            Func<ObjectLinkInfo, bool> referenceFilter = delegate(ObjectLinkInfo referenceInfo) {
                if (referenceInfo.IsInField) {
                    var hideReferenceAttributeType = typeof(HideReferenceAttribute);
                    bool hideReference = referenceInfo.ContainerField.GetCustomAttributes(hideReferenceAttributeType, true).Any();
                    if (hideReference) {
                        return false;
                    }
                }
                return true;
            };

            OrientedGraph<object> objectGraph = ObjectGraphBuilder.CreateGraph(roots, obejctFilter, referenceFilter);
            objectGraph.ContractNodes(obj => !Component.IsComponent(obj));
            var componentGraph = objectGraph.Clone(obj => new Component(obj, ComponentsDataStorage));
            return componentGraph;
        }

        private void RedrawComponents()
        {
            uiCanvas.Children.Clear();
            foreach (Component componentIterator in ComponentGraph.Nodes) {
                Component component = componentIterator;
                uiCanvas.Children.Add(component.RepresentationContainer);
                component.VisualBoundsChanged += delegate {
                    RedrawEdges();
                };
            }
            RedrawEdges();
        }

        private void RedrawEdges()
        {
            foreach (var element in edgesElements) {
                if (uiCanvas.Children.Contains(element)) {
                    uiCanvas.Children.Remove(element);
                }
            }
            edgesElements.Clear();

            foreach (var edge in ComponentGraph.GetAllEdges()) {
                FrameworkElement edgeUI = CreateEdgeRepresentation(edge.Source, edge.Target);
                edgesElements.Add(edgeUI);
                uiCanvas.Children.Add(edgeUI);
            }
        }

        private FrameworkElement CreateEdgeRepresentation(Component sourceNode, Component targetNode)
        {
            Point sourceEndPoint, targetEndPoint;
            ComputeLinkEnds(sourceNode.VisualBounds, targetNode.VisualBounds, 0, out sourceEndPoint, out targetEndPoint);
            sourceEndPoint.Y -= 10;
            targetEndPoint.Y -= 10;

            Vector relativeTargetLocation = targetEndPoint - sourceEndPoint;
            double lineLength = Math.Sqrt((relativeTargetLocation.X * relativeTargetLocation.X) + (relativeTargetLocation.Y * relativeTargetLocation.Y));
            double rotationAngle = Math.Atan2(relativeTargetLocation.Y, relativeTargetLocation.X) * 180d / Math.PI;

            Grid result = new Grid {
                ColumnDefinitions = { 
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                },
                Width = lineLength,
                RenderTransformOrigin = new Point(0, 0.5d),
                RenderTransform = new RotateTransform(rotationAngle)
            };
            Canvas.SetLeft(result, sourceEndPoint.X);
            Canvas.SetTop(result, sourceEndPoint.Y);
            Canvas.SetZIndex(result, -1);

            #region Create line
            Border line = new Border {
                Width = double.NaN,
                Height = 1,
                VerticalAlignment = VerticalAlignment.Center,                
                Background = Brushes.Black
            };
            Grid.SetColumn(line, 0);
            Grid.SetColumnSpan(line, 5);
            result.Children.Add(line);
            #endregion

            #region Create end arrow
            double arrowLength = 20;
            double arrowWidth = 15;
            Polyline endArrow = new Polyline {
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
            Grid.SetColumn(endArrow, 4);
            result.Children.Add(endArrow);
            #endregion

            return result;
        }
        
        #region Edge location computation support
        public void ComputeLinkEnds(Rect sourceBounds, Rect targetBounds, double positionOffset, out Point sourceEndPoint, out Point targetEndPoint)
        {
            Point sourceCenter = new Point(sourceBounds.X + sourceBounds.Width / 2, sourceBounds.Y + sourceBounds.Height / 2);
            Point targetCenter = new Point(targetBounds.X + targetBounds.Width / 2, targetBounds.Y + targetBounds.Height / 2);
            Vector direction = targetCenter - sourceCenter;

            Vector normal = new Vector(-direction.Y, direction.X);
            normal.Normalize();
            Point offsettedSourceCenter = sourceCenter + (normal * positionOffset);
            Point offsettedTargetCenter = targetCenter + (normal * positionOffset);

            sourceEndPoint = GetRectangleAndLineIntersection(sourceBounds, offsettedSourceCenter, offsettedTargetCenter);
            targetEndPoint = GetRectangleAndLineIntersection(targetBounds, offsettedTargetCenter, offsettedSourceCenter);
        }

        private Point GetRectangleAndLineIntersection(Rect rectangle, Point lineStart, Point lineEnd)
        {
            Point result = lineStart;
            if (GetLinesIntersection(rectangle.TopLeft, rectangle.TopRight, lineStart, lineEnd, ref result)) {
                return result;
            }
            if (GetLinesIntersection(rectangle.TopRight, rectangle.BottomRight, lineStart, lineEnd, ref result)) {
                return result;
            }
            if (GetLinesIntersection(rectangle.BottomRight, rectangle.BottomLeft, lineStart, lineEnd, ref result)) {
                return result;
            }
            if (GetLinesIntersection(rectangle.BottomLeft, rectangle.TopLeft, lineStart, lineEnd, ref result)) {
                return result;
            }
            return result;
        }

        private bool GetLinesIntersection(Point line1Point1, Point line1Point2, Point line2Point1, Point line2Point2, ref Point intersection)
        {
            double q = (line1Point1.Y - line2Point1.Y) * (line2Point2.X - line2Point1.X) - (line1Point1.X - line2Point1.X) * (line2Point2.Y - line2Point1.Y);
            double d = (line1Point2.X - line1Point1.X) * (line2Point2.Y - line2Point1.Y) - (line1Point2.Y - line1Point1.Y) * (line2Point2.X - line2Point1.X);
            if (d == 0) {
                return false;
            }

            double r = q / d;
            q = (line1Point1.Y - line2Point1.Y) * (line1Point2.X - line1Point1.X) - (line1Point1.X - line2Point1.X) * (line1Point2.Y - line1Point1.Y);
            double s = q / d;
            if (r < 0 || r > 1 || s < 0 || s > 1) {
                return false;
            }

            intersection.X = line1Point1.X + (int)(0.5 + r * (line1Point2.X - line1Point1.X));
            intersection.Y = line1Point1.Y + (int)(0.5 + r * (line1Point2.Y - line1Point1.Y));
            return true;
        }

        #endregion

        private void uiZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scale = uiZoom.Value;
            if (uiCanvas != null) {
                uiCanvas.LayoutTransform = new ScaleTransform(scale, scale);                
            }
            uiZoomHeader.Text = string.Format("Zoom {0:0.00}x", scale);
        }
    }
}
