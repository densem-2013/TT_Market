using System;
using System.Collections.Generic;

namespace TT_Market.Web.Models.HelpClasses
{
    public class Target
    {
        public string DbType { get; set; }
        public string TypeProperty { get; set; }
        public List<int> GroupNumbers { get; set; }
        public Type ValueType { get; set; }
    }
    public class ReadCellSetting
    {
        public int CellNumber { get; set; }
        public List<Target> Targets { get; set; }
        public string Mask { get; set; }
        public string Norecognized { get; set; }
        public Dictionary<string, string> AlterValue { get; set; } 
    }
}