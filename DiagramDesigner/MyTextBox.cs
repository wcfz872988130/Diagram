using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Windows.Media;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace DiagramDesigner
{
    public class MyTextBox:TextBox
    {
        public string type;
        public MyTextBox()
        {
        }
        public MyTextBox(string type)
        {
            this.type = type;
        }

        public string Property { get; set; }
        public string propertyValue { get; set; }
        public ListView ParentListView { get; set; }
        public DesignerItem ParentDesignerItem { get; set; }
        //public DesignerItem ParentDesignerItem { get; set; }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            TextBox tb = e.Source as TextBox;
            tb.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
            //tb.PreviewMouseDoubleClick += new MouseButtonEventHandler(textBox_MouseDoubleClick);
            DoTextBoxChange(this.Text, type);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            TextBox tb = e.Source as TextBox;
            tb.SelectAll();
            tb.AllowDrop = false;
            //tb.PreviewMouseDoubleClick -= new MouseButtonEventHandler(textBox_MouseDoubleClick);
            tb.PreviewMouseDown -= new MouseButtonEventHandler(OnPreviewMouseDown);
        }
        void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.Focus();
            e.Handled = true;
        }
        void textBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
            TextBox tb = e.Source as TextBox;
            tb.Focus();
            e.Handled = true;
        }

        void DoTextBoxChange(string inputContent, string type)
        {
            switch (type)
            {
                case "Integer":
                    Regex regex0 = new Regex("^[0-9]*[1-9][0-9]*$");
                    if (!regex0.IsMatch(inputContent))
                    {
                        MessageBox.Show("请输入正整数类型");
                    }
                    else
                    {
                        propertyValue = inputContent;
                    }
                    break;
                case "Float":
                    Regex regex1 = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
                    if (!regex1.IsMatch(inputContent))
                    {
                        MessageBox.Show("请输入浮点数类型");
                    }
                    else
                    {
                        propertyValue = inputContent;
                    }
                    break;
                case "String":
                    Regex regex2 = new Regex(@"^[0-9]*$");
                    if (!regex2.IsMatch(inputContent))
                    {
                        MessageBox.Show("请输入正确格式！");
                    }
                    else
                    {
                        propertyValue = inputContent;
                    }
                    break;
                case "InputInteger":
                    Regex regex3 = new Regex("^[0-9]*[1-9][0-9]*$");
                    if (!regex3.IsMatch(inputContent))
                    {
                        MessageBox.Show("请输入正整数类型");
                    }
                    else
                    {
                        int num = int.Parse(inputContent);
                        //ParentDesignerItem.addComboBox(num, ParentListView);
                        List<string> newExp = new List<string>();
                        DrawComponent.addComboBox(num, ParentListView);
                        propertyValue = inputContent;
                    }
                    break;
                case "OrderInteger":
                    Regex regex4 = new Regex("^[0-9]*[1-9][0-9]*$");
                    if (!regex4.IsMatch(inputContent))
                    {
                        MessageBox.Show("请输入正整数类型");
                    }
                    else
                    {
                        int num = int.Parse(inputContent);
                        ParentDesignerItem.ItemOrder = num;
                    }
                    break;
            }
        }
    }
}
