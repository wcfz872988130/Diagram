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
    public class ListBoxEx : ListBox
    {
        public enum ItemStyles
        {
            NormalStyle,
            CheckBoxStyle,
            RadioBoxStyle
        }

        public ItemStyles m_extendedStyle;
        public ItemStyles ExtendedStyle
        {
            get { return m_extendedStyle; }
            set
            {
                m_extendedStyle = value;
                ResourceDictionary resDict = new ResourceDictionary();
                resDict.Source = new Uri("pack://application:,,,/DiagramDesigner;component/Resources/Styles/ListBoxEx.xaml");
                if (resDict.Source == null)
                    throw new SystemException();

                switch (value)
                {
                    case ItemStyles.NormalStyle:
                        this.Style = (Style)resDict["NormalListBox"];
                        break;
                    case ItemStyles.CheckBoxStyle:
                        this.Style = (Style)resDict["CheckListBox"];
                        break;
                    case ItemStyles.RadioBoxStyle:
                        this.Style = (Style)resDict["RadioListBox"];
                        break;
                }
            }
        }

        static ListBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxEx), new FrameworkPropertyMetadata(typeof(ListBoxEx)));
        }

        public ListBoxEx(ItemStyles style)
            : base()
        {
            ExtendedStyle = style;
        }

        public ListBoxEx()
            : base()
        {
            ExtendedStyle = ItemStyles.NormalStyle;
        }
    }
}
