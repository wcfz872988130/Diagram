using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace DiagramDesigner
{
    public partial class DesignerCanvas
    {
        public static RoutedCommand Group = new RoutedCommand();
        public static RoutedCommand Ungroup = new RoutedCommand();
        public static RoutedCommand BringForward = new RoutedCommand();
        public static RoutedCommand BringToFront = new RoutedCommand();
        public static RoutedCommand SendBackward = new RoutedCommand();
        public static RoutedCommand SendToBack = new RoutedCommand();
        public static RoutedCommand AlignTop = new RoutedCommand();
        public static RoutedCommand AlignVerticalCenters = new RoutedCommand();
        public static RoutedCommand AlignBottom = new RoutedCommand();
        public static RoutedCommand AlignLeft = new RoutedCommand();
        public static RoutedCommand AlignHorizontalCenters = new RoutedCommand();
        public static RoutedCommand AlignRight = new RoutedCommand();
        public static RoutedCommand DistributeHorizontal = new RoutedCommand();
        public static RoutedCommand DistributeVertical = new RoutedCommand();
        public static RoutedCommand SelectAll = new RoutedCommand();
        public static RoutedCommand ParsePython = new RoutedCommand();

        public DesignerCanvas()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));
            //this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Group, Group_Executed, Group_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Ungroup, Ungroup_Executed, Ungroup_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringForward, BringForward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringToFront, BringToFront_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendBackward, SendBackward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendToBack, SendToBack_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignTop, AlignTop_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignVerticalCenters, AlignVerticalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignBottom, AlignBottom_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignLeft, AlignLeft_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignHorizontalCenters, AlignHorizontalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignRight, AlignRight_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeHorizontal, DistributeHorizontal_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeVertical, DistributeVertical_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));

            node = new AddNode();
            Init_Executed();

            //this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ParsePython, ParsePython_Executed));
            ToolboxItem.transname += GetNodeName;
            this.AllowDrop = true;            
            Clipboard.Clear();
        }

        private void Init_Executed()
        {
            monster = node.addNode("Monster", "Trigger");
            DesignerCanvas.SetLeft(monster, 10);
            DesignerCanvas.SetTop(monster, 80);
            this.Children.Add(monster);
            triggerList.Add(monster);

            //node = new AddNode();
            aiParam = node.addNode("AIParam", "Trigger");
            DesignerCanvas.SetLeft(aiParam, 10);
            DesignerCanvas.SetTop(aiParam, 260);
            this.Children.Add(aiParam);
            //SetConnectorDecoratorTemplate(aiParam);
            triggerList.Add(aiParam);

            ListData.Add(new ComboData { Id = -1, Value = "" });
        }

        #region PythonParse Command

        private void ParsePython_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //PythonParse pps = new PythonParse();
            //pps.ReDrawDiagram("1015");
        }
        #endregion

        #region New Command

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewCanvas();
        }

        private void DeleteConnection(DesignerItem item)
        {
            if (item.leftConnectionList != null || item.leftConnectionList.Count > 0)
            {
                for (int i = 0; i < item.leftConnectionList.Count; ++i)
                {
                    DeleteConnectorLinker(item.leftConnectionList[i]);
                    this.Children.Remove(item.leftConnectionList[i]);
                }

            }
            if (item.rightConnectionList != null || item.rightConnectionList.Count > 0)
            {
                for (int i = 0; i < item.rightConnectionList.Count; ++i)
                {
                    DeleteConnectorLinker(item.rightConnectionList[i]);
                    this.Children.Remove(item.rightConnectionList[i]);
                }
            }
        }

        private void DeleteConnectorLinker(Connection con)
        {
            Connector sourceConnector = con.Source;
            Connector sinkConnector = con.Sink;
            if(sourceConnector != null)
            { sourceConnector.ConnectorHasConnected = false; }
            if(sinkConnector != null)
            { sinkConnector.ConnectorHasConnected = false; }            
        }

        #endregion

        #region Open Command

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromFile();

            if (root == null)
                return;

            this.Children.Clear();
            this.SelectionService.ClearSelection();

            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
                DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
            }

            this.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                this.Children.Add(connection);
            }
        }

        #endregion

        #region Save Command

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            conditionList.Sort(SortDesignerItemList);
            actionList.Sort(SortDesignerItemList);
            
            for (int i = 0; i < AndConnectionList.Count; ++i)
            {
                Connector sinkConnector = AndConnectionList[i].Sink;
                Connector sourceConnector = AndConnectionList[i].Source;
                ComboBox tempBox = sinkConnector.Content as ComboBox;
                //string temp = tempBox.SelectedItem as string;
                //int length = temp.Length;
                this.andOrder[sourceConnector.ParentDesignerItem.ItemOrder] = tempBox.SelectedItem as string;
            }

            for (int i = 0; i < OrConnectionList.Count; ++i)
            {
                Connector sinkConnector = OrConnectionList[i].Sink;
                Connector sourceConnector = OrConnectionList[i].Source;
                ComboBox tempBox = sinkConnector.Content as ComboBox;
                this.orOrder[sourceConnector.ParentDesignerItem.ItemOrder] = tempBox.SelectedItem as string;
            }
            //andOrder.Sort(SortList);
            //orOrder.Sort(SortList);
            PythonParse.codePath = AppDomain.CurrentDomain.BaseDirectory + "Code";
            try
            {
                initAIID = int.Parse(aiParam.outputValue[0].propertyValue);
                PythonParse.PythonStructure(initAIID, triggerList, conditionList, actionList, targetList, andOrder, orOrder);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败:" + ex.Message);
                andOrder.Clear();
                orOrder.Clear();
            }
            
        }

        private int SortDesignerItemList(DesignerItem itemA, DesignerItem itemB)
        {
            if(itemA.ItemOrder > itemB.ItemOrder)
            {
                return 1;
            }
            else if(itemA.ItemOrder < itemB.ItemOrder)
            {
                return -1;
            }
            return 0;
        }
        private int SortList(int itemA, int itemB)
        {
            if (itemA > itemB)
            {
                return 1;
            }
            else if (itemA < itemB)
            {
                return -1;
            }
            return 0;
        }
        #endregion

        #region Print Command

        public void Print_Executed(string id)
        {
            List<Dictionary<string, List<string>>> paramType = XmlParse.parseTypeXml();
            DrawComponent.paramDictionary = paramType[0];
            DrawComponent.paramDescribe = paramType[1];
            PythonParse pps = new PythonParse();
            pps.ReDrawDiagram(id);
            DrawComponent.ReWriteDesignerItemValue(aiParam, pps.AIInitParams);
            DesignerItem triggerItem = pps.DrawTriggerOnCanvas();
            DesignerCanvas.SetLeft(triggerItem, 310);
            DesignerCanvas.SetTop(triggerItem, 10);
            this.Children.Add(triggerItem);
            this.triggerList.Add(triggerItem);
            SetConnectorDecoratorTemplate(triggerItem);
            conditionLinker = pps.conditionTargetLinker;
            actionLinker = pps.actionTargetLinker;
            List<DesignerItem> targetItemList = pps.DrawTargetTriggersOnCanvas();
            ListData = DrawComponent.ListData;
            for (int i = 0; i < targetItemList.Count; ++i)
            {
                targetPanel.Children.Add(targetItemList[i]);
                this.targetList.Add(targetItemList[i]);
            }

            List<DesignerItem> conditionItemList = pps.DrawConditionTriggersOnCanvas();
            for (int i = 0; i < conditionItemList.Count; ++i)
            {
                DesignerCanvas.SetLeft(conditionItemList[i], 610);
                DesignerCanvas.SetTop(conditionItemList[i], 10 + 260 * i);
                this.conditionList.Add(conditionItemList[i]);
                this.Children.Add(conditionItemList[i]);
                SetConnectorDecoratorTemplate(conditionItemList[i]);
            }

            List<DesignerItem> actionItemList = pps.DrawActionTriggersOnCanvas();
            for (int i = 0; i < actionItemList.Count; ++i)
            {
                DesignerCanvas.SetLeft(actionItemList[i], 1550);
                DesignerCanvas.SetTop(actionItemList[i], 200 + 260 * i);
                this.actionList.Add(actionItemList[i]);
                this.Children.Add(actionItemList[i]);
                SetConnectorDecoratorTemplate(actionItemList[i]);
            }
            //Dictionary<int, string> targetDictionary = pps.targetDictionary;
            //Dictionary<int, List<string>> targetParamDictionary = pps.targetParamDictionary;
            //Dictionary<List<int>, int> linkerRelation = pps.conditionTargetLinker;
            //foreach(KeyValuePair<List<int>, int> kvp in linkerRelation)
            //{
            //    List<int> tempKvp = kvp.Key;
            //    int index = tempKvp[0];
            //    int target = tempKvp[1];
            //    int targetIndex = kvp.Value;
            //    string protoName = null;
            //    List<string> param = new List<string>();
            //    if(targetDictionary.ContainsKey(targetIndex))
            //    {
            //        protoName = targetDictionary[targetIndex];
            //    }
            //    if (targetParamDictionary.ContainsKey(targetIndex))
            //    {
            //        param = targetParamDictionary[targetIndex];
            //    }
            //    DesignerItem targetDesignerItem = DrawComponent.addNode("Target", protoName, param);
            //    DesignerCanvas.SetLeft(targetDesignerItem, 880);
            //    DesignerCanvas.SetTop(targetDesignerItem, 260 * index + 100 * target);
            //    this.Children.Add(targetDesignerItem);
            //    SetConnectorDecoratorTemplate(targetDesignerItem);

            //    DesignerItem sourceDesignerItem = findDesignerItem(index + 1, conditionList);
            //    Connection targetLinker = new Connection(sourceDesignerItem.RightConnector[0], targetDesignerItem.LeftConnector);
            //    this.Children.Add(targetLinker);
            //}

            DesignerItem ExpConditionResult = pps.DrawExpResultOnCanvas();
            DesignerCanvas.SetLeft(ExpConditionResult, 1250);
            DesignerCanvas.SetTop(ExpConditionResult, 400);
            this.Children.Add(ExpConditionResult);
            expList.Add(ExpConditionResult);
            SetConnectorDecoratorTemplate(ExpConditionResult);

            DesignerItem ExpConditionAnd = pps.DrawAndorOrExpOnCanvas("AndExp", "ConditionAndExp", "and");
            
            if (ExpConditionAnd != null)
            {
                DesignerCanvas.SetLeft(ExpConditionAnd, 950);
                DesignerCanvas.SetTop(ExpConditionAnd, 200);
                this.AndDesignerItem = ExpConditionAnd;
                this.Children.Add(ExpConditionAnd);
                SetConnectorDecoratorTemplate(ExpConditionAnd);

                Connection newAndConnection = new Connection(ExpConditionAnd.RightConnector[0], ExpConditionResult.LeftConnector);
                addDesignerItemConnection(ExpConditionAnd.RightConnector[0], ExpConditionResult.LeftConnector, newAndConnection);
                this.Children.Add(newAndConnection);
                expList.Add(ExpConditionAnd);
            }

            DesignerItem ExpConditionOr = pps.DrawAndorOrExpOnCanvas("OrExp", "ConditionOrExp", "or");
            if (ExpConditionOr != null)
            {
                DesignerCanvas.SetLeft(ExpConditionOr, 950);
                DesignerCanvas.SetTop(ExpConditionOr, 570);
                this.OrDesignerItem = ExpConditionOr;
                this.Children.Add(ExpConditionOr);
                SetConnectorDecoratorTemplate(ExpConditionOr);

                Connection newOrConnection = new Connection(ExpConditionOr.RightConnector[0], ExpConditionResult.LeftConnector);
                addDesignerItemConnection(ExpConditionOr.RightConnector[0], ExpConditionResult.LeftConnector, newOrConnection);
                this.Children.Add(newOrConnection);
                expList.Add(ExpConditionOr);
            }

            //triggerItem.Focus();
            this.AndConnectionList.Clear();
            this.OrConnectionList.Clear();    
            for (int i = 0; i < conditionList.Count; ++i)
            {
                Connection newConnection = new Connection(triggerItem.RightConnector[0], conditionList[i].LeftConnector);
                addDesignerItemConnection(triggerItem.RightConnector[0], conditionList[i].LeftConnector, newConnection);
                this.Children.Add(newConnection);            
                Connection ConditionExpConnector= new Connection(conditionList[i].RightConnector[0], DrawComponent.expDic[i]);
                addDesignerItemConnection(conditionList[i].RightConnector[0], DrawComponent.expDic[i], ConditionExpConnector);
                if (DrawComponent.AndExp.Contains(i))
                {
                    this.AndConnectionList.Add(ConditionExpConnector);
                }
                else 
                {
                    this.OrConnectionList.Add(ConditionExpConnector);
                }
                this.Children.Add(ConditionExpConnector);
            }

            for (int i = 0; i < actionItemList.Count; ++i)
            {
                Connection newConnection = new Connection(ExpConditionResult.RightConnector[0], actionItemList[i].LeftConnector);
                addDesignerItemConnection(ExpConditionResult.RightConnector[0], actionItemList[i].LeftConnector, newConnection);
                this.Children.Add(newConnection);
            }
        }

        private void addDesignerItemConnection(Connector sourceConnector, Connector sinkConnector, Connection connection)
        {
            DesignerItem sourceDesignerItem = sourceConnector.ParentDesignerItem;
            DesignerItem sinkDesignerItem = sinkConnector.ParentDesignerItem;
            sourceDesignerItem.rightConnectionList.Add(connection);
            sinkDesignerItem.leftConnectionList.Add(connection);
        }

        #endregion

        #region Copy Command

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Paste Command

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return;

            // create DesignerItems
            Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            List<ISelectable> newItems = new List<ISelectable>();
            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");

            double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            foreach (XElement itemXML in itemsXML)
            {
                Guid oldID = new Guid(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);
                DesignerItem item = DeserializeDesignerItem(itemXML, newID, offsetX, offsetY);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
                newItems.Add(item);
            }

            // update group hierarchy
            SelectionService.ClearSelection();
            foreach (DesignerItem el in newItems)
            {
                if (el.ParentID != Guid.Empty)
                    el.ParentID = mappingOldToNewIDs[el.ParentID];
            }


            foreach (DesignerItem item in newItems)
            {
                if (item.ParentID == Guid.Empty)
                {
                    SelectionService.AddToSelection(item);
                }
            }

            // create Connections
            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid oldSinkID = new Guid(connectionXML.Element("SinkID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldSinkID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newSinkID = mappingOldToNewIDs[oldSinkID];

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(newSourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(newSinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector);
                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                    this.Children.Add(connection);

                    SelectionService.AddToSelection(connection);
                }
            }

            DesignerCanvas.BringToFront.Execute(null, this);

            // update paste offset
            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        #endregion

        #region Delete Command

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrentSelection();
        }

        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Cut Command

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            DeleteCurrentSelection();
        }

        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Group Command

        private void Group_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var items = from item in this.SelectionService.CurrentSelection.OfType<DesignerItem>()
                        where item.ParentID == Guid.Empty
                        select item;

            Rect rect = GetBoundingRectangle(items);

            DesignerItem groupItem = new DesignerItem();
            groupItem.IsGroup = true;
            groupItem.Width = rect.Width;
            groupItem.Height = rect.Height;
            Canvas.SetLeft(groupItem, rect.Left);
            Canvas.SetTop(groupItem, rect.Top);
            Canvas groupCanvas = new Canvas();
            groupItem.Content = groupCanvas;
            Canvas.SetZIndex(groupItem, this.Children.Count);
            this.Children.Add(groupItem);

            foreach (DesignerItem item in items)
                item.ParentID = groupItem.ID;

            this.SelectionService.SelectItem(groupItem);
        }

        private void Group_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            int count = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                         where item.ParentID == Guid.Empty
                         select item).Count();

            e.CanExecute = count > 1;
        }

        #endregion

        #region Ungroup Command

        private void Ungroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var groups = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                          where item.IsGroup && item.ParentID == Guid.Empty
                          select item).ToArray();

            foreach (DesignerItem groupRoot in groups)
            {
                var children = from child in SelectionService.CurrentSelection.OfType<DesignerItem>()
                               where child.ParentID == groupRoot.ID
                               select child;

                foreach (DesignerItem child in children)
                    child.ParentID = Guid.Empty;

                this.SelectionService.RemoveFromSelection(groupRoot);
                this.Children.Remove(groupRoot);
                UpdateZIndex();
            }
        }

        private void Ungroup_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                              where item.ParentID != Guid.Empty
                              select item;


            e.CanExecute = groupedItem.Count() > 0;
        }

        #endregion

        #region BringForward Command

        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) descending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Min(count - 1 - i, currentIndex + 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
            e.CanExecute = true;
        }

        #endregion

        #region BringToFront Command

        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();

            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, childrenSorted.Count - selectionSorted.Count + j++);
                }
                else
                {
                    Canvas.SetZIndex(item, i++);
                }
            }
        }

        #endregion

        #region SendBackward Command

        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) ascending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Max(i, currentIndex - 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region SendToBack Command

        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();
            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, j++);

                }
                else
                {
                    Canvas.SetZIndex(item, selectionSorted.Count + i++);
                }
            }
        }        

        #endregion

        #region AlignTop Command

        private void AlignTop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = top - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        private void Align_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
            //                  where item.ParentID == Guid.Empty
            //                  select item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region AlignVerticalCenters Command

        private void AlignVerticalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignBottom Command

        private void AlignBottom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignLeft Command

        private void AlignLeft_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = left - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignHorizontalCenters Command

        private void AlignHorizontalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double center = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = center - (Canvas.GetLeft(item) + item.Width / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignRight Command

        private void AlignRight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double right = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = right - (Canvas.GetLeft(item) + item.Width);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region DistributeHorizontal Command

        private void DistributeHorizontal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemLeft = Canvas.GetLeft(item)
                                orderby itemLeft
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Double.MaxValue;
                double right = Double.MinValue;
                double sumWidth = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    left = Math.Min(left, Canvas.GetLeft(item));
                    right = Math.Max(right, Canvas.GetLeft(item) + item.Width);
                    sumWidth += item.Width;
                }

                double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
                double offset = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                    offset = offset + item.Width + distance;
                }
            }
        }

        private void Distribute_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
            //                  where item.ParentID == Guid.Empty
            //                  select item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region DistributeVertical Command

        private void DistributeVertical_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemTop = Canvas.GetTop(item)
                                orderby itemTop
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Double.MaxValue;
                double bottom = Double.MinValue;
                double sumHeight = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    top = Math.Min(top, Canvas.GetTop(item));
                    bottom = Math.Max(bottom, Canvas.GetTop(item) + item.Height);
                    sumHeight += item.Height;
                }

                double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
                double offset = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                    offset = offset + item.Height + distance;
                }
            }
        }

        #endregion

        #region SelectAll Command

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.SelectAll();
        }

        #endregion

        #region Helper Methods

        private XElement LoadSerializedDataFromFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                try
                {
                    return XElement.Load(openFile.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        void SaveFile(XElement xElement)
        {
            //SaveFileDialog saveFile = new SaveFileDialog();
            //saveFile.Filter = "Files (*.xml)|*.xml|All Files (*.*)|*.*";
            //if (saveFile.ShowDialog() == true)
            //{
            //    try
            //    {
            //        xElement.Save(saveFile.FileName);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //PythonParse.PythonStructure(1105, triggerList[0], conditionList, actionList, andOrder, orOrder);
            //ExportCode.codePath = AppDomain.CurrentDomain.BaseDirectory + "Code";
            //ExportCode.triggerParse(triggerList);
        }

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        private XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                       from item in designerItems
                                       let contentXaml = XamlWriter.Save(((DesignerItem)item).Content)
                                       select new XElement("DesignerItem",
                                                  new XElement("Left", Canvas.GetLeft(item)),
                                                  new XElement("Top", Canvas.GetTop(item)),
                                                  new XElement("Width", item.Width),
                                                  new XElement("Height", item.Height),
                                                  new XElement("ID", item.ID),
                                                  new XElement("zIndex", Canvas.GetZIndex(item)),
                                                  new XElement("IsGroup", item.IsGroup),
                                                  new XElement("ParentID", item.ParentID),
                                                  new XElement("Content", contentXaml)
                                              )
                                   );

            return serializedItems;
        }

        private XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           from connection in connections
                           select new XElement("Connection",
                                      new XElement("SourceID", connection.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", connection.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", connection.Source.Name),
                                      new XElement("SinkConnectorName", connection.Sink.Name),
                                      new XElement("SourceArrowSymbol", connection.SourceArrowSymbol),
                                      new XElement("SinkArrowSymbol", connection.SinkArrowSymbol),
                                      new XElement("zIndex", Canvas.GetZIndex(connection))
                                     )
                                  );

            return serializedConnections;
        }

        private static DesignerItem DeserializeDesignerItem(XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            DesignerItem item = new DesignerItem(id);
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            item.ParentID = new Guid(itemXML.Element("ParentID").Value);
            item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
            Object content = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("Content").Value)));
            item.Content = content;
            return item;
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<DesignerItem> selectedDesignerItems =
                this.SelectionService.CurrentSelection.OfType<DesignerItem>();

            List<Connection> selectedConnections =
                this.SelectionService.CurrentSelection.OfType<Connection>().ToList();

            foreach (Connection connection in this.Children.OfType<Connection>())
            {
                if (!selectedConnections.Contains(connection))
                {
                    DesignerItem sourceItem = (from item in selectedDesignerItems
                                               where item.ID == connection.Source.ParentDesignerItem.ID
                                               select item).FirstOrDefault();

                    DesignerItem sinkItem = (from item in selectedDesignerItems
                                             where item.ID == connection.Sink.ParentDesignerItem.ID
                                             select item).FirstOrDefault();

                    if (sourceItem != null &&
                        sinkItem != null &&
                        BelongToSameGroup(sourceItem, sinkItem))
                    {
                        selectedConnections.Add(connection);
                    }
                }
            }

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems);
            XElement connectionsXML = SerializeConnections(selectedConnections);
            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        public void NewCanvas()
        {
            for (int i = 0; i < conditionList.Count; ++i)
            {
                DeleteConnection(conditionList[i]);
            }
            conditionList.Clear();
            for (int i = 0; i < expList.Count; ++i)
            {
                DeleteConnection(expList[i]);
            }
            expList.Clear();
            for (int i = 0; i < actionList.Count; ++i)
            {
                DeleteConnection(actionList[i]);
            }
            actionList.Clear();
            for (int i = 0; i < triggerList.Count; ++i)
            {
                DeleteConnection(triggerList[i]);
            }
            triggerList.Clear();
            this.Children.Clear();
            targetList.Clear();
            actionList.Clear();
            targetPanel.Children.Clear();
            conditionList.Clear();
            //AIBar.Children.Clear();
            AndConnectionList.Clear();
            OrConnectionList.Clear();
            andOrder.Clear();
            orOrder.Clear();
            conditionLinker.Clear();
            actionLinker.Clear();
            ListData.Clear();
            this.SelectionService.ClearSelection();
            Init_Executed();
        }

        private void DeleteCurrentSelection()
        {
            //foreach (Connection connection in SelectionService.CurrentSelection.OfType<Connection>())
            //{
            //    this.Children.Remove(connection);
            //}
            foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
            {
                //Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                //List<Connector> connectors = new List<Connector>();
                //GetConnectors(cd, connectors);
                //foreach (Connector connector in connectors)
                //{
                //    foreach (Connection con in connector.Connections)
                //    {
                //        this.Children.Remove(con);
                //    }
                //}
                DeleteFromList(item);
                DeleteConnection(item);
                this.Children.Remove(item);                
            }
            SelectionService.ClearSelection();
            UpdateZIndex();
        }

        private void UpdateZIndex()
        {
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerItem> items)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (DesignerItem item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }

        private Connector GetConnector(Guid itemID, String connectorName)
        {
            DesignerItem designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.ID == itemID
                                         select item).FirstOrDefault();

            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            connectorDecorator.ApplyTemplate();

            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }

        private bool BelongToSameGroup(IGroupable item1, IGroupable item2)
        {
            IGroupable root1 = SelectionService.GetGroupRoot(item1);
            IGroupable root2 = SelectionService.GetGroupRoot(item2);
            return (root1.ID == root2.ID);
        }

        #endregion
    }
}
