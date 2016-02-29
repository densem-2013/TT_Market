using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Excel;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.Formula;
using NPOI.XSSF.UserModel;
using TT_Market.Core.Domains;

namespace TT_Market.Web.Models.HelpClasses
{
    public static class ImportData
    {
        private static readonly ApplicationDbContext _db = new ApplicationDbContext();

        public static int ParseAndInsert(string path)
        {
            string filename = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            PriceReadSetting pricelist = _db.PriceReadSettings.FirstOrDefault(pl => string.Equals(pl.FileName,filename ));
            if (pricelist!=null)
            {
                    byte[] file = File.ReadAllBytes(path);
                    MemoryStream ms = new MemoryStream(file);
                    Price price = new Price
                    {
                        DownLoadDate = DateTime.UtcNow,
                    };
                    //XSSFWorkbook wb = new XSSFWorkbook(fs);
                    DataSet data = GetExcelDataAsDataSet(path, false);
                    JObject jobj = JObject.Parse(pricelist.TransformMask);
                    foreach (JProperty sheetname in jobj["ReadSettings.Sheets.Sheet"])
                    {
                        //XSSFSheet sh = (XSSFSheet)wb.GetSheet(sheetname);
                        string dsname = data.DataSetName;
                        string sh = (string)sheetname["@Name"];
                    }

            }
            return 0;
        }

        public static int AddDataToDb(string entity, string property, string value)
        {
            Assembly assembly = Assembly.LoadFrom("TT_Market.Core.dll");
            var type = assembly.GetType("TT_Market.Core.Domains." + entity);
            _db.Set(type).Load();
            var collection = _db.Set(type).Local;

            var newEntity = Activator.CreateInstance(type);
            type.InvokeMember(property, BindingFlags.SetProperty, Type.DefaultBinder, newEntity,
                new Object[] {value});
            collection.Add(newEntity);
            _db.SaveChanges();

            return 0;
        }
        private static IExcelDataReader GetExcelDataReader(string path, bool isFirstRowAsColumnNames)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader dataReader;

                if (path.EndsWith(".xls"))
                    dataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                else if (path.EndsWith(".xlsx"))
                    dataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                else
                    throw new FileLoadException("The file to be processed is not an Excel file");

                dataReader.IsFirstRowAsColumnNames = isFirstRowAsColumnNames;
                return dataReader;
            }
        }

        private static DataSet GetExcelDataAsDataSet(string path, bool isFirstRowAsColumnNames)
        {
            return GetExcelDataReader(path, isFirstRowAsColumnNames).AsDataSet();
        }

        private static DataTable GetExcelWorkSheet(string path, string workSheetName, bool isFirstRowAsColumnNames)
        {
            DataTable workSheet = GetExcelDataAsDataSet(path, isFirstRowAsColumnNames).Tables[workSheetName];
            if (workSheet == null)
                throw new WorkbookNotFoundException(string.Format("The worksheet {0} does not exist, has an incorrect name, or does not have any data in the worksheet", workSheetName));
            return workSheet;
        }

        private static IEnumerable<DataRow> GetData(string path, string workSheetName, bool isFirstRowAsColumnNames = true)
        {
            return from DataRow row in GetExcelWorkSheet(path, workSheetName, isFirstRowAsColumnNames).Rows select row;
        }
    }
}