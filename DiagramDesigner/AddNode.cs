using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Text;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DiagramDesigner
{
    class AddNode
    {
        private Border border;
        private GroupBox groupBox;
        public DesignerItem newItem;
        DecideColor decide = new DecideColor();
        public List<ComboData> comboData = new List<ComboData>();
        public AddNode()
        {
        }
        public DesignerItem addNode(string nodeName, string nodeFileName)
        {
            XmlParse parse = new XmlParse(nodeName, nodeFileName);
            parse.parseXml();
            groupBox = new GroupBox();
            //groupBox.Cursor = Cursors.SizeAll;
            groupBox.Background = new SolidColorBrush(Colors.LightSlateGray);
            groupBox.Foreground = new SolidColorBrush(Colors.DarkBlue);
            Thickness thick = new Thickness(0, 0, 0, 0);
            groupBox.Margin = thick;
            Thickness groupBorderThick = new Thickness(0);
            groupBox.BorderThickness = groupBorderThick;
            groupBox.BorderBrush = Brushes.Transparent;
            groupBox.Width = 180;
            groupBox.MinHeight = 40;
            groupBox.Header = nodeName;

            border = new Border();
            //border.BorderBrush = Brushes.Bisque;
            Thickness borderthick = new Thickness(2);
            border.BorderThickness = borderthick;
            CornerRadius bordercornerRadius = new CornerRadius(10);
            border.CornerRadius = bordercornerRadius;
            //border.Background = new SolidColorBrush(Colors.LightBlue);
            Thickness innerpadding = new Thickness(0, 5, 0, 0);
            border.Height = groupBox.Height;
            border.Margin = innerpadding;
            Grid grid = new Grid();
            //grid.Background = new SolidColorBrush(Colors.LightYellow);
            grid.Background = decide.typeColor(nodeFileName);
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            newItem = new DesignerItem();
            ListView listView1 = new ListView();
            newItem.IDName = nodeName;
            if (nodeName.Equals("AndExp"))
            {
                newItem.ItemType = "ConditionAndExp";
            }
            else if (nodeName.Equals("OrExp"))
            {
                newItem.ItemType = "ConditionOrExp";
            }
            else if (nodeName.Equals("Exp"))
            {
                newItem.ItemType = "ConditionExpResult";
            }
            else
            {
                newItem.ItemType = nodeFileName;
            }
            listView1.Width = 80;
            for (int i = 0; i < parse.input.Length; ++i)
            {
                string connectorName = parse.input[i].Name;
                string connectorType = parse.input[i].Type;
                string connectorDescribe = parse.input[i].Value;
                addTextBlockOrTextBoxByType(listView1, connectorName, connectorDescribe, connectorType, newItem, "left");    
            }

            ListView listView2 = new ListView();
            listView2.Width = 80;
            for (int i = 0; i < parse.output.Length; ++i)
            {
                string connectorName = parse.output[i].Name;
                string connectorType = parse.output[i].Type;
                string connectorDescribe = parse.output[i].Value;
                addTextBlockOrTextBoxByType(listView2, connectorName, connectorDescribe, connectorType, newItem, "right");               
            }
            Grid.SetColumn(listView1, 0);
            Grid.SetColumn(listView2, 1);
            grid.Children.Add(listView1);
            grid.Children.Add(listView2);
            groupBox.Content = grid;
            border.Child = groupBox;
            newItem.Content = border;
            
            newItem.Width = border.Width + 6;
            newItem.Height = border.Height + 10;
            return newItem;
        }

        ConnectorDataType GetDataType(string type)
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

        void addTextBlockOrTextBoxByType(ListView listView, string connectorName, string describe, string connectorType, DesignerItem item, string direction)
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
                DrawComponent.GetConnectorType(co1, item.ItemType, direction);
                co1.Orientation = direction == "left" ? (ConnectorOrientation.Left) : (ConnectorOrientation.Right);
                co1.DataType = GetDataType(connectorType);
                co1.Content = listItem;
                co1.HorizontalAlignment = HorizontalAlignment.Left;
                listView.Items.Add(co1);
            }
            else if(connectorType == "Combo")
            {
                ComboBox comboBox = new ComboBox();
                comboBox.ItemsSource = comboData;
                comboBox.DisplayMemberPath = "Value";
                comboBox.SelectedValuePath = "Id";
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
                //textBox.ParentDesignerItem = item;
                textBox.propertyValue = connectorName;
                textBox.TextAlignment = TextAlignment.Center;
                textBox.Width = 80;
                textBox.Height = 25;
                textBox.ToolTip = describe;
                if(connectorType!="OrderInteger")
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
    }
}

