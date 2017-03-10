using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesigner
{
    class output
    {
        public output(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}


