using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            //MonsterConnector.
            List<string> sections = new List<string> { "Monster_ID", "Monster_Name", "Monster_Tag" };
            SearchFrame.SectionsList = sections;
            SearchFrame.SectionsStyle = SearchTextBox.SectionsStyles.RadioBoxStyle;
            SearchFrame.OnSearch += new RoutedEventHandler(OnSearch);

            MyDesignerCanvas.targetPanel = TargetPanel;
            MyDesignerCanvas.AIBar = AiBar;
            //List<int> priorityList = new List<int> { 1, 2, 3 };
            //Border newBorder = DrawComponent.addMyGrid("thinkTrigger", priorityList);
            //AiBar.Children.Add(newBorder);
        }

        void OnSearch(object sender, RoutedEventArgs e)
        {
            SearchEventArgs searchArgs = e as SearchEventArgs;
            foreach (string section in searchArgs.Sections)
            {
                string input = searchArgs.Keyword;
                string searchType = section;
                if (searchType == null)
                {
                    MessageBox.Show("请选择搜索类型");
                }
                else
                {
                    PythonParse pps = new PythonParse();
                    Dictionary<string, Dictionary<int, List<int>>> monsterParse = pps.searchParse(input, searchType);
                    if(monsterParse != null)
                    {
                        DrawComponent.canvas = MyDesignerCanvas;
                        List<GroupBox> newGroupBoxList = DrawComponent.addMyGrid(monsterParse);
                        AiBar.Children.Clear();
                        for (int i = 0; i < newGroupBoxList.Count; ++i)
                        {
                            AiBar.Children.Add(newGroupBoxList[i]);
                        } 
                    }
                }               
            }
        }
    }
}
