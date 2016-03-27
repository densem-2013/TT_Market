using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private static ReadSheetSetting curSheetSetting { get; set; }
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
                        curSheetSetting = pair.Value;
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
                    tire.Country = prevtire.Country;
                }
                else
                {
                    tire.TireTitle = tiretitle;
                    tire.Country = GetCountry(row);

                }
            }
            return tire;
        }

        public static Model GetModel(DataRow row)
        {
            ReadCellSetting modelsetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Model")));
            Model model = null;
            if (modelsetting != null)
            {
                string mdstring = row[modelsetting.CellNumber - 1].ToString().Trim();
                Target modelTarget = modelsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Model"));
                if (modelTarget != null)
                {
                    string modvalue = "";
                    string mdmask = modelTarget.Mask;
                    if (mdmask != null)
                    {
                        Regex reg = new Regex(mdmask);
                        modelTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(mdstring).Groups[x].Value;
                            if (val != null && !string.Equals(val, string.Empty))
                            {
                                modvalue = val;
                            }
                        });

                        model = Db.Models.ToList().FirstOrDefault(mod => string.Equals(mod.ModelTitle, modvalue)) ??
                                 new Model
                                 {
                                     ModelTitle = modvalue
                                 };
                    }
                }
            }
            return model;
        }

        public static ConvSign GetConvSign(ref string modstring)
        {
            List<ConvSign> convs = Db.ConvSigns.Include("convAlters").ToList();
            ConvSign convSign = null;

            foreach (ConvSign item in convs)
            {
                string pattern = item.Key + "$";
                Regex reg = new Regex(pattern);
                if (reg.IsMatch(modstring))
                {
                    modstring = Regex.Replace(modstring, pattern, "");
                    return item;
                }
                foreach (ConvAlter alter in item.ConvAlters)
                {
                    string patalter = alter.AlterKey + "$";
                    Regex regalter = new Regex(patalter);
                    if (reg.IsMatch(modstring))
                    {
                        modstring = Regex.Replace(modstring, patalter, "");
                        return item;
                    }
                    
                }
            }

            return convSign;
        }

        public static SpeedIndex GetSpeedIndex(DataRow row)
        {
            ReadCellSetting sisetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "SpeedIndex")));
            SpeedIndex speed = null;
            if (sisetting != null)
            {
                string sistring = row[sisetting.CellNumber - 1].ToString().Trim();
                Target speedTarget = sisetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "SpeedIndex"));
                if (speedTarget != null)
                {
                    string sivalue = "";
                    string simask = speedTarget.Mask;
                    if (simask != null)
                    {
                        Regex reg = new Regex(simask);
                        speedTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(sistring).Groups[x].Value;
                            if (val != null && !string.Equals(val, string.Empty))
                            {
                                sivalue = val;
                            }
                        });
                        speed = Db.SpeedIndexs.ToList().FirstOrDefault(si => string.Equals(si.Key, sivalue)) ??
                                 new SpeedIndex
                                 {
                                     Key = sivalue
                                 };
                    }
                }
            }
            return speed;
        }
        public static PressIndex GetPressIndex(DataRow row)
        {
            ReadCellSetting pisetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "PressIndex")));
            PressIndex press = null;
            if (pisetting != null)
            {
                string pistring = row[pisetting.CellNumber - 1].ToString().Trim();
                Target pressTarget = pisetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "PressIndex"));
                if (pressTarget != null)
                {
                    string pivalue = "";
                    string pimask = pressTarget.Mask;
                    if (pimask != null)
                    {
                        Regex reg = new Regex(pimask);
                        pressTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(pistring).Groups[x].Value;
                            if (val != null && !string.Equals(val, string.Empty))
                            {
                                pivalue = val;
                            }
                        });
                        press = Db.PressIndexs.ToList().FirstOrDefault(pi => string.Equals(pi.Key, pivalue)) ??
                                 new PressIndex
                                 {
                                     Key = pivalue
                                 };
                    }
                }
            }
            return press;
        }
        public static Diameter GetdDiameter(DataRow row)
        {

            ReadCellSetting diamsetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Diameter")));
            Diameter diameter = null;
            if (diamsetting != null)
            {
                string diastring = row[diamsetting.CellNumber - 1].ToString().Trim();
                Target diaTarget = diamsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Diameter"));
                if (diaTarget != null)
                {
                    string dmvalue = "";
                    string heimask = diaTarget.Mask;
                    if (heimask != null)
                    {
                        Regex reg = new Regex(heimask);
                        diaTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(diastring).Groups[x].Value;
                            if (val != null && !string.Equals(val, string.Empty))
                            {
                                dmvalue = val;
                            }
                        });
                        diameter = Db.Diameters.ToList().FirstOrDefault(d => string.Equals(d.DSize, dmvalue)) ??
                                 new Diameter
                                 {
                                     DSize = dmvalue
                                 };
                    }
                }
            }
            return diameter;
        }
        public static Width GetWidth(DataRow row)
        {

            ReadCellSetting widthsetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Width")));
            Width width = null;
            if (widthsetting != null)
            {
                string heistring = row[widthsetting.CellNumber - 1].ToString().Trim();
                Target widthTarget = widthsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Width"));
                if (widthTarget != null)
                {
                    string widthvalue = "";
                    string heimask = widthTarget.Mask;
                    if (heimask != null)
                    {
                        Regex reg = new Regex(heimask);
                        widthTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(heistring).Groups[x].Value;
                            if (val != null && !string.Equals(val, string.Empty))
                            {
                                widthvalue = val;
                            }
                        });
                        double widthVal;
                        double.TryParse(widthvalue, out widthVal);
                        width = Db.Widths.ToList().FirstOrDefault(h => Math.Abs(h.Value - widthVal) < 0.001) ??
                                 new Width
                                 {
                                     Value = widthVal
                                 };
                    }
                }
            }
            return width;
        }
        public static Height GetHeight(DataRow row)
        {
            ReadCellSetting heightsetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Height")));
            Height height = null;
            if (heightsetting != null)
            {
                string heistring = row[heightsetting.CellNumber - 1].ToString().Trim();
                Target heiTarget = heightsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Height"));
                if (heiTarget != null)
                {
                    string heivalue = "";
                    string heimask = heiTarget.Mask;
                    if (heimask != null)
                    {
                        Regex reg = new Regex(heimask);
                        heiTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(heistring).Groups[x].Value;
                            if (val != null && !string.Equals(val, string.Empty))
                            {
                                heivalue = val;
                            }
                        });
                        double heightVal;
                        double.TryParse(heivalue, out heightVal);
                        height = Db.Heights.ToList().FirstOrDefault(h => Math.Abs(h.Value - heightVal) < 0.001) ??
                                 new Height
                                 {
                                     Value = heightVal
                                 };
                    }
                }
            }
            return height;
        }

        public static Country GetCountry(DataRow row)
        {
            ReadCellSetting countrysetting =
                curSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "CountryTitle")));

            Country country = null;

            if (countrysetting != null)
            {
                string ctstring = row[countrysetting.CellNumber - 1].ToString().Trim();
                CountryTitle countryTitle =
                    Db.CountryTitles.Include("Country").FirstOrDefault(
                        ct => ct.PriceLanguage.Id == PriceLanguage.Id && string.Equals(ct.Title, ctstring));
                if (countryTitle != null)
                {
                    country = countryTitle.Country;
                }
                else
                {
                    country = new Country();
                    country.CountryTitles.Add(new CountryTitle
                    {
                        Title = ctstring,
                        PriceLanguage = PriceLanguage
                    });
                }
            }
            return country;
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
            CityTitle cityTitle = Db.CityTitles.Include("City").FirstOrDefault(st => st.PriceLanguage.Id == PriceLanguage.Id &&
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