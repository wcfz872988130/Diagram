using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.IO;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using IronPython;
using IronPython.Hosting;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace DiagramDesigner
{
    class PythonParse
    {
#region trigger variable
        public string triggerName;
        public List<string> triggerParamList = new List<string>();
        public Dictionary<string, string> AIInitParams = new Dictionary<string,string>();
#endregion
        Dictionary<string, string> monsterParam = new Dictionary<string, string>();
        Dictionary<int, string> conditionDictionary = new Dictionary<int, string>();
        Dictionary<int, List<string>> conditionParamDictionary = new Dictionary<int, List<string>>();
        Dictionary<int, string> actionDictionary = new Dictionary<int, string>();
        Dictionary<int, List<string>> actionParamDictionary = new Dictionary<int, List<string>>();
        public Dictionary<int, string> targetDictionary = new Dictionary<int, string>();
        public Dictionary<int, List<string>> targetParamDictionary = new Dictionary<int, List<string>>();
        public Dictionary<int, int> conditionTargetLinker = new Dictionary<int, int>();
        public Dictionary<int, int> actionTargetLinker = new Dictionary<int, int>();
        Dictionary<int, string> andExp = new Dictionary<int, string>();
        Dictionary<int, string> orExp = new Dictionary<int, string>();
        List<string> conditionExp = new List<string>();
        IronPython.Runtime.PythonDictionary aiAction;
        public static string codePath;

        public PythonParse()
        {
        }

        public void ReDrawDiagram(string index)
        {
            int id = int.Parse(index);
            AIInitParams["id"] = index;
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile("Code\\monster_ai_data.py");
            IronPython.Runtime.PythonDictionary data = test.data;
            aiAction = data[id] as IronPython.Runtime.PythonDictionary;
            init();
            IronPython.Runtime.List Trigger = aiAction["trigger"] as IronPython.Runtime.List;
            triggerName = Convert.ToString(Trigger[0]);
            for (int i = 1; i < Trigger.Count; ++i)
            {
                triggerParamList.Add(Convert.ToString(Trigger[i]));
            }
            foreach (var key in aiAction.Keys)
            {
                SelectDictionary(Convert.ToString(key));
            }
        }
        private void init()
        {
            string[] aiParamName = new string[] { "aiCD", "priority", "times", "enterCombatDelay", "cdSharedAIs", "intLevel", "intCD", "initOffset", "repeatOffset" };
            initParam(aiParamName);
        }

        private void initParam(string[] paramName)
        {
            for (int i = 0; i < paramName.Length; ++i)
            {
                if (aiAction.ContainsKey(paramName[i]))
                {
                    string param = Convert.ToString(aiAction[paramName[i]]);
                    AIInitParams[paramName[i]] = param;
                }
            }
        }

        private void SelectDictionary(string type)
        {
            Regex regex0 = new Regex(@"^condition\d$");
            Regex regex1 = new Regex(@"^condition\dArgs$");
            Regex regex2 = new Regex(@"^condition\dTgt\d$");
            Regex regex3 = new Regex(@"^action\d$");
            Regex regex4 = new Regex(@"^action\dArgs$");
            Regex regex5 = new Regex(@"^action\dTgt\d$");
            Regex regex6 = new Regex(@"^tgtMethodName\d$");
            Regex regex7 = new Regex(@"^tgtMethodArgs\d$");
            if(regex0.IsMatch(type))
            {
                addStringDictionary(type, 9, conditionDictionary);
            }
            else if(regex1.IsMatch(type))
            {
                addListDictionary(type, 9, conditionParamDictionary);
            }
            else if(regex2.IsMatch(type))
            {
                addLinkerDictionary(type, 9, 13, conditionTargetLinker);
            }
            else if(regex3.IsMatch(type))
            {
                addStringDictionary(type, 6, actionDictionary);
            }
            else if(regex4.IsMatch(type))
            {
                addListDictionary(type, 6, actionParamDictionary);
            }
            else if(regex5.IsMatch(type))
            {
                addLinkerDictionary(type, 6, 10, actionTargetLinker);
            }
            else if(regex6.IsMatch(type))
            {
                addStringDictionary(type, 13, targetDictionary);
            }
            else if(regex7.IsMatch(type))
            {
                addListDictionary(type, 13, targetParamDictionary);
            }
            else if(type.Equals("conditionExp"))
            {
                int gap = 0;
                if (aiAction[type] is IronPython.Runtime.PythonTuple)
                {
                    IronPython.Runtime.PythonTuple conditionExp = aiAction[type] as IronPython.Runtime.PythonTuple;
                    for (int i = 0; i < conditionExp.Count; ++i)
                    {
                        if (conditionExp[i] is IronPython.Runtime.List)
                        {
                            Dictionary<int, string> tempList = new Dictionary<int, string>();
                            IronPython.Runtime.List conditionExpList = conditionExp[i] as IronPython.Runtime.List;
                            for (int j = 0; j < conditionExpList.Count; ++j)
                            {
                                orExp[i + j] = Convert.ToString(conditionExpList[j]);
                                gap++;
                            }
                        }
                        else
                        {
                            gap = gap == 0 ? 0 : (gap - 1);
                            andExp[i+gap] = Convert.ToString(conditionExp[i]);
                        }
                    }
                }
                else 
                {
                    string exp = Convert.ToString(aiAction[type]);
                    andExp[0] = exp;
                }
            }
        }

        private void addStringDictionary(string type, int startPosition, Dictionary<int, string> otherDictionary)
        {
            int index = int.Parse(type.Substring(startPosition, 1));
            string tempName = Convert.ToString(aiAction[type]);
            otherDictionary[index] = tempName;
        }

        private void addListDictionary(string type, int startPosition, Dictionary<int, List<string>> otherParamDictionary)
        {
            int index = int.Parse(type.Substring(startPosition, 1));
            IronPython.Runtime.List templateArgsList = aiAction[type] as IronPython.Runtime.List;
            List<string> templateArgsTempList = new List<string>();
            for (int i = 0; i < templateArgsList.Count; ++i)
            {
                templateArgsTempList.Add(templateArgsList[i].ToString());
            }
            otherParamDictionary[index] = templateArgsTempList;
        }

        private void addLinkerDictionary(string type, int startPosition, int endPosition, Dictionary<int, int> otherLinkerDictionary)
        {
            int index = int.Parse(type.Substring(startPosition, 1));
            //int target = int.Parse(type.Substring(endPosition, 1));
            int targetIndex = Convert.ToInt32(aiAction[type]);
            //List<int> tempLinkerList = new List<int>();
            //tempLinkerList.Add(index);
            //tempLinkerList.Add(target);
            otherLinkerDictionary[index] = targetIndex;
        }

        public DesignerItem DrawTriggerOnCanvas()
        {
            DesignerItem item = DrawComponent.DrawTriggerComponent(triggerName,triggerParamList);
            return item;        
        }
        public List<DesignerItem> DrawConditionTriggersOnCanvas()
        {
            List<DesignerItem> itemList = DrawComponent.addNode("Condition", conditionDictionary, conditionParamDictionary, conditionTargetLinker);
            return itemList;
        }

        public List<DesignerItem> DrawActionTriggersOnCanvas()
        {
            List<DesignerItem> itemList = DrawComponent.addNode("Action", actionDictionary, actionParamDictionary, actionTargetLinker);
            return itemList;
        }

        public List<DesignerItem> DrawTargetTriggersOnCanvas()
        {
            List<DesignerItem> itemList = DrawComponent.addNode("Target", targetDictionary, targetParamDictionary);
            return itemList;
        }

        public DesignerItem DrawAndorOrExpOnCanvas(string protoName, string typeName, string expType)
        {
            DrawComponent.expComboBox = expType.Equals("and") ? andExp : orExp;
            if(DrawComponent.expComboBox.Count > 0)
            {
                Dictionary<int, string> protoType = new Dictionary<int, string>();
                Dictionary<int, List<string>> param = new Dictionary<int, List<string>>();
                protoType[0] = protoName;
                List<string> paramList1 = new List<string>();
                paramList1.Add(DrawComponent.expComboBox.Count.ToString());
                param[0] = paramList1;
                List<DesignerItem> itemList = DrawComponent.addNode(typeName, protoType, param);
                return itemList[0];
            }
            return null;
        }
        public DesignerItem DrawExpResultOnCanvas()
        {
            Dictionary<int, string> protoType = new Dictionary<int, string>();
            Dictionary<int, List<string>> param = new Dictionary<int, List<string>>();
            protoType[0] = "Exp";
            List<DesignerItem> itemList = DrawComponent.addNode("ConditionExpResult", protoType, param);
            return itemList[0];
        }

        public static void PythonStructure(int newID, List<DesignerItem> triggerList, List<DesignerItem> conditionList, List<DesignerItem> actionList, List<DesignerItem> targetList, Dictionary<int, string> andOrder, Dictionary<int, string> orOrder)
        {
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile("Code\\monster_ai_data.py");
            IronPython.Runtime.PythonDictionary data = test.data;
            IronPython.Runtime.PythonDictionary unit = new IronPython.Runtime.PythonDictionary();

            /////trigger/////
            IronPython.Runtime.List Trigger = new IronPython.Runtime.List();
            Trigger.Add(triggerList[2].IDName);
            
            for (int i = 0; i < triggerList[2].outputValue.Count; ++i)
            {
                MyTextBox tempTextBox = triggerList[2].outputValue[i];
                Trigger.Add(tempTextBox.propertyValue);
            }
            unit["trigger"] = Trigger;
            /////trigger/////

            ////AIParam////
            MyTextBox idTextBox = triggerList[1].outputValue[0] as MyTextBox;
            int aiID = Convert.ToInt32(idTextBox.propertyValue);
            Monster_Data(triggerList[0], aiID);
            for (int i = 1; i < triggerList[1].outputValue.Count; ++i)
            {
                MyTextBox tempTextBox = triggerList[1].outputValue[i];
                string convertString = triggerList[1].outputValue[i].propertyValue;
                //////////////////////////handle temporarily///////////////////////////////
                if (Regex.IsMatch(convertString, @"^\d+$") || Regex.IsMatch(convertString, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$"))
                {
                    unit[tempTextBox.Property] = convertString;
                }
            }
            ////AIParam////

            ////condition////
            for (int i = 0; i < conditionList.Count; ++i)
            {
                unit["condition" + i] = conditionList[i].IDName;
                IronPython.Runtime.List conditionParam = new IronPython.Runtime.List();
                for (int j = 0; j < conditionList[i].outputValue.Count; ++j)
                {
                    MyTextBox tempTextBox = conditionList[i].outputValue[j];
                    conditionParam.Add(tempTextBox.propertyValue);
                }
                if(conditionParam.Count > 0)
                    unit["condition" + i + "Args"] = conditionParam;
                ComboBox tempCombo = conditionList[i].myCombo;

                string selectedIndex = null;
                if(tempCombo.SelectedItem != null)
                    selectedIndex = ((ComboData)tempCombo.SelectedItem).Value;
                if (selectedIndex != null)
                    unit["condition" + i + "Tgt0"] = selectedIndex;
            }
            ////condition////

            ////action////
            for (int i = 0; i < actionList.Count; ++i)
            {
                unit["action" + i] = actionList[i].IDName;
                IronPython.Runtime.List actionParam = new IronPython.Runtime.List();
                for (int j = 0; j < actionList[i].outputValue.Count; ++j)
                {
                    MyTextBox tempTextBox = actionList[i].outputValue[j];
                    actionParam.Add(tempTextBox.propertyValue);
                }
                if(actionParam.Count > 0)
                    unit["action" + i + "Args"] = actionParam;
                ComboBox tempCombo = actionList[i].myCombo;
                string selectedIndex = null;
                if (tempCombo.SelectedItem != null)
                    selectedIndex = ((ComboData)tempCombo.SelectedItem).Value;
                if (selectedIndex != null)
                    unit["action" + i + "Tgt0"] = selectedIndex;
            }
            ////action////

            ////exp////
            if (andOrder.Count <= 1 && orOrder.Count == 0)
            {
                unit["conditionExp"] = andOrder[1] == "True" ? true : false;
            }
            else 
            {
                IronPython.Runtime.List orList = new IronPython.Runtime.List();
                var dicSort = from objDic in orOrder orderby objDic.Key ascending select objDic;
                foreach (KeyValuePair<int, string> kvp in dicSort)
                {
                    bool judge = kvp.Value == "True" ? true : false;
                    orList.Add(judge);
                }

                IronPython.Runtime.List andList = new IronPython.Runtime.List();

                var newdicSort = from objDic in andOrder orderby objDic.Key ascending select objDic;
                int lastValue = 0;
                foreach (KeyValuePair<int, string> kvp in newdicSort)
                {
                    if ((kvp.Key - lastValue) > 1)
                    {
                        andList.Add(orList);
                        bool judge = kvp.Value == "True" ? true : false;
                        andList.Add(judge);
                        lastValue = kvp.Key;
                    }
                    else
                    {
                        bool judge = kvp.Value == "True" ? true : false;
                        andList.Add(judge);
                        lastValue = kvp.Key;
                    }
                }

                IronPython.Runtime.PythonTuple expTuple = new IronPython.Runtime.PythonTuple(andList);
                unit["conditionExp"] = expTuple;
            }

            ////exp////

            ////target////
            if(targetList != null)
            {
                for (int i = 0; i < targetList.Count; ++i)
                {
                    unit["tgtMethodName" + i] = targetList[i].IDName;
                    IronPython.Runtime.List targetParam = new IronPython.Runtime.List();
                    for (int j = 0; j < targetList[i].outputValue.Count; ++j)
                    {
                        MyTextBox tempTextBox = targetList[i].outputValue[j];
                        targetParam.Add(tempTextBox.propertyValue);
                    }
                    if (targetParam.Count > 0)
                        unit["tgtMethodArgs" + i] = targetParam;
                }
            }
            ////target////

            data[newID] = unit;
            exportCode(data, "monster_ai_data.py");
        }

        private static void Monster_Data(DesignerItem monster, int aiID)
        {
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile("Code\\monster_data.py");
            IronPython.Runtime.PythonDictionary data = test.data;
            IronPython.Runtime.PythonDictionary unit = null;
            IronPython.Runtime.PythonTuple tuple = null;
            List<int> aiIDList = new List<int>();
            aiIDList.Add(aiID);
            int monsterID = -1;
            for (int i = 0; i < monster.outputValue.Count; ++i)
            {
                MyTextBox tempTextBox = monster.outputValue[i];
                if (tempTextBox.Property.Equals("ID"))
                {
                    try { monsterID = Convert.ToInt32(tempTextBox.propertyValue); }
                    catch (Exception e)
                    {
                        var ex = new Exception("缺少怪物ID");
                        throw ex;
                    }
                }
            }
            if (data.ContainsKey(monsterID))
            {
                unit = data[monsterID] as IronPython.Runtime.PythonDictionary;
                if (unit["monsterAI"] != null)
                {
                    tuple = unit["monsterAI"] as IronPython.Runtime.PythonTuple;
                    for (int i = 0; i < tuple.Count; ++i)
                    {
                        int exitAIID = Convert.ToInt32(tuple[i]);
                        aiIDList.Add(exitAIID);
                    }
                }
            }
            else 
            { 
                unit = new IronPython.Runtime.PythonDictionary();
            }
            unit["monsterAI"] = new IronPython.Runtime.PythonTuple(aiIDList);
            for (int i = 0; i < monster.outputValue.Count; ++i)
            {
                MyTextBox tempTextBox = monster.outputValue[i];
                if(!tempTextBox.Property.Equals(tempTextBox.propertyValue) && tempTextBox.propertyValue != null && !tempTextBox.Property.Equals("ID"))
                {
                    unit[tempTextBox.Property] = tempTextBox.propertyValue;
                }
            }
            data[monsterID] = unit;
            exportCode(data, "monster_data.py");
        }

        public Dictionary<string, Dictionary<int, List<int>>> searchParse(string inputData, string searchType)
        {
            Dictionary<string, Dictionary<int, List<int>>> monster = null;
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile("Code\\monster_data.py");
            IronPython.Runtime.PythonDictionary data = test.data;
            switch(searchType)
            {
                case "Monster_ID":
                    List<int> aiDataID = new List<int>();
                    if (Regex.IsMatch(inputData, @"^\d+$"))
                    {
                        int key = int.Parse(inputData);
                        if (data.ContainsKey(key))
                        {
                            monsterParam["ID"] = key.ToString();
                            IronPython.Runtime.PythonDictionary aiDictionary = data[key] as IronPython.Runtime.PythonDictionary;
                            if(aiDictionary.ContainsKey("name"))
                            {
                                string name = Convert.ToString(aiDictionary["name"]);
                                monsterParam["name"] = name;
                            }
                            if (aiDictionary.ContainsKey("tag"))
                            {
                                string tag = Convert.ToString(aiDictionary["tag"]);
                                monsterParam["tag"] = tag;
                            }
                            if (aiDictionary.ContainsKey("monsterAI"))
                            {
                                DrawComponent.monsterParam = monsterParam;
                                IronPython.Runtime.PythonTuple monsterTuple = aiDictionary["monsterAI"] as IronPython.Runtime.PythonTuple;
                                for (int i = 0; i < monsterTuple.Count; ++i)
                                {
                                    int id = Convert.ToInt32(monsterTuple[i]);
                                    aiDataID.Add(id);
                                }
                                monster = searchAIByID(aiDataID);
                            }
                            else { MessageBox.Show("此怪物不包含AI"); }
                        }
                        else
                        {
                            MessageBox.Show("不包含此ID");
                        }
                    }
                    else { MessageBox.Show("输入的格式不正确"); }
                    
                    break;
                case "Monster_Name":
                    break;
                case "Monster_Tag":
                    break;
            }
            return monster;
        }

        private static Dictionary<string, Dictionary<int, List<int>>> searchAIByID(List<int> IDList)
        {
            Dictionary<string, Dictionary<int, List<int>>> assemble = new Dictionary<string, Dictionary<int, List<int>>>();
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile("Code\\monster_ai_data.py");
            IronPython.Runtime.PythonDictionary data = test.data;
            for (int i = 0; i < IDList.Count; ++i)
            {
                IronPython.Runtime.PythonDictionary aiAction = data[IDList[i]] as IronPython.Runtime.PythonDictionary;
                IronPython.Runtime.List Trigger = aiAction["trigger"] as IronPython.Runtime.List;
                string aiName = Convert.ToString(Trigger[0]);
                if (assemble.ContainsKey(aiName))
                {
                    if (!aiAction.ContainsKey("priority"))
                    {
                        if (assemble[aiName].ContainsKey(0))
                        {
                            assemble[aiName][0].Add(IDList[i]);
                        }
                        else
                        {
                            List<int> newIDList = new List<int>();
                            newIDList.Add(IDList[i]);
                            assemble[aiName][0] = newIDList;
                        }
                    }
                    else 
                    {
                        int aiPriority = Convert.ToInt32(aiAction["priority"]);
                        if (assemble[aiName].ContainsKey(aiPriority))
                        {
                            assemble[aiName][aiPriority].Add(IDList[i]);
                        }
                        else
                        {
                            List<int> newIDList = new List<int>();
                            newIDList.Add(IDList[i]);
                            assemble[aiName][aiPriority] = newIDList;
                        }
                    }
                }
                else
                {
                    Dictionary<int, List<int>> tempAssemble = new Dictionary<int, List<int>>();
                    if (!aiAction.ContainsKey("priority"))
                    {
                        List<int> tempList = new List<int>();
                        tempList.Add(IDList[i]);
                        tempAssemble[0] = tempList;
                        assemble[aiName] = tempAssemble;
                    }
                    else
                    {
                        int aiPriority = Convert.ToInt32(aiAction["priority"]);
                        List<int> tempList = new List<int>();
                        tempList.Add(IDList[i]);
                        tempAssemble[aiPriority] = tempList;
                        assemble[aiName] = tempAssemble;
                    }
                }
            }
            return assemble;
        }

        public static void exportCode(IronPython.Runtime.PythonDictionary data, string path)
        {
            string Path = codePath + "\\" + path;
            FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string startString = "data = {";
            sw.WriteLine(startString);
            foreach (var key in data.Keys)
            {
                string phraseOne = "\t" + Convert.ToString(key) + " : " + "{";
                string phraseValue = null;
                IronPython.Runtime.PythonDictionary nodeValue = data[key] as IronPython.Runtime.PythonDictionary;
                foreach(var kvp in nodeValue.Keys)
                {
                    if(nodeValue[kvp] == null || nodeValue[kvp].Equals(""))
                    {
                        continue;
                    }
                    string phraseTwo = "'" + Convert.ToString(kvp) + "' : ";
                    string phraseThree = null;
                    if(nodeValue[kvp] is IronPython.Runtime.List)
                    {
                        IronPython.Runtime.List paramList = nodeValue[kvp] as IronPython.Runtime.List;
                        phraseThree = "[";
                        for (int j = 0; j < paramList.Count; ++j)
                        {
                            string convertString = Convert.ToString(paramList[j]);
                            if(Regex.IsMatch(convertString, @"^(\-|\+)?\d+(\.\d+)?$"))
                            {
                                phraseThree += convertString;
                            }
                            else
                            {
                                phraseThree += "'" + convertString + "'";
                            }                            
                            if (j != paramList.Count - 1)
                                phraseThree += ",";
                            else
                                phraseThree += "], ";
                        }
                    }
                    else 
                    {
                        string convertString = Convert.ToString(nodeValue[kvp]);
                        if (Regex.IsMatch(convertString, @"^\d+$") || nodeValue[kvp] is IronPython.Runtime.PythonTuple)
                        {
                            phraseThree = Convert.ToString(nodeValue[kvp]) + ", ";
                        }
                        else
                            phraseThree = "'" + Convert.ToString(nodeValue[kvp]) + "'" + ", ";
                    }
                    phraseValue += (phraseTwo + phraseThree);
                }
                phraseValue = phraseOne + phraseValue + "},";
                sw.WriteLine(phraseValue);
            }
            sw.WriteLine("}");
            sw.Close();
            fs.Close();
        }
    }
}
