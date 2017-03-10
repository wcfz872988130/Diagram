using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Text;
using System.Xml;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiagramDesigner
{
    class XmlParse
    {
        Input[] input_array;
        output[] output_array;
        private string nodeName = null;
        private Dictionary<string, string[]> _input = new Dictionary<string, string[]>();
        private List<string[]> _output = new List<string[]>();
        private string name;
        private string nodeFileName;
        public static Dictionary<string, List<string>> paramType = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> paramDescribe = new Dictionary<string, List<string>>();
        public XmlParse()
        { 
        }
        public XmlParse(string Name, string nodeFileName)
        {
            name = Name;
            this.nodeFileName = nodeFileName;
        }
        private void parseinputnode(XmlNode node)
        {
            if (node.ChildNodes.Count > 0)
            {
                foreach (XmlNode childnode in node.ChildNodes)
                {
                    string[] typename = new string[2];
                    typename[1] = childnode.InnerText;
                    typename[0] = childnode.Attributes["type"].Value.ToString();
                    string name = childnode.Name;
                    _input.Add(name, typename);
                }
            }
        }

        private void parseoutputnode(XmlNode node)
        {
            if (node.ChildNodes.Count > 0)
            {
                foreach (XmlNode childnode in node.ChildNodes)
                {
                    string[] nametype = new string[3];
                    nametype[1] = childnode.Attributes["type"].Value.ToString();
                    nametype[0] = childnode.Name;
                    nametype[2] = childnode.InnerText;
                    _output.Add(nametype);
                }
            }
        }

        private void addValue()
        {
            int length = _input.Count;
            input_array = new Input[length];
            int index = 0;
            foreach (KeyValuePair<string, string[]> kv in _input)
            {
                string name = kv.Key;
                string type = kv.Value[0];
                string value = kv.Value[1];
                input_array[index++] = new Input(name, type, value);
            }

            length = _output.Count;
            output_array = new output[length];
            index = 0;
            foreach (string[] value in _output)
            {
                string name = value[0];
                string type = value[1];
                string describe = value[2];
                output_array[index++] = new output(name, type, describe);
            }
        }

        public Input[] input
        {
            get
            {
                return input_array;
            }
        }

        public output[] output
        {
            get
            {
                return output_array;
            }
        }

        public void parseXml()
        {
            string xmlFilePath = @"../../Xml/" + nodeFileName + "/" + name + ".xml";
            // string path = Path.Combine(Application.StartupPath, "soso.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlNodeList nodeList = doc.SelectNodes("/node");
            if (nodeList != null)
            {
                foreach (XmlNode xmlnode in nodeList)
                {
                    XmlNodeList childnodeList = xmlnode.ChildNodes;
                    if (childnodeList.Count > 0)
                    {
                        XmlNode titlenode = xmlnode.SelectSingleNode("title");
                        nodeName = titlenode.InnerText;
                        XmlNode inputnode = xmlnode.SelectSingleNode("input");
                        if (inputnode != null)
                        {
                            parseinputnode(inputnode);
                        }
                        XmlNode outputnode = xmlnode.SelectSingleNode("output");
                        if (outputnode != null)
                        {
                            parseoutputnode(outputnode);
                        }
                    }
                }
            }
            addValue();
        }

        public static List<Dictionary<string, List<string>>> parseTypeXml()
        {
            string xmlFilePath = @"../../Xml/" + "ParamTypeList.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            List<Dictionary<string, List<string>>> parseData = new List<Dictionary<string, List<string>>>();
            XmlNodeList nodeList = doc.SelectNodes("/Type");
            XmlNode TypeNode = nodeList[0];
            XmlNodeList TypeNodeList = TypeNode.ChildNodes;
            if (nodeList != null)
            {
                foreach (XmlNode xmlnode in TypeNodeList)
                {
                    XmlNodeList childnodeList = xmlnode.ChildNodes;
                    List<string> paramList = new List<string>();
                    List<string> paramDescribeList = new List<string>();
                    for (int i = 0; i < childnodeList.Count; ++i)
                    {
                        paramDescribeList.Add(childnodeList[i].InnerText);
                        paramList.Add(childnodeList[i].Attributes["type"].Value.ToString());
                    }
                    paramType[xmlnode.Name] = paramList;
                    paramDescribe[xmlnode.Name] = paramDescribeList;
                }
            }
            parseData.Add(paramType);
            parseData.Add(paramDescribe);
            //List<string> priorityList = new List<string>();
            //priorityList.Add("Integer");
            //List<string> cdTimeList = new List<string>();
            //cdTimeList.Add("Float");
            //paramType["priority"] = priorityList;
            //paramType["aiCD"] = cdTimeList;
            return parseData;
        }
    }
}
