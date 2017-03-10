using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner
{
    // Note: I couldn't find a useful open source library that does
    // orthogonal routing so started to write something on my own.
    // Categorize this as a quick and dirty short term solution.
    // I will keep on searching.

    // Helper class to provide an orthogonal connection path
    internal class PathFinder
    {
        private const int margin = 20;
        private const int distance = 40;
        private const int leftmargin = 55;
        private const int rightmargin = 45;

        internal static List<Point> GetConnectionLine1(ConnectorInfo source, ConnectorInfo sink, bool showLastLine)
        {
            List<Point> linePoints = new List<Point>();
            Rect rectSource = GetRectWithMargin(source, margin);
            Point startPoint = GetOffsetPoint(source, rectSource);
            Rect rectSource1 = GetRectWithMargin(sink, margin);
            Point sinkPoint = GetOffsetPoint(sink, rectSource1);
            Point endPoint = sinkPoint;

            linePoints.Add(startPoint);
            Point currentPoint = startPoint;

            if (startPoint.X < sinkPoint.X)
            {
                linePoints.Clear();
                Point p1 = new Point();
                p1.X = startPoint.X + (sinkPoint.X - startPoint.X) / 2;
                p1.Y = startPoint.Y;
                Point p2 = new Point();
                p2.X = sinkPoint.X - (sinkPoint.X - startPoint.X) / 2;
                p2.Y = sinkPoint.Y;
                linePoints.Add(startPoint);
                linePoints.Add(p1);
                linePoints.Add(p2);
                linePoints.Add(sinkPoint);
            }

            if (startPoint.X >= sinkPoint.X)
            {
                linePoints.Clear();
                Point p1 = new Point();
                p1.X = startPoint.X + distance;
                p1.Y = startPoint.Y;
                Point p4 = new Point();
                p4.X = sinkPoint.X - distance;
                p4.Y = sinkPoint.Y;
                Point p2 = new Point();
                p2.X = p1.X;
                p2.Y = sinkPoint.Y - (sinkPoint.Y - startPoint.Y) / 2;
                Point p3 = new Point();
                p3.X = sinkPoint.X - distance;
                p3.Y = p2.Y;
                if ((sink.DesignerItemTop + sink.DesignerItemSize.Height) < source.DesignerItemTop)
                {
                    p2.Y = source.DesignerItemTop - (source.DesignerItemTop - (sink.DesignerItemTop + sink.DesignerItemSize.Height)) / 2;
                    p3.Y = p2.Y;
                }
                if (sink.DesignerItemTop > (source.DesignerItemTop + source.DesignerItemSize.Height))
                {
                    p2.Y = sink.DesignerItemTop - (sink.DesignerItemTop - (source.DesignerItemTop + source.DesignerItemSize.Height)) / 2;
                    p3.Y = p2.Y;
                }
                linePoints.Add(startPoint);
                linePoints.Add(p1);
                linePoints.Add(p2);
                linePoints.Add(p3);
                linePoints.Add(p4);
                linePoints.Add(sinkPoint);
            }
            return linePoints;
        }        
        internal static List<Point> GetConnectionLine1(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation)
        {
            List<Point> linePoints = new List<Point>();
            Rect rectSource = GetRectWithMargin(source, 10);
            Point startPoint = GetOffsetPoint(source, rectSource);
            Point endPoint = sinkPoint;

            linePoints.Add(startPoint);
            Point currentPoint = startPoint;

            if (startPoint.X < sinkPoint.X)
            {
                linePoints.Clear();
                Point p1 = new Point();
                p1.X = startPoint.X + (sinkPoint.X - startPoint.X) / 2;
                p1.Y = startPoint.Y;
                Point p2 = new Point();
                p2.X = sinkPoint.X - (sinkPoint.X - startPoint.X) / 2;
                p2.Y = sinkPoint.Y;
                linePoints.Add(startPoint);
                linePoints.Add(p1);
                linePoints.Add(p2);
                linePoints.Add(sinkPoint);
            }

            if (startPoint.X >= sinkPoint.X)
            {
                linePoints.Clear();
                Point p1 = new Point();
                p1.X = startPoint.X + distance;
                p1.Y = startPoint.Y;
                Point p4 = new Point();
                p4.X = sinkPoint.X - distance;
                p4.Y = sinkPoint.Y;
                Point p2 = new Point();
                p2.X = p1.X;
                p2.Y = sinkPoint.Y - (sinkPoint.Y - startPoint.Y) / 2;
                Point p3 = new Point();
                p3.X = sinkPoint.X - distance;
                p3.Y = p2.Y;
                if (sinkPoint.Y > (source.DesignerItemTop + source.DesignerItemSize.Height))
                {
                    p2.Y = (sinkPoint.Y - (source.DesignerItemTop + source.DesignerItemSize.Height)) / 2 + (source.DesignerItemTop + source.DesignerItemSize.Height);
                    p3.Y = sinkPoint.Y - (sinkPoint.Y - (source.DesignerItemTop + source.DesignerItemSize.Height)) / 2;
                }
                if (sinkPoint.Y < source.DesignerItemTop)
                {
                    p2.Y = source.DesignerItemTop - (source.DesignerItemTop - sinkPoint.Y) / 2;
                    p3.Y = p2.Y;
                }
                linePoints.Add(startPoint);
                linePoints.Add(p1);
                linePoints.Add(p2);
                linePoints.Add(p3);
                linePoints.Add(p4);
                linePoints.Add(sinkPoint);
            }
            return linePoints;
        }
        private static List<Point> OptimizeLinePoints(List<Point> linePoints, Rect[] rectangles, ConnectorOrientation sourceOrientation, ConnectorOrientation sinkOrientation)
        {
            List<Point> points = new List<Point>();
            int cut = 0;

            for (int i = 0; i < linePoints.Count; i++)
            {
                if (i >= cut)
                {
                    for (int k = linePoints.Count - 1; k > i; k--)
                    {
                        if (IsPointVisible(linePoints[i], linePoints[k], rectangles))
                        {
                            cut = k;
                            break;
                        }
                    }
                    points.Add(linePoints[i]);
                }
            }

            #region Line
            for (int j = 0; j < points.Count - 1; j++)
            {
                if (points[j].X != points[j + 1].X && points[j].Y != points[j + 1].Y)
                {
                    ConnectorOrientation orientationFrom;
                    ConnectorOrientation orientationTo;

                    // orientation from point
                    if (j == 0)
                        orientationFrom = sourceOrientation;
                    else
                        orientationFrom = GetOrientation(points[j], points[j - 1]);

                    // orientation to pint 
                    if (j == points.Count - 2)
                        orientationTo = sinkOrientation;
                    else
                        orientationTo = GetOrientation(points[j + 1], points[j + 2]);


                    if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                        (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                    {
                        double centerX = Math.Min(points[j].X, points[j + 1].X) + Math.Abs(points[j].X - points[j + 1].X) / 2;
                        points.Insert(j + 1, new Point(centerX, points[j].Y));
                        points.Insert(j + 2, new Point(centerX, points[j + 2].Y));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return points;
                    }

                    if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                        (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                    {
                        double centerY = Math.Min(points[j].Y, points[j + 1].Y) + Math.Abs(points[j].Y - points[j + 1].Y) / 2;
                        points.Insert(j + 1, new Point(points[j].X, centerY));
                        points.Insert(j + 2, new Point(points[j + 2].X, centerY));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return points;
                    }

                    if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                        (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                    {
                        points.Insert(j + 1, new Point(points[j + 1].X, points[j].Y));
                        return points;
                    }

                    if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                        (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                    {
                        points.Insert(j + 1, new Point(points[j].X, points[j + 1].Y));
                        return points;
                    }
                }
            }
            #endregion

            return points;
        }

        private static ConnectorOrientation GetOrientation(Point p1, Point p2)
        {
            if (p1.X == p2.X)
            {
                if (p1.Y >= p2.Y)
                    return ConnectorOrientation.Bottom;
                else
                    return ConnectorOrientation.Top;
            }
            else if (p1.Y == p2.Y)
            {
                if (p1.X >= p2.X)
                    return ConnectorOrientation.Right;
                else
                    return ConnectorOrientation.Left;
            }
            throw new Exception("Failed to retrieve orientation");
        }

        private static Orientation GetOrientation(ConnectorOrientation sourceOrientation)
        {
            switch (sourceOrientation)
            {
                case ConnectorOrientation.Left:
                    return Orientation.Horizontal;
                case ConnectorOrientation.Top:
                    return Orientation.Vertical;
                case ConnectorOrientation.Right:
                    return Orientation.Horizontal;
                case ConnectorOrientation.Bottom:
                    return Orientation.Vertical;
                default:
                    throw new Exception("Unknown ConnectorOrientation");
            }
        }

        private static Point GetNearestNeighborSource(ConnectorInfo source, Point endPoint, Rect rectSource, Rect rectSink, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(source.Orientation, rectSource, out n1, out n2);

            if (rectSink.Contains(n1))
            {
                flag = false;
                return n2;
            }

            if (rectSink.Contains(n2))
            {
                flag = true;
                return n1;
            }

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestNeighborSource(ConnectorInfo source, Point endPoint, Rect rectSource, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(source.Orientation, rectSource, out n1, out n2);

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestVisibleNeighborSink(Point currentPoint, Point endPoint, ConnectorInfo sink, Rect rectSource, Rect rectSink)
        {
            Point s1, s2; // neighbors on sink side
            GetNeighborCorners(sink.Orientation, rectSink, out s1, out s2);

            bool flag1 = IsPointVisible(currentPoint, s1, new Rect[] { rectSource, rectSink });
            bool flag2 = IsPointVisible(currentPoint, s2, new Rect[] { rectSource, rectSink });

            if (flag1) // s1 visible
            {
                if (flag2) // s1 and s2 visible
                {
                    if (rectSink.Contains(s1))
                        return s2;

                    if (rectSink.Contains(s2))
                        return s1;

                    if ((Distance(s1, endPoint) <= Distance(s2, endPoint)))
                        return s1;
                    else
                        return s2;

                }
                else
                {
                    return s1;
                }
            }
            else // s1 not visible
            {
                if (flag2) // only s2 visible
                {
                    return s2;
                }
                else // s1 and s2 not visible
                {
                    return new Point(double.NaN, double.NaN);
                }
            }
        }

        private static bool IsPointVisible(Point fromPoint, Point targetPoint, Rect[] rectangles)
        {
            foreach (Rect rect in rectangles)
            {
                if (RectangleIntersectsLine(rect, fromPoint, targetPoint))
                    return false;
            }
            return true;
        }

        private static bool IsRectVisible(Point fromPoint, Rect targetRect, Rect[] rectangles)
        {
            if (IsPointVisible(fromPoint, targetRect.TopLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.TopRight, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomRight, rectangles))
                return true;

            return false;
        }

        private static bool RectangleIntersectsLine(Rect rect, Point startPoint, Point endPoint)
        {
            rect.Inflate(-1, -1);
            return rect.IntersectsWith(new Rect(startPoint, endPoint));
        }

        private static void GetOppositeCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    n1 = rect.TopRight; n2 = rect.BottomRight;
                    break;
                case ConnectorOrientation.Top:
                    n1 = rect.BottomLeft; n2 = rect.BottomRight;
                    break;
                case ConnectorOrientation.Right:
                    n1 = rect.TopLeft; n2 = rect.BottomLeft;
                    break;
                case ConnectorOrientation.Bottom:
                    n1 = rect.TopLeft; n2 = rect.TopRight;
                    break;
                default:
                    throw new Exception("No opposite corners found!");
            }
        }

        private static void GetNeighborCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    n1 = rect.TopLeft; n2 = rect.BottomLeft;
                    break;
                case ConnectorOrientation.Top:
                    n1 = rect.TopLeft; n2 = rect.TopRight;
                    break;
                case ConnectorOrientation.Right:
                    n1 = rect.TopRight; n2 = rect.BottomRight;
                    break;
                case ConnectorOrientation.Bottom:
                    n1 = rect.BottomLeft; n2 = rect.BottomRight;
                    break;
                default:
                    throw new Exception("No neighour corners found!");
            }
        }

        private static double Distance(Point p1, Point p2)
        {
            return Point.Subtract(p1, p2).Length;
        }

        private static Rect GetRectWithMargin(ConnectorInfo connectorThumb, double margin)
        {
            Rect rect = new Rect(connectorThumb.DesignerItemLeft,
                                 connectorThumb.DesignerItemTop,
                                 connectorThumb.DesignerItemSize.Width,
                                 connectorThumb.DesignerItemSize.Height);

            rect.Inflate(margin, margin);

            return rect;
        }

        private static Point GetOffsetPoint(ConnectorInfo connector, Rect rect)
        {
            Point offsetPoint = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Left:
                    offsetPoint = new Point(connector.Position.X - leftmargin, connector.Position.Y);
                    break;
                case ConnectorOrientation.Top:
                    offsetPoint = new Point(connector.Position.X, rect.Top);
                    break;
                case ConnectorOrientation.Right:
                    offsetPoint = new Point(connector.Position.X + rightmargin, connector.Position.Y);
                    break;
                case ConnectorOrientation.Bottom:
                    offsetPoint = new Point(connector.Position.X, rect.Bottom);
                    break;
                default:
                    break;
            }

            return offsetPoint;
        }

        private static void CheckPathEnd(ConnectorInfo source, ConnectorInfo sink, bool showLastLine, List<Point> linePoints)
        {
            if (showLastLine)
            {
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);
                double marginPath = 15;
                switch (source.Orientation)
                {
                    case ConnectorOrientation.Left:
                        startPoint = new Point(source.Position.X - marginPath, source.Position.Y);
                        break;
                    case ConnectorOrientation.Top:
                        startPoint = new Point(source.Position.X, source.Position.Y - marginPath);
                        break;
                    case ConnectorOrientation.Right:
                        startPoint = new Point(source.Position.X + marginPath, source.Position.Y);
                        break;
                    case ConnectorOrientation.Bottom:
                        startPoint = new Point(source.Position.X, source.Position.Y + marginPath);
                        break;
                    default:
                        break;
                }

                switch (sink.Orientation)
                {
                    case ConnectorOrientation.Left:
                        endPoint = new Point(sink.Position.X - marginPath, sink.Position.Y);
                        break;
                    case ConnectorOrientation.Top:
                        endPoint = new Point(sink.Position.X, sink.Position.Y - marginPath);
                        break;
                    case ConnectorOrientation.Right:
                        endPoint = new Point(sink.Position.X + marginPath, sink.Position.Y);
                        break;
                    case ConnectorOrientation.Bottom:
                        endPoint = new Point(sink.Position.X, sink.Position.Y + marginPath);
                        break;
                    default:
                        break;
                }
                linePoints.Insert(0, startPoint);
                linePoints.Add(endPoint);
            }
            else
            {
                linePoints.Insert(0, source.Position);
                linePoints.Add(sink.Position);
            }
        }

        private static ConnectorOrientation GetOpositeOrientation(ConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Left:
                    return ConnectorOrientation.Right;
                case ConnectorOrientation.Top:
                    return ConnectorOrientation.Bottom;
                case ConnectorOrientation.Right:
                    return ConnectorOrientation.Left;
                case ConnectorOrientation.Bottom:
                    return ConnectorOrientation.Top;
                default:
                    return ConnectorOrientation.Top;
            }
        }
    }
}
