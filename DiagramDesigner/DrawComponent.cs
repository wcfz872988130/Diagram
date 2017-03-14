using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DiagramDesigner
{
    class DrawComponent
    {
        static DecideColor decide = new DecideColor();
        public static Dictionary<string, string> monsterParam = new Dictionary<string, string>();
        public static Dictionary<int, string> expComboBox = new Dictionary<int, string>();
        public static Dictionary<int, Connector> expDic = new Dictionary<int,Connector>();
        public static Dictionary<string, List<string>> paramDictionary;
        public static Dictionary<string, List<string>> paramDescribe;
        public static List<int> AndExp = new List<int>();
        public static List<int> OrExp = new List<int>();
        public static DesignerCanvas canvas = null;

        public static List<ComboData> ListData = new List<ComboData>();
        public static DesignerItem DrawTriggerComponent(string triggerName,List<string> triggerParamList)
        {
            Dictionary<int, string> protoType = new Dictionary<int, string>();
            protoType[0] = triggerName;
            Dictionary<int, List<string>> param = new Dictionary<int, List<string>>();
            if(triggerParamList != null)
                param[0] = triggerParamList;
            List<DesignerItem> item = addNode("Trigger", protoType, param);
            return item[0];
        }

        //public static Dictionary<int, DesignerItem> addTarget()
        //{ }

        public static DesignerItem addNode(string type, string protoType, List<string> param)
        {
            DesignerItem item = new DesignerItem();
            GroupBox groupBox = new GroupBox();
            groupBox.Background = new SolidColorBrush(Colors.LightSlateGray);
            groupBox.Foreground = new SolidColorBrush(Colors.DarkBlue);
            groupBox.BorderBrush = Brushes.Transparent;
            groupBox.Width = 180;
            groupBox.Header = protoType;

            Border border = new Border();
            Thickness borderthick = new Thickness(2);
            border.BorderThickness = borderthick;
            CornerRadius bordercornerRadius = new CornerRadius(10);
            border.CornerRadius = bordercornerRadius;
            Thickness innerpadding = new Thickness(0, 5, 0, 0);
            border.Height = groupBox.Height;
            border.Margin = innerpadding;

            Grid grid = new Grid();
            grid.Background = decide.typeColor(type);
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            ListView listView1 = new ListView();
            listView1.Width = 80;
            ListView listView2 = new ListView();
            listView2.Width = 80;

            addTempleteByType(listView1, listView2, type, item);
            List<string> connectorTypeList = paramDictionary[protoType];
            List<string> connectorTypeDescribe = paramDescribe[protoType];
            for (int i = 0; i < param.Count; ++i)
            {
                string paramNodeName = param[i];
                string connectorType = connectorTypeList[i];
                string connectorDescribe = connectorTypeDescribe[i];
                addTextBlockOrTextBoxByType(listView1, paramNodeName, connectorDescribe, connectorType, item, "left");
            }

            Grid.SetColumn(listView1, 0);
            Grid.SetColumn(listView2, 1);
            grid.Children.Add(listView1);
            grid.Children.Add(listView2);
            groupBox.Content = grid;
            border.Child = groupBox;
            item.Content = border;
            item.IDName = protoType;
            item.ItemType = type;
            item.Width = border.Width + 6;
            item.Height = border.Height + 10;

            return item;
        }

        public static List<DesignerItem> addNode(string type, Dictionary<int, string> protoType, Dictionary<int, List<string>> param, Dictionary<int, int> linker = null)
        {
            List<DesignerItem> listItem = new List<DesignerItem>();
            int order = 1;
            var dicSort = from objDic in protoType orderby objDic.Key ascending select objDic;
            foreach (KeyValuePair<int, string> kvp in dicSort)
            {
                string nodeName = kvp.Value;
                int nodeIndex = kvp.Key;
                List<string> nodeParam = new List<string>();
                DesignerItem newItem = new DesignerItem();
                newItem.ItemType = type;
                GroupBox groupBox = new GroupBox();
                groupBox.Background = new SolidColorBrush(Colors.LightSlateGray);
                groupBox.Foreground = new SolidColorBrush(Colors.DarkBlue);

                groupBox.BorderBrush = Brushes.Transparent;
                groupBox.Width = 180;
                groupBox.Header = nodeName;

                Border border = new Border();
                Thickness borderthick = new Thickness(2);
                border.BorderThickness = borderthick;
                CornerRadius bordercornerRadius = new CornerRadius(10);
                border.CornerRadius = bordercornerRadius;
                Thickness innerpadding = new Thickness(0, 5, 0, 0);
                border.Height = groupBox.Height;
                border.Margin = innerpadding;

                Grid grid = new Grid();
                grid.Background = decide.typeColor(type);
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                ListView listView1 = new ListView();
                listView1.Width = 80;
                ListView listView2 = new ListView();
                listView2.Width = 80;

                addTempleteByType(listView1, listView2, type, newItem);

                if(param.Count > 0 && param.ContainsKey(nodeIndex))
                {
                    nodeParam = param[nodeIndex];
                }
                
                List<string> connectorTypeList = paramDictionary[nodeName];
                List<string> connectorTypeDescribe = paramDescribe[nodeName];

                if(type.Equals("Condition")||type.Equals("Action"))
                {
                    string describe = null;
                    switch (type)
                    {
                        case "Condition":
                            describe = "条件的顺序";                            
                            break;
                        case "Action":
                            describe = "动作的顺序";
                            break;
                    }
                    string index = "";
                    if (linker.ContainsKey(nodeIndex))
                        index = linker[nodeIndex].ToString();
                    addTextBlockOrTextBoxByType(listView2, index, describe, "Combo", newItem, "right");
                    addTextBlockOrTextBoxByType(listView1, order.ToString(), describe, "OrderInteger", newItem, "left");
                    newItem.ItemOrder = order;
                    order++;
                }

                for (int i = 0; i < nodeParam.Count; ++i)
                {
                    string paramNodeName = nodeParam[i];
                    string connectorType = connectorTypeList[i];
                    string connectorDescribe = connectorTypeDescribe[i];
                    addTextBlockOrTextBoxByType(listView1, paramNodeName, connectorDescribe, connectorType, newItem, "left");
                }

                if (type.Equals("ConditionAndExp"))
                {
                    addComboBox2(listView1, expComboBox, true);
                }

                if(type.Equals("ConditionOrExp"))
                {
                    addComboBox2(listView1, expComboBox, false);
                }

                if(type.Equals("Target"))
                {
                    if (ListData.Count == 0)
                        ListData.Add(new ComboData { Id = -1, Value = ""});
                    int id = ListData.Count - 1;
                    string value = id.ToString();
                    ListData.Add(new ComboData { Id = id, Value = value });
                    newItem.ItemOrder = nodeIndex;
                }
                Grid.SetColumn(listView1, 0);
                Grid.SetColumn(listView2, 1);
                grid.Children.Add(listView1);
                grid.Children.Add(listView2);
                groupBox.Content = grid;
                border.Child = groupBox;
                newItem.Content = border;
                newItem.IDName = nodeName;
                
                newItem.Width = border.Width + 6;
                newItem.Height = border.Height + 10;
                listItem.Add(newItem);
            }
            return listItem;
        }

        private static ConnectorDataType GetDataType(string type)
        {
            ConnectorDataType datatype = ConnectorDataType.None;
            switch (type)
            {
                case "SingleLinker":
                    datatype = ConnectorDataType.SingleLinker;
                    break;
                case "MultiLinker":
                    datatype = ConnectorDataType.MultiLinker;
                    break;
                default:
                    datatype = ConnectorDataType.None;
                    break;
            }
            return datatype;
        }

        private static void addTempleteByType(ListView listView, ListView listView2, string type, DesignerItem item)
        {
            switch(type)
            {
                case "Trigger":
                    addTextBlockOrTextBoxByType(listView, "monster", "", "SingleLinker", item, "left");
                    addTextBlockOrTextBoxByType(listView2, "condition", "", "MultiLinker", item, "right");
                    break;
                case "Condition":
                    addTextBlockOrTextBoxByType(listView, "trigger", "", "SingleLinker", item, "left");
                    //addTextBlockOrTextBoxByType(listView2, "target", "", "Combo", item, "right");
                    addTextBlockOrTextBoxByType(listView2, "conditionExp", "", "SingleLinker", item, "right");
                    break;
                case "Action":
                    addTextBlockOrTextBoxByType(listView, "conditionExp", "", "SingleLinker", item, "left");
                    //addTextBlockOrTextBoxByType(listView2, "target", "", "Combo", item, "right");
                    break;
                case "ConditionAndExp":
                    addTextBlockOrTextBoxByType(listView2, "exp", "", "MultiLinker", item, "right");
                    break;
                case "ConditionOrExp":
                    addTextBlockOrTextBoxByType(listView2, "exp", "", "MultiLinker", item, "right");
                    break;
                case "ConditionExpResult":
                    addTextBlockOrTextBoxByType(listView, "exp", "", "MultiLinker", item, "left");
                    addTextBlockOrTextBoxByType(listView2, "action", "", "MultiLinker", item, "right");
                    break;
            }
        }

        private static void addTextBlockOrTextBoxByType(ListView listView, string connectorName, string describe, string connectorType, DesignerItem item, string direction)
        {
            Grid listItem = new Grid();
            listItem.Width = 80;
            listItem.Height = 25;

            if (connectorType == "SingleLinker" || connectorType == "MultiLinker")
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = connectorName;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Width = 80;
                textBlock.Height = 25;
                textBlock.ToolTip = describe;
                textBlock.HorizontalAlignment = direction == "left" ? (HorizontalAlignment.Left) : (HorizontalAlignment.Right);
                SolidColorBrush typeColor = decide.typeColor(connectorType);
                textBlock.Background = typeColor;
                listItem.Children.Add(textBlock);

                Connector co1 = new Connector();
                GetConnectorType(co1, item.ItemType, direction);
                co1.Orientation = direction == "left" ? (ConnectorOrientation.Left) : (ConnectorOrientation.Right);
                co1.DataType = GetDataType(connectorType);
                co1.Content = listItem;
                co1.HorizontalAlignment = HorizontalAlignment.Left;
                listView.Items.Add(co1);
                if (direction == "left")
                {
                    item.LeftConnector = co1;
                }
                else 
                {
                    item.RightConnector.Add(co1);
                }
            }
            else if (connectorType == "Combo")
            {
                ComboBox comboBox = new ComboBox();
                comboBox.ItemsSource = ListData;
                comboBox.DisplayMemberPath = "Value";
                comboBox.SelectedValuePath = "Id";
                int idex = 0;
                if (int.TryParse(connectorName, out idex))
                {
                    comboBox.SelectedIndex = idex + 1;
                }
                comboBox.Width = 80;
                comboBox.Height = 25;
                item.myCombo = comboBox;
                listView.Items.Add(comboBox);
            }
            else
            {
                MyTextBox textBox = new MyTextBox(connectorType);
                textBox.Text = connectorName;
                textBox.Property = connectorName;
                textBox.ParentListView = listView;
                textBox.ParentDesignerItem = item;
                textBox.propertyValue = connectorName;
                textBox.TextAlignment = TextAlignment.Center;
                textBox.Width = 80;
                textBox.Height = 25;
                textBox.ToolTip = describe;
                if (!connectorType.Equals("OrderInteger"))
                {
                    item.outputValue.Add(textBox);
                }                
                textBox.HorizontalAlignment = direction == "left" ? (HorizontalAlignment.Left) : (HorizontalAlignment.Right);
                SolidColorBrush typeColor = decide.typeColor(connectorType);
                textBox.Background = typeColor;
                Interaction.GetBehaviors(textBox).Add(new TextBoxEnterKeyUpdateBehavior());
                listItem.Children.Add(textBox);

                Grid co2 = new Grid();
                co2.Children.Add(listItem);
                co2.HorizontalAlignment = HorizontalAlignment.Left;
                listView.Items.Add(co2);
            }
        }

        public static void addComboBox(int num, ListView listView)
        {
            int conNum = 0;
            int length = listView.Items.Count;
            for (int i = 1; i < length; ++i)
            {
                Connector con = listView.Items[i] as Connector;
                if(con.ConnectorHasConnected)
                {
                    conNum++;
                }
            }

            if(num < conNum)
            {
                MessageBox.Show("所填数目不能小于已连接数目");
                return;
            }
            
            if((num - length) >= 0)
            {
                int extraLength = num - length + 1;
                for (int i = 0; i < extraLength; ++i)
                {
                    ComboBox cBox = new ComboBox();
                    cBox.Width = 80;
                    cBox.Height = 25;
                    cBox.ItemsSource = new List<string> { "True", "False" };
                    cBox.SelectedIndex = 0;
                    Connector co1 = new Connector();
                    co1.SelfType = "ConditionAndOrExp";
                    co1.Orientation = ConnectorOrientation.Left;
                    co1.DataType = ConnectorDataType.SingleLinker;
                    co1.Content = cBox;
                    co1.HorizontalAlignment = HorizontalAlignment.Left;
                    listView.Items.Add(co1);
                }
            }
            else if((num - length) < -1)
            {
                int redundantLength = length - num - 1;
                for (int i = 0; i < redundantLength; ++i)
                {
                    int position = listView.Items.Count - 1;
                    listView.Items.RemoveAt(position);
                }
            }
            listView.Items.Refresh();
        }
        public static void addComboBox2(ListView listView, Dictionary<int, string> expcomboBox, bool flag)
        {
            int length = listView.Items.Count;
            for (int i = 1; i < length; ++i)
            {
                listView.Items.RemoveAt(1);
            }

            foreach (KeyValuePair<int, string> vp in expcomboBox)
            {
                ComboBox newcBox = new ComboBox();
                newcBox.Width = 80;
                newcBox.Height = 25;
                newcBox.ItemsSource = new List<string> { "True", "False" };
                int expIndex = vp.Key;
                int index = vp.Value == "True" ? 0 : 1;
                newcBox.SelectedIndex = index;
                Connector co1 = new Connector();
                co1.SelfType = "ConditionAndOrExp";
                co1.Orientation = ConnectorOrientation.Left;
                co1.DataType = ConnectorDataType.SingleLinker;
                co1.Content = newcBox;
                co1.HorizontalAlignment = HorizontalAlignment.Left;
                expDic[expIndex] = co1;
                if (flag)
                {
                    AndExp.Add(expIndex);
                }
                else 
                {
                    OrExp.Add(expIndex);
                }
                listView.Items.Add(co1);
            }

            listView.Items.Refresh();
        }

        private static Border addBorder(int elementHeight, int radiu)
        {
            Border border = new Border();
            Thickness borderthick = new Thickness(2);
            border.BorderThickness = borderthick;
            CornerRadius bordercornerRadius = new CornerRadius(radiu);
            border.CornerRadius = bordercornerRadius;
            Thickness innerpadding = new Thickness(0, 3, 0, 2);
            border.Margin = innerpadding;
            border.Height = elementHeight;
            border.BorderBrush = new SolidColorBrush(Colors.SlateGray);
            return border;
        }

        public static List<GroupBox> addMyGrid(Dictionary<string, Dictionary<int, List<int>>> assemble)
        {
            List<GroupBox> groupBoxList = new List<GroupBox>();
            foreach(KeyValuePair<string, Dictionary<int, List<int>>> kvp in assemble)
            {
                string name = kvp.Key;
                Dictionary<int, List<int>> allAI = kvp.Value;
                GroupBox groupBox = new GroupBox();
                groupBox.Header = name;
                Thickness groupMargin = new Thickness(3, 0, 0, 3);
                groupBox.Margin = groupMargin;
                groupBox.Style = null;

                StackPanel priorityPanel = new StackPanel();
                priorityPanel.Orientation = Orientation.Horizontal;
                foreach (KeyValuePair<int, List<int>> kp in allAI)
                {
                    int priority = kp.Key;
                    List<int> aiList = kp.Value;
                    Grid newGrid = new Grid();
                    newGrid.Background = new SolidColorBrush(Colors.White);
                    RowDefinition rowOne = new RowDefinition();
                    var converter = new GridLengthConverter();
                    rowOne.Height = (GridLength)converter.ConvertFromString("*");
                    newGrid.RowDefinitions.Add(rowOne);
                    RowDefinition rowTwo = new RowDefinition();
                    rowTwo.Height = (GridLength)converter.ConvertFromString("2*");
                    newGrid.RowDefinitions.Add(rowTwo);
                    StackPanel NodePanel = new StackPanel();
                    NodePanel.Background = new SolidColorBrush(Colors.Beige);
                    NodePanel.Orientation = Orientation.Horizontal;
                    NodePanel.Height = 30;

                    for (int i = 0; i < aiList.Count; ++i)
                    {
                        MyButton numBlock = new MyButton();
                        numBlock.AID = aiList[i].ToString();
                        numBlock.canvas = canvas;
                        numBlock.Content = aiList[i].ToString();
                        numBlock.FontSize = 20;
                        numBlock.Width = 75;
                        numBlock.Height = 30;
                        NodePanel.Children.Add(numBlock);
                    }

                    TextBlock textBlock = new TextBlock();
                    textBlock.Width = 200;
                    textBlock.Height = 25;
                    textBlock.Text = priority.ToString();
                    textBlock.FontSize = 20;
                    //textBlock.Background = new SolidColorBrush(Colors.AliceBlue);
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                    textBlock.TextAlignment = TextAlignment.Center;

                    Grid.SetRow(textBlock, 0);
                    Grid.SetRow(NodePanel, 1);
                    newGrid.Children.Add(textBlock);
                    newGrid.Children.Add(NodePanel);
                    Border fullBorder = addBorder(100, 3);
                    fullBorder.Child = newGrid;
                    priorityPanel.Children.Add(fullBorder);
                }               
                groupBox.Content = priorityPanel;
                groupBoxList.Add(groupBox);
            }
            return groupBoxList;
        }

        public static void ReWriteDesignerItemValue(DesignerItem item, Dictionary<string, string> param)
        {
            foreach (KeyValuePair<string, string> kvp in param)
            {
                string textBoxName = kvp.Key;
                string value = kvp.Value;
                for (int i = 0; i < item.outputValue.Count; ++i)
                {
                    if(textBoxName.Equals(item.outputValue[i].Property))
                    {
                        item.outputValue[i].propertyValue = value;
                        item.outputValue[i].Text = value;
                        break;
                    }
                }
            }
        }

        public static void GetConnectorType(Connector con, string type, string direction)
        {
            switch (type)
            {
                case "Trigger":
                    if (direction.Equals("right"))
                    {
                        con.NextType = "Condition";
                    }
                    else { con.SelfType = "Trigger"; }
                    break;
                case "Condition":
                    if (direction.Equals("right"))
                    {
                        con.NextType = "ConditionAndOrExp";
                    }
                    else { con.SelfType = "Condition"; }
                    break;
                case "ConditionAndExp":
                    con.NextType = "ConditionExpResult";
                    break;
                case "ConditionOrExp":
                    con.NextType = "ConditionExpResult";
                    break;
                case "ConditionExpResult":
                    if (direction.Equals("right"))
                    {
                        con.NextType = "Action";
                    }
                    else { con.SelfType = "ConditionExpResult"; }
                    break;
                case "Action":
                    con.SelfType = "Action";
                    break;
            }
        }

    }
}
