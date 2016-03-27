using System;
using System.Collections.Generic;

namespace TT_Market.Core.HelpClasses
{
    public class Target
    {
        public string Entity { get; set; }
        public string TypeProperty { get; set; }
        public List<string> Groups { get; set; }
        public string Value { get; set; }
        public string Mask { get; set; }
    }
    public class ReadCellSetting
    {
        public int CellNumber { get; set; }
        public List<Target> Targets { get; set; }
    }

    public class StartRow
    {
        public int? StartReadRow { get; set; }
        public int? RewievColumn { get; set; }
        public string TitleRowPattern { get; set; }
    }

    public class EndRow
    {
        public int? RewievColumn { get; set; }
        public string StopReadPattern { get; set; }
    }
    public class ReadSheetSetting
    {
        public string  SheetName { get; set; }
        public StartRow StartRow { get; set; }
        public EndRow EndRow { get; set; }
        public List<ReadCellSetting> ReadCellSettings { get; set; }
    }

    public class ReadData
    {
        public Target Target { get; set; }
        public dynamic Value { get; set; }
        public Type Type { get; set; }
    }
}