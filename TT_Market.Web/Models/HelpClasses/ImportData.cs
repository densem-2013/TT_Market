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
using TT_Market.Core.HelpClasses;
using TT_Market.Core.Identity;

namespace TT_Market.Web.Models.HelpClasses
{
    public static class ImportData
    {
        private static readonly ApplicationDbContext Db = new ApplicationDbContext();

        public static void ParseAndInsert(string path)
        {

            string filename = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var priceDocument =
                Db.PriceDocuments.Include("PriceReadSetting")
                    .FirstOrDefault(pd => string.Equals(pd.FileName, filename));
            if (priceDocument != null)
            {
                PriceReadSetting priceReadSetting = priceDocument.PriceReadSetting;
                if (priceReadSetting != null)
                {
                    //byte[] file = File.ReadAllBytes(path);
                    //MemoryStream ms = new MemoryStream(file);

                    priceDocument.DownLoadDate = DateTime.UtcNow;

                    JObject jobj = JObject.Parse(priceReadSetting.TransformMask);
                    Dictionary<string, ReadSheetSetting> docReadSettings = ReadSetting.GetReadSetting(Db, jobj);
                    foreach (var pair in docReadSettings)
                    {
                        ReadSheetSetting shSetting = pair.Value;
                        if (shSetting.StartRow.StartReadRow != null)
                        {
                            IEnumerable<DataRow> dataCollection =
                                GetData(path, pair.Key, false).Skip(shSetting.StartRow.StartReadRow.Value - 1);

                            List<ReadCellSetting> tirePropos =
                                shSetting.ReadCellSettings.Where(
                                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "TireProposition"))).ToList();
                            foreach (DataRow row in dataCollection)
                            {
                                TireProposition tireProposition = new TireProposition();
                                foreach (ReadCellSetting rcs in tirePropos)
                                {
                                    var orDefault =
                                        rcs.Targets.FirstOrDefault(t => string.Equals(t.Entity, "TireProposition"));
                                    if (orDefault != null)
                                    {
                                        string entity = orDefault.Entity;
                                        switch (entity)
                                        {
                                            case "PriceCode":
                                                tireProposition.PriceCode = row[rcs.CellNumber].ToString();
                                                break;
                                            case "ExtendedData":
                                                tireProposition.ExtendedData = row[rcs.CellNumber].ToString();
                                                break;
                                            case "RegularPrice":
                                                tireProposition.RegularPrice =
                                                    double.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                            case "DiscountPrice":
                                                tireProposition.DiscountPrice =
                                                    double.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                            case "SpecialPrice":
                                                tireProposition.SpecialPrice =
                                                    double.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                            case "RegionCount":
                                                tireProposition.RegionCount = int.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                            case "PartnersCount":
                                                tireProposition.PartnersCount = int.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                            case "WaitingCount":
                                                tireProposition.WaitingCount = int.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                            case "ReservCount":
                                                tireProposition.ReservCount = int.Parse(row[rcs.CellNumber].ToString());
                                                break;
                                        }
                                    }
                                }
                                priceDocument.TirePropositions.Add(tireProposition);
                            }
                        }
                    }
                }
            }
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

        public static bool IsEntityExists(Assembly assembly, string entity, string property, string value)
        {
            var type = assembly.GetType("TT_Market.Core.Domains." + entity);
            Db.Set(type).Load();
            var collection = Db.Set(type).Local;
            return false;
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