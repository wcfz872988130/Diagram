using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DiagramDesigner
{
    class DecideColor
    {
        public SolidColorBrush typeColor(String type)
        {
            SolidColorBrush backGroundColor = null;
            switch (type)
            {
                case "Any":
                    backGroundColor = new SolidColorBrush(Colors.LightSeaGreen);
                    break;
                case "Boolean":
                    backGroundColor = new SolidColorBrush(Colors.LightSkyBlue);
                    break;
                case "Integer":
                    backGroundColor = new SolidColorBrush(Colors.PaleVioletRed);
                    break;
                case "OrderInteger":
                    backGroundColor = new SolidColorBrush(Colors.PaleVioletRed);
                    break;
                case "InputInteger":
                    backGroundColor = new SolidColorBrush(Colors.Firebrick);
                    break;
                case "Float":
                    backGroundColor = new SolidColorBrush(Colors.WhiteSmoke);
                    break;
                case "Vec3":
                    backGroundColor = new SolidColorBrush(Colors.LightYellow);
                    break;
                case "String":
                    backGroundColor = new SolidColorBrush(Colors.YellowGreen);
                    break;
                case "SingleLinker":
                    backGroundColor = new SolidColorBrush(Colors.CornflowerBlue);
                    break;
                case "MultiLinker":
                    backGroundColor = new SolidColorBrush(Colors.Cornsilk);
                    break;
                case "Trigger":
                    backGroundColor = new SolidColorBrush(Colors.LightBlue);
                    break;
                case "Condition":
                    backGroundColor = new SolidColorBrush(Colors.LightCoral);
                    break;
                case "ConditionAndExp":
                    backGroundColor = new SolidColorBrush(Colors.LightCoral);
                    break;
                case "ConditionOrExp":
                    backGroundColor = new SolidColorBrush(Colors.LightCoral);
                    break;
                case "ConditionExpResult":
                    backGroundColor = new SolidColorBrush(Colors.LightCoral);
                    break;
                case "Action":
                    backGroundColor = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "Target":
                    backGroundColor = new SolidColorBrush(Colors.LightSteelBlue);
                    break;
            }
            return backGroundColor;
        }
    }
}
