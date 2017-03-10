using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner
{
    public class MyButton:Button
    {
        public string AID { get; set; }
        public DesignerCanvas canvas;

        protected override void OnClick()
        {
            base.OnClick();
            canvas.NewCanvas();
            canvas.Print_Executed(AID);
            DrawComponent.ReWriteDesignerItemValue(canvas.monster, DrawComponent.monsterParam);
        }
    }
}
