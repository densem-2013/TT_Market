using System;
using System.Collections;
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
using TT_Market.Core.Identity;

namespace TT_Market.Web.Models.HelpClasses
{
    public static class ImportData
    {
        private static readonly ApplicationDbContext Db = new ApplicationDbContext();

        public static int ParseAndInsert(string path)
        {
            string filename = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var firstOrDefault = Db.PriceDocuments.FirstOrDefault(pd => string.Equals(pd.FileName,filename));
            if (firstOrDefault != null)
            {
                PriceReadSetting priceReadSetting = firstOrDefault.PriceReadSetting;
                if (priceReadSetting != null)
                {
                    byte[] file = File.ReadAllBytes(path);
                    MemoryStream ms = new MemoryStream(file);
                    PriceDocument priceDocument = new PriceDocument
                    {
                        DownLoadDate = DateTime.UtcNow,
                        FileName = filename
                    };
                    //XSSFWorkbook wb = new XSSFWorkbook(ms);
                    //DataSet data = GetExcelDataAsDataSet(path, false);

                    JObject jobj = JObject.Parse(priceReadSetting.TransformMask);
                    //

                    //IEnumerable<DataRow> data = GetData(path, sheetName, false);
                    //ReadDataFromDataRows(jobj, data);
                }
            }
            return 0;
        }

        public static Dictionary<string, List<ReadCellSetting>> AddRecordsToDict(JObject jobj)
        {
            Dictionary<string, List<ReadCellSetting>> dict = new Dictionary<string, List<ReadCellSetting>>();

            int sheetsCount = jobj.SelectToken("Sheets").Select(s => s.SelectToken("Sheet")).Count();
            if (sheetsCount != 0)
            {
                if (sheetsCount > 1)
                {

                }
                else
                {
                    string sheetName = jobj.SelectToken("Sheets.Sheet.@Name").ToString();
                }
            }
            return dict;
        }
        public static int ReadDataFromDataRows(JObject jobj, IEnumerable<DataRow> datarows)
        {
            int[] titleRowsNumbers =
                jobj.SelectToken("Sheets.Sheet.TitleRows.Row").Select(r => (int)r.SelectToken("@RowNumber").ToObject(typeof(int))).ToArray();
            for (int i = 0; i < titleRowsNumbers.Length; i++)
            {
                var rowQuery = jobj.SelectToken("Sheets.Sheet.TitleRows.Row").Where(tr => (int)tr.SelectToken("@RowNumber").ToObject(typeof(int)) == titleRowsNumbers[i]);
               
            }
            return 0;
        }

        /// <summary>[?(@.@RowNumber=5)]
        /// Добавляет указанное значение как свойство сущности EntityFramework. Эта сущность
        /// добавляется к соответствующему свойству контекста базы дфнных
        /// </summary>
        /// <param name="entity">Сущность в базе данных, представляющая одноимённую таблицу</param>
        /// <param name="property">Свойство сущности  для изменения значения</param>
        /// <param name="value">Сохраняемое значение</param>
        /// <returns></returns>
        public static int AddDataToDb(string entity, string property, string value)
        {
            Assembly assembly = Assembly.LoadFrom("TT_Market.Core.dll");
            var type = assembly.GetType("TT_Market.Core.Domains." + entity);
            Db.Set(type).Load();
            var collection = Db.Set(type).Local;

            var newEntity = Activator.CreateInstance(type);
            type.InvokeMember(property, BindingFlags.SetProperty, Type.DefaultBinder, newEntity,
                new Object[] {value});
            collection.Add(newEntity);
            Db.SaveChanges();

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