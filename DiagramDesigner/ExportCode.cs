using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using IronPython;
using IronPython.Hosting;

namespace DiagramDesigner
{
    class ExportCode
    {
        public static string codePath;
        public static void triggerParse(List<DesignerItem> triggerList)
        {
            try 
            {
                codePath += "\\" + "monster_ai_data.py";
                FileStream fs = new FileStream(codePath, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

                for (int i = 0; i < triggerList.Count; ++i)
                {
                    string triggerString = "\'trigger\':[" + "\'" + triggerList[i].IDName + "\'], ";
                    sw.Write(triggerString);

                    for (int j = 0; j < triggerList[i].outputValue.Count; ++j)
                    {
                        MyTextBox tempTextBox = triggerList[i].outputValue[j];
                        string triggerOutputValueString = "'" + tempTextBox.Property + "'" + ":" + tempTextBox.propertyValue + ", ";
                        sw.Write(triggerOutputValueString);
                    }
                }
                sw.Close();
                fs.Close();
                MessageBox.Show("保存成功");
            }
            catch(Exception e)
            {
                MessageBox.Show("保存失败:" + e.Message);
            }
        }

        public static void PythonStructure()
        {
            IronPython.Runtime.PythonDictionary data = new IronPython.Runtime.PythonDictionary();
        }
    }
}
