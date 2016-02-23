using System;
using NPOI.HSSF.UserModel;

namespace TT_Market.Core.HelpClasses
{
    public class ExcelData
    {

        private string _path;

        private readonly string xmlFilename = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
                                              @"TT_Market.Core\DBinitial\ReadSettings.xml";

        private readonly string xmlsearchwords = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
                                                 @"TT_Market.Core\DBinitial\SearchWords.xml";

        public ExcelData(string path)
        {
            _path = path;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public void GetCellMergedAttributes(HSSFCell cell)
        {
            //if (cell.)
            //{

            //}
        }
    }
}
