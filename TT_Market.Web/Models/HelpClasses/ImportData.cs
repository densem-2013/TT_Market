using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using Excel;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula;
using TT_Market.Core.Domains;
using TT_Market.Core.HelpClasses;
using TT_Market.Core.Identity;

namespace TT_Market.Web.Models.HelpClasses
{
    public static class ImportData
    {
        private static readonly ApplicationDbContext Db = new ApplicationDbContext();
        private static PriceLanguage PriceLanguage { get; set; }
        private static PriceDocument PriceDocument { get; set; }

        private static bool PrevDocumentExists { get; set; }

        public static void ParseAndInsert(string path)
        {

            string filename = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            PriceDocument =
                Db.PriceDocuments.Include("PriceReadSetting")
                    .FirstOrDefault(pd => string.Equals(pd.FileName, filename));
            PrevDocumentExists = false;
            if (PriceDocument != null)
            {
                PrevDocumentExists = true;
                PriceLanguage = PriceDocument.PriceLanguage;
                PriceReadSetting priceReadSetting = PriceDocument.PriceReadSetting;
                if (priceReadSetting != null)
                {
                    //byte[] file = File.ReadAllBytes(path);
                    //MemoryStream ms = new MemoryStream(file);

                    PriceDocument.DownLoadDate = DateTime.UtcNow;

                    JObject jobj = JObject.Parse(priceReadSetting.TransformMask);
                    Dictionary<string, ReadSheetSetting> docReadSettings = ReadSetting.GetReadSetting(Db, jobj);
                    foreach (var pair in docReadSettings)
                    {
                        ReadSheetSetting curSheetSetting = pair.Value;
                        if (curSheetSetting.StartRow.StartReadRow != null)
                        {
                            IEnumerable<DataRow> dataCollection =
                                GetData(path, pair.Key, false).Skip(curSheetSetting.StartRow.StartReadRow.Value - 1);

                            List<ReadCellSetting> tirePropos =
                                curSheetSetting.ReadCellSettings.Where(
                                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "TireProposition"))).ToList();

                            List<ReadCellSetting> tirePrices =
                                curSheetSetting.ReadCellSettings.Where(
                                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "TirePrice"))).ToList();

                            List<ReadCellSetting> stockValues =
                                curSheetSetting.ReadCellSettings.Where(
                                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Stock"))).ToList();

                            foreach (DataRow row in dataCollection)
                            {
                                TireProposition tireProposition = ReadTireProposition(tirePropos, row);

                                TirePrice tirePrice = ReadTirePrice(tirePrices, row);

                                tireProposition.TirePrices.Add(tirePrice);

                                foreach (ReadCellSetting rcs in stockValues)
                                {
                                    tireProposition.Stocks.Add(ReadStock(rcs,row));
                                }

                                PriceDocument.TirePropositions.Add(tireProposition);
                            }
                        }
                    }
                }
            }
        }

        public static Tire ReadTire(ReadCellSetting celsetting, DataRow row)
        {
            Tire tire = new Tire();
            if (PrevDocumentExists)
            {
                string tiretitle = row[celsetting.CellNumber].ToString().Trim();
                Tire prevtire =
                    Db.Tires.Include("TireProposition.PriceDocument")
                        .FirstOrDefault(
                            t =>
                                string.Equals(t.TireProposition.PriceDocument.FileName, PriceDocument.FileName) &&
                                string.Equals(t.TireTitle, tiretitle));
                if (prevtire != null)
                {
                    tire.Model = prevtire.Model;
                    tire.ConvSign = prevtire.ConvSign;
                    tire.Diameter = prevtire.Diameter;
                    tire.PressIndex = prevtire.PressIndex;
                    tire.TireTitle = tiretitle;
                    tire.Height = prevtire.Height;
                    tire.SpeedIndex = prevtire.SpeedIndex;
                    tire.Width = prevtire.Width;
                }
                else
                {
                        
                }
            }
            return tire;
        }

        public static Stock ReadStock(ReadCellSetting celsetting, DataRow row)
        {
            Target stockTarget = celsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Stock"));
            int stokval;
            int.TryParse(row[celsetting.CellNumber - 1].ToString().Trim(), out stokval);
            Stock stock = new Stock
            {
                StockValue = stokval,
                City = ReadCity(celsetting, row)
            };
            return stock;
        }

        public static City ReadCity(ReadCellSetting celsetting, DataRow row)
        {
            Target cityTarget = celsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "CityTitle"));
            CityTitle cityTitle = Db.CityTitles.FirstOrDefault(st => st.PriceLanguage.Id == PriceLanguage.Id &&
                                                                     string.Equals(st.Title, cityTarget.Value));
            City city;
            if (cityTitle != null)
            {
                city = cityTitle.City;
            }
            else
            {
                city = new City();
                if (cityTarget != null)
                    city.CityTitles.Add(new CityTitle
                    {
                        Title = row[celsetting.CellNumber].ToString().Trim(),
                        PriceLanguage = PriceLanguage
                    });
            }
            return city;
        }

        public static TireProposition ReadTireProposition(List<ReadCellSetting> celsettings, DataRow row)
        {

            TireProposition tireProposition = new TireProposition();
            foreach (ReadCellSetting rcs in celsettings)
            {
                var orDefault =
                    rcs.Targets.FirstOrDefault(t => string.Equals(t.Entity, "TireProposition"));
                if (orDefault != null)
                {
                    string property = orDefault.TypeProperty;
                    switch (property)
                    {
                        case "TirePriceCode":
                            tireProposition.TirePriceCode = row[rcs.CellNumber].ToString().Trim();
                            break;
                        case "ExtendedData":
                            tireProposition.ExtendedData = row[rcs.CellNumber].ToString().Trim();
                            break;
                        case "RegionCount":
                            tireProposition.RegionCount = int.Parse(row[rcs.CellNumber].ToString().Trim());
                            break;
                        case "PartnersCount":
                            tireProposition.PartnersCount = int.Parse(row[rcs.CellNumber].ToString().Trim());
                            break;
                        case "WaitingCount":
                            tireProposition.WaitingCount = int.Parse(row[rcs.CellNumber].ToString().Trim());
                            break;
                        case "ReservCount":
                            tireProposition.ReservCount = int.Parse(row[rcs.CellNumber].ToString().Trim());
                            break;
                    }
                }
            }
            return tireProposition;
        }

        public static TirePrice ReadTirePrice(List<ReadCellSetting> celsettings, DataRow row)
        {
            TirePrice tirePrice = new TirePrice();
            foreach (ReadCellSetting price in celsettings)
            {
                var firstOrDefault = price.Targets.FirstOrDefault(t => string.Equals(t.Entity, "TirePrice"));
                if (firstOrDefault != null)
                {
                    string property =
                        firstOrDefault.TypeProperty;
                    switch (property)
                    {
                        case "RegularPrice":
                            tirePrice.RegularPrice =
                                double.Parse(row[price.CellNumber].ToString().Trim());
                            break;
                        case "DiscountPrice":
                            tirePrice.DiscountPrice =
                                double.Parse(row[price.CellNumber].ToString().Trim());
                            break;
                        case "SpecialPrice":
                            tirePrice.SpecialPrice =
                                double.Parse(row[price.CellNumber].ToString().Trim());
                            break;
                    }
                }
                var curValue = price.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Currency"));
                tirePrice.Currency =
                        Db.Currencys.FirstOrDefault(
                            c =>
                                (curValue != null)
                                    ? string.Equals(c.CurrencyTitle, curValue.Value)
                                    : c.IsDefault);

            }
            return tirePrice;
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