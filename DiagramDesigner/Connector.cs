using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class Connector : ContentControl, INotifyPropertyChanged
    {
        // drag start point, relative to the DesignerCanvas
        private Point? dragStartPoint = null;

        public ConnectorOrientation Orientation { get; set; }
        private bool connectorhasconnected = false;
        public bool isStart { get; set; }
        public Guid nodeProperty { get; set; }

        public bool ConnectorHasConnected
        {
            get { return connectorhasconnected; }
            set { connectorhasconnected = value; }
        }
        public ConnectorDataType datatype;
        public ConnectorDataType DataType 
        {
            get
            {
                return datatype;
            }
            set
            {
                datatype = value;
            }
        }
        // center position of this Connector relative to the DesignerCanvas
        private Point position;
        public Point Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        // the DesignerItem this Connector belongs to;
        // retrieved from DataContext, which is set in the
        // DesignerItem template
        private DesignerItem parentDesignerItem;
        public DesignerItem ParentDesignerItem
        {
            get
            {
                if (parentDesignerItem == null)
                {
                    ListView parent1 = this.Parent as ListView;
                    Grid parent2 = parent1.Parent as Grid;
                    GroupBox parent3 = parent2.Parent as GroupBox;
                    Border parent4 = parent3.Parent as Border;
                    parentDesignerItem = parent4.Parent as DesignerItem;
                }
                //parentDesignerItem = this.DataContext as DesignerItem;
                return parentDesignerItem;
            }
        }

        // keep track of connections that link to this connector
        private List<Connection> connections;
        public List<Connection> Connections
        {
            get
            {
                if (connections == null)
                    connections = new List<Connection>();
                return connections;
            }
        }

        public Connector()
        {
            // fired when layout changes
            base.LayoutUpdated += new EventHandler(Connector_LayoutUpdated);            
        }

        // when the layout changes we update the position property
        void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            DesignerCanvas designer = GetDesignerCanvas(this);
            if (designer != null)
            {
                //get centre position of this Connector relative to the DesignerCanvas
                this.Position = this.TransformToAncestor(designer).Transform(new Point(this.Width / 2, this.Height / 2));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DesignerCanvas canvas = GetDesignerCanvas(this);
            if (canvas != null)
            {
                // position relative to DesignerCanvas
                this.dragStartPoint = new Point?(e.GetPosition(canvas));
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            // but if mouse button is pressed and start point value is set we do have one
            if (this.dragStartPoint.HasValue)
            {
                // create connection adorner 
                DesignerCanvas canvas = GetDesignerCanvas(this);
                if (canvas != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                    if (adornerLayer != null)
                    {
                        ConnectorAdorner adorner = new ConnectorAdorner(canvas, this);
                        if (adorner != null)
                        {
                            adornerLayer.Add(adorner);
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        internal ConnectorInfo GetInfo()
        {
            ConnectorInfo info = new ConnectorInfo();
            info.DesignerItemLeft = DesignerCanvas.GetLeft(this.ParentDesignerItem);
            info.DesignerItemTop = DesignerCanvas.GetTop(this.ParentDesignerItem);
            info.DesignerItemSize = new Size(this.ParentDesignerItem.ActualWidth, this.ParentDesignerItem.ActualHeight);
            info.Orientation = this.Orientation;
            info.ConnectorSize = new Size(this.ActualWidth, this.ActualHeight);
            info.Position = this.Position;
            info.DataType = this.DataType;
            return info;
        }

        // iterate through visual tree to get parent DesignerCanvas
        private DesignerCanvas GetDesignerCanvas(DependencyObject element)
        {
            while (element != null && !(element is DesignerCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as DesignerCanvas;
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    // provides compact info about a connector; used for the 
    // routing algorithm, instead of hand over a full fledged Connector
    internal struct ConnectorInfo
    {
        public double DesignerItemLeft { get; set; }
        public double DesignerItemTop { get; set; }
        public Size DesignerItemSize { get; set; }
        public Size ConnectorSize { get; set; }
        public Point Position { get; set; }
        public ConnectorOrientation Orientation { get; set; }
        public ConnectorDataType DataType { get; set; }
    }

    public enum ConnectorOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }

    [Flags]
    public enum ConnectorDataType
    {
        None,
        SingleLinker = 0x20,
        MultiLinker = 0x40,
    }
}
