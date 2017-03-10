using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace DiagramDesigner
{
    public class ConnectorAdorner : Adorner
    {
        private PathGeometry pathGeometry;
        private DesignerCanvas designerCanvas;
        private Connector sourceConnector;
        private Pen drawingPen;

        private DesignerItem hitDesignerItem;
        private DesignerItem HitDesignerItem
        {
            get { return hitDesignerItem; }
            set
            {
                if (hitDesignerItem != value)
                {
                    if (hitDesignerItem != null)
                        hitDesignerItem.IsDragConnectionOver = false;

                    hitDesignerItem = value;

                    if (hitDesignerItem != null)
                        hitDesignerItem.IsDragConnectionOver = true;
                }
            }
        }

        private Connector hitConnector;
        private Connector HitConnector
        {
            get { return hitConnector; }
            set
            {
                if (hitConnector != value)
                {
                    hitConnector = value;
                }
            }
        }

        public ConnectorAdorner(DesignerCanvas designer, Connector sourceConnector) : base(designer)
        {
            this.designerCanvas = designer;
            this.sourceConnector = sourceConnector;
            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            drawingPen.LineJoin = PenLineJoin.Round;
            this.Cursor = Cursors.Cross;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (HitConnector != null && !HitConnector.ConnectorHasConnected && !this.sourceConnector.ConnectorHasConnected)
            {
                Connector sourceConnector = this.sourceConnector;
                Connector sinkConnector = this.HitConnector;
                Connection newConnection = new Connection(sourceConnector, sinkConnector);

                DesignerItem sourceDesignerItem = sourceConnector.ParentDesignerItem;
                DesignerItem sinkDesignerItem = sinkConnector.ParentDesignerItem;

                sourceDesignerItem.leftConnectionList.Add(newConnection);
                sinkDesignerItem.rightConnectionList.Add(newConnection);

                if (sinkDesignerItem.ItemType.Equals("ConditionAndExp") && sourceDesignerItem.ItemType.Equals("Condition"))
                {
                    this.designerCanvas.AndConnectionList.Add(newConnection);
                    //ComboBox tempBox = sinkConnector.Content as ComboBox;
                    //this.designerCanvas.andOrder[sourceConnector.ParentDesignerItem.ItemOrder] = ((ComboBoxItem)tempBox.SelectedItem).Content.ToString();
                }

                if (sinkDesignerItem.ItemType.Equals("ConditionOrExp") && sourceDesignerItem.ItemType.Equals("Condition"))
                {
                    this.designerCanvas.OrConnectionList.Add(newConnection);
                    //this.designerCanvas.orOrder.Add(sourceConnector.ParentDesignerItem.ItemOrder);
                }                
                //DesignerItem parentItem = sourceConnector.ParentDesignerItem;
                //DesignerItem childItem = sinkConnector.ParentDesignerItem;
                //parentItem.childNodeItem.Add(childItem);

                sinkConnector.ConnectorHasConnected = sinkConnector.DataType != ConnectorDataType.MultiLinker ? true : false;
                sourceConnector.ConnectorHasConnected = sourceConnector.DataType != ConnectorDataType.MultiLinker ? true : false;
                Canvas.SetZIndex(newConnection, designerCanvas.Children.Count);
                this.designerCanvas.Children.Add(newConnection);
                //this.designerCanvas.connectionList.Add(newConnection);
            }
            if (HitDesignerItem != null)
            {
                this.HitDesignerItem.IsDragConnectionOver = false;
            }

            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured) this.CaptureMouse();
                HitTesting(e.GetPosition(this));
                this.pathGeometry = GetPathGeometry(e.GetPosition(this));
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, drawingPen, this.pathGeometry);

            // without a background the OnMouseMove event would not be fired
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        private PathGeometry GetPathGeometry(Point position)
        {
            PathGeometry geometry = new PathGeometry();

            ConnectorOrientation targetOrientation;
            if (HitConnector != null)
                targetOrientation = HitConnector.Orientation;
            else
                targetOrientation = ConnectorOrientation.None;

            List<Point> pathPoints = PathFinder.GetConnectionLine1(sourceConnector.GetInfo(), position, targetOrientation);

            if (pathPoints.Count > 0)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = pathPoints[0];
                pathPoints.Remove(pathPoints[0]);
                figure.Segments.Add(new PolyLineSegment(pathPoints, true));
                geometry.Figures.Add(figure);
            }

            return geometry;
        }

        private void HitTesting(Point hitPoint)
        {
            bool hitConnectorFlag = false;

            DependencyObject hitObject = designerCanvas.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != sourceConnector.ParentDesignerItem &&
                   hitObject.GetType() != typeof(DesignerCanvas))
            {
                if (hitObject is Connector && hitObject != sourceConnector)
                {
                    HitConnector = hitObject as Connector;
                    hitConnectorFlag = true;
                    //Connector Con = hitObject as Connector;
                    //if ((Con.DataType & sourceConnector.DataType) == 0)
                    //    HitConnector = null;
                    //else
                    //{
                    //    HitConnector = hitObject as Connector;
                    //    hitConnectorFlag = true;
                    //}        
                }

                if (hitObject is DesignerItem)
                {
                    HitDesignerItem = hitObject as DesignerItem;
                    if (!hitConnectorFlag)
                        HitConnector = null;
                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
            HitDesignerItem = null;
        }
    }
}
