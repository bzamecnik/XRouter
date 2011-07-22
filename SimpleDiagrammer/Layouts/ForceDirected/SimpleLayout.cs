using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleDiagrammer.Layouts.ForceDirected
{
    class SimpleLayout : LayoutAlgorithm
    {
        private static readonly Random random = new Random();

        private const double ATTRACTION_CONSTANT = 0.1;		// spring constant
        private const double REPULSION_CONSTANT = 10000;	// charge constant

        private const double DEFAULT_DAMPING = 0.5;
        private const int DEFAULT_SPRING_LENGTH = 100;

        private Dictionary<Node, NodeLayoutInfo> nodeToLayoutInfo = new Dictionary<Node, NodeLayoutInfo>();

        public override void UpdateLayout(IEnumerable<Node> nodes)
        {
            #region Separate overlapping nodes
            foreach (Node node1 in nodes) {
                foreach (Node node2 in nodes) {
                    if ((node1 != node2) && (CalcDistance(node1.Location, node2.Location) < 1.0d)) {
                        node1.Location += new Vector(random.NextDouble() * 100d - 50d, random.NextDouble() * 100d - 50d);
                    }
                }
            }
            #endregion

            #region Update nodeToLayoutInfo
            foreach (Node node in nodeToLayoutInfo.Keys.ToArray()) {
                if (!nodes.Contains(node)) {
                    nodeToLayoutInfo.Remove(node);
                }
            }
            foreach (Node node in nodes) {
                if (!nodeToLayoutInfo.ContainsKey(node)) {
                    NodeLayoutInfo layoutInfo = new NodeLayoutInfo(node, new MyVector(), new Point());
                    nodeToLayoutInfo.Add(node, layoutInfo);
                }
            }
            #endregion

            double totalDisplacement = 0;
            foreach (NodeLayoutInfo current in nodeToLayoutInfo.Values) {
                // express the node's current position as a vector, relative to the origin
                MyVector currentPosition = new MyVector(CalcDistance(new Point(), current.Node.Location), GetBearingAngle(new Point(), current.Node.Location));
                MyVector netForce = new MyVector(0, 0);

                // determine repulsion between nodes
                foreach (Node other in nodes) {
                    if (other != current.Node) netForce += CalcRepulsionForce(current.Node, other);
                }

                // determine attraction caused by connections
                foreach (Node child in current.Node.GetAdjacentNodes()) {
                    netForce += CalcAttractionForce(current.Node, child, DEFAULT_SPRING_LENGTH);
                }
                foreach (Node parent in nodes) {
                    if (parent.GetAdjacentNodes().Contains(current.Node)) netForce += CalcAttractionForce(current.Node, parent, DEFAULT_SPRING_LENGTH);
                }

                // apply net force to node velocity
                current.Velocity = (current.Velocity + netForce) * DEFAULT_DAMPING;

                // apply velocity to node position
                current.NextPosition = (currentPosition + current.Velocity).ToPoint();
            }

            // move nodes to resultant positions (and calculate total displacement)
            foreach (NodeLayoutInfo current in nodeToLayoutInfo.Values) {
                totalDisplacement += CalcDistance(current.Node.Location, current.NextPosition);
                current.Node.Location = current.NextPosition;
            }
        }

        /// <summary>
        /// Calculates the attraction force between two connected nodes, using the specified spring length.
        /// </summary>
        /// <param name="x">The node that the force is acting on.</param>
        /// <param name="y">The node creating the force.</param>
        /// <param name="springLength">The length of the spring, in pixels.</param>
        /// <returns>A Vector representing the attraction force.</returns>
        private MyVector CalcAttractionForce(Node x, Node y, double springLength)
        {
            double proximity = Math.Max(CalcDistance(x.Location, y.Location), 1d);

            // Hooke's Law: F = -kx
            double force = ATTRACTION_CONSTANT * Math.Max(proximity - springLength, 0);
            double angle = GetBearingAngle(x.Location, y.Location);

            return new MyVector(force, angle);
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The pixel distance between the two points.</returns>
        public static double CalcDistance(Point a, Point b)
        {
            double xDist = (a.X - b.X);
            double yDist = (a.Y - b.Y);
            return Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));
        }

        /// <summary>
        /// Calculates the repulsion force between any two nodes in the diagram space.
        /// </summary>
        /// <param name="x">The node that the force is acting on.</param>
        /// <param name="y">The node creating the force.</param>
        /// <returns>A Vector representing the repulsion force.</returns>
        private MyVector CalcRepulsionForce(Node x, Node y)
        {
            double proximity = Math.Max(CalcDistance(x.Location, y.Location), 1);

            // Coulomb's Law: F = k(Qq/r^2)
            double force = -(REPULSION_CONSTANT / Math.Pow(proximity, 2));
            double angle = GetBearingAngle(x.Location, y.Location);

            return new MyVector(force, angle);
        }

        /// <summary>
        /// Calculates the bearing angle from one point to another.
        /// </summary>
        /// <param name="start">The node that the angle is measured from.</param>
        /// <param name="end">The node that creates the angle.</param>
        /// <returns>The bearing angle, in degrees.</returns>
        private double GetBearingAngle(Point start, Point end)
        {
            Point half = new Point(start.X + ((end.X - start.X) / 2), start.Y + ((end.Y - start.Y) / 2));

            double diffX = half.X - start.X;
            double diffY = half.Y - start.Y;

            if (diffX == 0) diffX = 0.001;
            if (diffY == 0) diffY = 0.001;

            double angle;
            if (Math.Abs(diffX) > Math.Abs(diffY)) {
                angle = Math.Tanh(diffY / diffX) * (180.0 / Math.PI);
                if (((diffX < 0) && (diffY > 0)) || ((diffX < 0) && (diffY < 0))) angle += 180;
            } else {
                angle = Math.Tanh(diffX / diffY) * (180.0 / Math.PI);
                if (((diffY < 0) && (diffX > 0)) || ((diffY < 0) && (diffX < 0))) angle += 180;
                angle = (180 - (angle + 90));
            }

            return angle;
        }

        /// <summary>
        /// Private inner class used to track the node's position and velocity during simulation.
        /// </summary>
        private class NodeLayoutInfo
        {

            public Node Node;			// reference to the node in the simulation
            public MyVector Velocity;		// the node's current velocity, expressed in vector form
            public Point NextPosition;	// the node's position after the next iteration

            /// <summary>
            /// Initialises a new instance of the Diagram.NodeLayoutInfo class, using the specified parameters.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="velocity"></param>
            /// <param name="nextPosition"></param>
            public NodeLayoutInfo(Node node, MyVector velocity, Point nextPosition)
            {
                Node = node;
                Velocity = velocity;
                NextPosition = nextPosition;
            }
        }
    }
}
