using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Windows.Data;
using System.Diagnostics;

namespace DiagramDesigner
{
    public partial class DesignerCanvas : Canvas
    {
        private Point? rubberbandSelectionStartPoint = null;
        private string name = null;
        private string nodeFileName = null;
        private AddNode node;

        public List<DesignerItem> triggerList = new List<DesignerItem>();
        public List<DesignerItem> conditionList = new List<DesignerItem>();
        public List<DesignerItem> actionList = new List<DesignerItem>();
        public List<DesignerItem> targetList = new List<DesignerItem>();
        public List<DesignerItem> expList = new List<DesignerItem>();
        public List<Connection> AndConnectionList = new List<Connection>();
        public List<Connection> OrConnectionList = new List<Connection>();

        public Dictionary<int, string> andOrder = new Dictionary<int, string>();
        public Dictionary<int, string> orOrder = new Dictionary<int, string>();
        public Dictionary<int, int> conditionLinker = new Dictionary<int, int>();
        public Dictionary<int, int> actionLinker = new Dictionary<int, int>();
        public DesignerItem AndDesignerItem = new DesignerItem();
        public DesignerItem OrDesignerItem = new DesignerItem();

        public StackPanel targetPanel = new StackPanel();
        public StackPanel AIBar;
        public int initAIID;
        public List<ComboData> ListData = new List<ComboData>();        
        public DesignerItem aiParam = new DesignerItem();
        public DesignerItem monster = new DesignerItem();
        //private List<AddNode> allNode = new List<AddNode>();

        private void GetNodeName(string Name, string nodeFileName)
        {
            name = Name;
            this.nodeFileName = nodeFileName;
        }

        private SelectionService selectionService;
        internal SelectionService SelectionService
        {
            get
            {
                if (selectionService == null)
                    selectionService = new SelectionService(this);
                return selectionService;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                // in case that this click is the start of a 
                // drag operation we cache the start point
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                SelectionService.ClearSelection();
                Focus();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (!String.IsNullOrEmpty(name))
            {
                node.comboData = ListData;
                DesignerItem newItem = node.addNode(name, nodeFileName);
                if (newItem.ItemType == "Target")
                {
                    bool hasExist = false;
                    for (int i = 0; i < targetPanel.Children.Count; ++i)
                    {
                        DesignerItem temp = targetPanel.Children[i] as DesignerItem;
                        if (newItem.IDName.Equals(temp.IDName))
                        {
                            hasExist = true;
                            break;
                        }
                    }
                    if(!hasExist)
                    {
                        this.targetPanel.Children.Add(newItem);
                        int id = ListData.Count - 1;
                        string value = id.ToString();
                        ListData.Add(new ComboData { Id = id, Value = value });
                        AddTriggerItem(newItem.ItemType, newItem);
                    }
                }
                else
                {
                    Point position = e.GetPosition(this);
                    DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                    DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    Canvas.SetZIndex(newItem, this.Children.Count);
                    this.Children.Add(newItem);
                    SetConnectorDecoratorTemplate(newItem);
                    //update selection
                    this.SelectionService.SelectItem(newItem);
                    AddTriggerItem(newItem.ItemType, newItem);
                    newItem.Focus();
                }
                e.Handled = true;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.ApplyTemplate() && item.Content is UIElement)
            {
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
                Control decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if (decorator != null && template != null)
                    decorator.Template = template;
            }
        }

        public void AddTriggerItem(string type, DesignerItem item)
        {
            switch(type)
            {
                case "Trigger":
                    triggerList.Add(item);
                    break;
                case "Condition":
                    conditionList.Add(item);
                    break;
                case "Action":
                    actionList.Add(item);
                    break;
                case "Target":
                    targetList.Add(item);
                    break;
                case "ConditionAndExp":
                    AndDesignerItem = item;
                    break;
                case "ConditionOrExp":
                    OrDesignerItem = item;
                    break;
            }
        }

        public void DeleteFromList(DesignerItem item)
        {
            int index;
            int index_2;
            switch(item.ItemType)
            {
                case "Trigger":
                    index = triggerList.IndexOf(item);
                    triggerList.RemoveAt(index);
                    break;
                case "Condition":
                    index = conditionList.IndexOf(item);
                    conditionList.RemoveAt(index);
                    conditionLinker.Remove(item.ItemOrder);
                    andOrder.Remove(item.ItemOrder);
                    orOrder.Remove(item.ItemOrder);
                    for (int i = 0; i < item.rightConnectionList.Count; ++i)
                    {
                        index_2 = AndConnectionList.IndexOf(item.rightConnectionList[i]);
                        if(index_2 != -1)
                        {
                            AndConnectionList.RemoveAt(index_2);
                        }
                        index_2 = OrConnectionList.IndexOf(item.rightConnectionList[i]);
                        if (index_2 != -1)
                        {
                            OrConnectionList.RemoveAt(index_2);
                        }
                    }
                    break;
                case "ConditionAndExp":
                    AndConnectionList.Clear();
                    andOrder.Clear();
                    break;
                case "ConditionOrExp":
                    OrConnectionList.Clear();
                    orOrder.Clear();
                    break;
                case "Action":
                    index = actionList.IndexOf(item);
                    actionList.RemoveAt(index);
                    actionLinker.Remove(item.ItemOrder);
                    break;
            }
        }
    }
}
