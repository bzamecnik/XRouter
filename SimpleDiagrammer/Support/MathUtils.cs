using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleDiagrammer.Support
{
    /// <summary>
    /// Math utils.
    /// </summary>
    /// <remarks>
    /// In this class, a <b>link</b> between rectangles is defined as a line from the centre of a source rectangle to the centre of a target rectangle
    /// without points which are contained in these rectangles. Such link is suitable to be drawn as an edge between nodes in a diagram.
    /// </remarks>
    class MathUtils
    {
        public static double GetLinkLengthBetweenRectangles(Rect rectangle1, Rect rectangle2)
        {
            if (rectangle1.IntersectsWith(rectangle2)) {
                return 0;
            }
            Point linkStart, linkEnd;
            ComputeLinkBetweenRectangles(rectangle1, rectangle2, 0, out linkStart, out linkEnd);
            Vector link = linkEnd - linkStart;
            return link.Length;
        }

        public static void ComputeLinkBetweenRectangles(Rect sourceRectangle, Rect targetRectangle, double positionOffset, out Point linkStart, out Point linkEnd)
        {
            Point sourceCenter = new Point(sourceRectangle.X + sourceRectangle.Width / 2, sourceRectangle.Y + sourceRectangle.Height / 2);
            Point targetCenter = new Point(targetRectangle.X + targetRectangle.Width / 2, targetRectangle.Y + targetRectangle.Height / 2);
            Vector direction = targetCenter - sourceCenter;

            Vector normal = new Vector(-direction.Y, direction.X);
            normal.Normalize();
            Point offsettedSourceCenter = sourceCenter + (normal * positionOffset);
            Point offsettedTargetCenter = targetCenter + (normal * positionOffset);

            linkStart = GetRectangleAndLineIntersection(sourceRectangle, offsettedSourceCenter, offsettedTargetCenter);
            linkEnd = GetRectangleAndLineIntersection(targetRectangle, offsettedTargetCenter, offsettedSourceCenter);
        }

        public static Point GetRectangleAndLineIntersection(Rect rectangle, Point lineStart, Point lineEnd)
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

        public static bool GetLinesIntersection(Point line1Point1, Point line1Point2, Point line2Point1, Point line2Point2, ref Point intersection)
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
    }
}
