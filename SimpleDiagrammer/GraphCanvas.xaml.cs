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
using SimpleDiagrammer.Layouts;
using SimpleDiagrammer.Layouts.ForceDirected;
using System.Windows.Threading;

namespace SimpleDiagrammer
{
    /// <summary>
    /// Interaction logic for GraphCanvas.xaml
    /// </summary>
    public partial class GraphCanvas : UserControl
    {
        internal IInternalGraphPresenter GraphPresenter { get; private set; }

        private IList<Node> Nodes { get; set; }
        private IList<Edge> Edges { get; set; }

        private LayoutAlgorithm layoutAlgorithm;

        public Canvas Canvas { get; private set; }

        public Vector CanvasLocationOffset { get; private set; }

        private DispatcherTimer layoutUpdateTimer;

        internal GraphCanvas(IInternalGraphPresenter graphPresenter)
        {
            InitializeComponent();

            layoutAlgorithm = new ForceDirectedLayout();

            Canvas = uiCanvas;
            Nodes = new List<Node>();
            Edges = new List<Edge>();
            GraphPresenter = graphPresenter;
            Loaded += GraphCanvas_Loaded;
            Unloaded += GraphCanvas_Unloaded;
        }

        private void GraphCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            GraphPresenter.GraphChanged += GraphPresenter_GraphChanged;
            UpdateDiagram();

            layoutUpdateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.ApplicationIdle, delegate {
                UpdateDiagramLayout();
            }, Dispatcher);
        }

        private void GraphCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            layoutUpdateTimer.Stop();
        }

        private void UpdateDiagram()
        {
            #region Update nodes
            var oldNodes = Nodes.ToArray();
            Nodes.Clear();
            foreach (var nodeObject in GraphPresenter.GetNodes()) {
                Node node = oldNodes.FirstOrDefault(n => n.NodeObject == nodeObject);
                if (node == null) {
                    var nodePresenter = GraphPresenter.CreateNodePresenter(nodeObject);
                    node = new Node(nodePresenter, this, nodeObject);
                }
                Nodes.Add(node);
            }
            #endregion

            #region Update edges
            var oldEdges = Edges.ToArray();
            Edges.Clear();
            foreach (var edgeObject in GraphPresenter.GetEdges()) {
                Edge edge = oldEdges.FirstOrDefault(e => e.EdgeObject == edgeObject);
                if (edge == null) {
                    var edgePresenter = GraphPresenter.CreateEdgePresenter(edgeObject);
                    Node sourceNode = GetNodeByObject(edgePresenter.Source);
                    Node targetNode = GetNodeByObject(edgePresenter.Target);
                    edge = new Edge(edgePresenter, edgeObject, sourceNode, targetNode, Edge.CreateEndArrow());
                }
                Edges.Add(edge);
            }
            #endregion
        }

        private void UpdateDiagramLayout()
        {
            List<UIElement> oldCanvasChildren = Canvas.Children.OfType<UIElement>().ToList();

            #region Compute layout of nodes
            foreach (Node node in Nodes) {
                if (oldCanvasChildren.Contains(node.UIFrame)) {
                    oldCanvasChildren.Remove(node.UIFrame);
                } else {
                    uiCanvas.Children.Add(node.UIFrame);
                }
            }
            uiCanvas.UpdateLayout();
            layoutAlgorithm.UpdateLayout(Nodes);
            #endregion

            #region Compute location on canvas from logical location
            double minX = Nodes.Min(n => n.Location.X);
            double minY = Nodes.Min(n => n.Location.Y);
            double maxX = Nodes.Max(n => n.Location.X + n.Size.Width);
            double maxY = Nodes.Max(n => n.Location.Y + n.Size.Height);
            double maxAbsX = Math.Max(Math.Abs(minX), Math.Abs(maxX));
            double maxAbsY = Math.Max(Math.Abs(minY), Math.Abs(maxY));
            uiCanvas.Width = maxAbsX * 2;
            uiCanvas.Height = maxAbsY * 2;
            CanvasLocationOffset = new Vector(maxAbsX, maxAbsY);
            foreach (Node node in Nodes) {
                Point canvasLocation = node.Location + CanvasLocationOffset;
                Canvas.SetLeft(node.UIFrame, canvasLocation.X);
                Canvas.SetTop(node.UIFrame, canvasLocation.Y);
            }
            #endregion

            #region Update layout of edges
            foreach (Edge edge in Edges) {
                if (oldCanvasChildren.Contains(edge.UILink)) {
                    oldCanvasChildren.Remove(edge.UILink);
                } else {
                    uiCanvas.Children.Add(edge.UILink);
                }
            }
            uiCanvas.UpdateLayout();
            foreach (Edge edge in Edges) {
                edge.UpdatePosition();
            }
            #endregion

            foreach (UIElement canvasChild in oldCanvasChildren) {
                Canvas.Children.Remove(canvasChild);
            }
        }

        private void GraphPresenter_GraphChanged()
        {
            Dispatcher.Invoke(new Action(delegate {
                UpdateDiagram();
            }));
        }

        internal Node GetNodeByObject(object nodeObject)
        {
            return Nodes.FirstOrDefault(n => n.NodeObject.Equals(nodeObject));
        }

        internal Edge GetEdgeByObject(object edgeObject)
        {
            return Edges.FirstOrDefault(e => e.EdgeObject.Equals(edgeObject));
        }
    }
}
