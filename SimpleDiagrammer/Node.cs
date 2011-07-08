using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleDiagrammer
{
    class Node
    {
        private static Random random = new Random();

        public FrameworkElement UIFrame { get; private set; }

        public IInternalNodePresenter Presenter { get; private set; }

        public GraphCanvas GraphCanvas { get; private set; }

        public object NodeObject { get; private set; }

        public Point Location { get; set; }

        public Vector CanvasLocationOffset { get; set; }

        public Size Size {
            get {
                return new Size(UIFrame.ActualWidth, UIFrame.ActualHeight);
            }
        }

        private FrameworkElement dragArea;
        private bool isDragging;
        private Vector dragOffset;

        public Node(IInternalNodePresenter preseneter, GraphCanvas graphCanvas, object nodeObject)
        {
            Presenter = preseneter;
            GraphCanvas = graphCanvas;
            NodeObject = nodeObject;

            ContentControl border = new ContentControl();
            border.Content = Presenter.Content;

            UIFrame = border;
            Location = new Point(random.NextDouble() * 100d - 50d, random.NextDouble() * 100d - 50d);
            Canvas.SetZIndex(UIFrame, 1);

            #region Prepare dragging
            dragArea = Presenter.DragArea;
            if (dragArea == null) {
                dragArea = UIFrame;
            }
            dragArea.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
            dragArea.MouseMove += DragArea_MouseMove;
            dragArea.MouseLeftButtonUp += DragArea_MouseLeftButtonUp;
            #endregion
        }

        private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point locationOnNode = e.GetPosition(UIFrame);
            dragOffset = new Vector(locationOnNode.X, locationOnNode.Y);
            dragArea.CaptureMouse();
            isDragging = true;
        }

        private void DragArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            dragArea.ReleaseMouseCapture();
        }

        private void DragArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) {
                return;
            }

            Point newLocation = e.GetPosition(GraphCanvas);
            newLocation -= dragOffset;
            newLocation -= CanvasLocationOffset;

            Location = newLocation;
            System.Diagnostics.Trace.WriteLine(Location);
        }        

        public IEnumerable<Node> GetAdjacentNodes()
        {
            List<Node> result = new List<Node>();
            var edges = GraphCanvas.GraphPresenter.GetEdges().Select(eo => GraphCanvas.GetEdgeByObject(eo));
            foreach (Edge edge in edges) {
                if (edge != null) {
                    Node adjacentNode = null;
                    if (edge.Source == this) { adjacentNode = edge.Target; }
                    if (edge.Target == this) { adjacentNode = edge.Source; }
                    if (adjacentNode != null) {
                        result.Add(adjacentNode);
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            return NodeObject.ToString();
        }
    }
}
