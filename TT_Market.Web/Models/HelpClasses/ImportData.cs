using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Excel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using NLog;
using TT_Market.Core.Domains;
using TT_Market.Core.HelpClasses;
using TT_Market.Core.Identity;

namespace TT_Market.Web.Models.HelpClasses
{
    public static class ImportData
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly ApplicationDbContext Db = new ApplicationDbContext();
        private static PriceLanguage PriceLanguage { get; set; }
        private static PriceDocument PriceDocument { get; set; }
        private static ReadSheetSetting CurSheetSetting { get; set; }
        private static bool PrevDocumentExists { get; set; }

        public static void ParseAndInsert(string path)
        {

            string filename = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            PriceDocument pdoc = new PriceDocument();
            PriceDocument = pdoc;
            PriceReadSetting priceReadSetting = Db.PriceReadSettings.Include("PriceLanguage").Include("Agent")
                .FirstOrDefault(prs => string.Equals(prs.FileName, filename));
            PrevDocumentExists = false;

            if (priceReadSetting != null)
            {
                Db.PriceDocuments.Add(pdoc);
                PrevDocumentExists = priceReadSetting.PriceDocuments.Any();
                logger.Trace("{0} - PrevDocument {1} Exists", priceReadSetting.FileName, PrevDocumentExists ? "" : "not");
                PriceLanguage = priceReadSetting.PriceLanguage;
                PriceDocument.DownLoadDate = DateTime.UtcNow;

                JObject jobj = JObject.Parse(priceReadSetting.TransformMask);
                Dictionary<string, ReadSheetSetting> docReadSettings = ReadSetting.GetReadSetting(jobj);
                foreach (var pair in docReadSettings)
                {
                    CurSheetSetting = pair.Value;

                    IEnumerable<DataRow> dataCollection = GetRealDataRows(path, pair.Key);

                    List<ReadCellSetting> tirePropos =
                        CurSheetSetting.ReadCellSettings.Where(
                            rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "TireProposition"))).ToList();

                    List<ReadCellSetting> tirePrices =
                        CurSheetSetting.ReadCellSettings.Where(
                            rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "TirePrice"))).ToList();

                    List<ReadCellSetting> stockValues =
                        CurSheetSetting.ReadCellSettings.Where(
                            rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Stock"))).ToList();
                    List<ReadCellSetting> brandValues =
                        CurSheetSetting.ReadCellSettings.Where(
                            rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Brand"))).ToList();

                    foreach (DataRow row in dataCollection)
                    {
                        if (CurSheetSetting.SkipColumn != null &&
                            (string.Equals(row[(int) CurSheetSetting.SkipColumn - 1].ToString(), string.Empty)))
                        {
                            continue;
                        }
                        TireProposition tireProposition = ReadTireProposition(tirePropos, row);

                        TirePrice tirePrice = ReadTirePrice(tirePrices, row);

                        tireProposition.TirePrices.Add(tirePrice);

                        foreach (ReadCellSetting rcs in stockValues)
                        {
                            tireProposition.Stocks.Add(ReadStock(rcs, row));
                        }
                        if (tireProposition.Stocks.Count == 0)
                        {
                            logger.Trace("Stocks was not recognised at line {0}", row.Table.Rows.IndexOf(row));
                        }
                        pdoc.TirePropositions.Add(tireProposition);
                        Tire tire = ReadTire(row);
                        tire.TireProposition = tireProposition;

                        Db.Tires.Add(tire);
                    }
                }
                Db.SaveChanges();
            }
        }

        public static int? CheckStartReadRow(IEnumerable<DataRow> rows)
        {
            if (CurSheetSetting.StartRow.TitleRow != null)
            {
                int definedStart = CurSheetSetting.StartRow.TitleRow.Value - 1;
                int? rowspan = CurSheetSetting.StartRow.TitleRowSpan;
                if (CurSheetSetting.StartRow.RewievColumn != null)
                {
                    IEnumerable<DataRow> dataRows = rows as IList<DataRow> ?? rows.ToList();
                    string controlTitle =
                        dataRows.ElementAt(definedStart - 1).ItemArray[CurSheetSetting.StartRow.RewievColumn.Value - 1]
                            .ToString
                            ().Trim();
                    if (string.Equals(CurSheetSetting.StartRow.TitleRowPattern, controlTitle))
                    {
                        return definedStart + (rowspan ?? 0);
                    }
                    IEnumerable<DataRow> startDataRows = dataRows.Take(10);
                    foreach (
                        DataRow row in
                            startDataRows.Where(
                                row =>
                                    string.Equals(CurSheetSetting.StartRow.TitleRowPattern,
                                        row.ItemArray[CurSheetSetting.StartRow.RewievColumn.Value - 1].ToString())))
                    {
                        return row.Table.Rows.IndexOf(row) + 1;
                    }
                }
            }
            return null;
        }

        public static List<DataRow> GetRealDataRows(string path,string sheetname)
        {
            List<DataRow> datacollection = GetData(path, sheetname, false).ToList();
            int? startrow = CheckStartReadRow(datacollection);
            if (startrow != null) datacollection = datacollection.Skip(startrow.Value).ToList();

            if (CurSheetSetting.EndRow != null)
            {
                if (CurSheetSetting.EndRow.RewievColumn != null)
                {
                    int rewColumn = CurSheetSetting.EndRow.RewievColumn.Value;
                    string stopPattern = CurSheetSetting.EndRow.StopReadPattern;
                    if (!string.Equals(stopPattern,string.Empty))
                    {
                        Regex stopReg = new Regex(stopPattern,RegexOptions.Compiled|RegexOptions.Multiline);
                        int? skipCol = CurSheetSetting.SkipColumn;
                        if (skipCol!=null)
                        {
                            datacollection =
                                datacollection.FindAll(row => !string.IsNullOrEmpty(row[(int) skipCol - 1].ToString()));
                        }

                        datacollection = datacollection.FindAll(row => stopReg.IsMatch(row[rewColumn].ToString().Trim()));
                    }
                }
            }
            return datacollection;
        }
        public static Tire ReadTire(DataRow row)
        {
            ReadCellSetting celsetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Tire")));

            Tire tire = new Tire();
            string tiretitle = row[celsetting.CellNumber - 1].ToString().Trim();
            if (PrevDocumentExists)
            {
                Tire prevtire =
                    Db.Tires.Include("TireProposition.PriceDocument.PriceReadSetting")
                        .ToList().FirstOrDefault(
                            t =>
                                string.Equals(t.TireProposition.PriceDocument.PriceReadSetting.FileName, PriceDocument.PriceReadSetting.FileName) &&
                                string.Equals(t.TireTitle, tiretitle));
                if (prevtire != null)
                {
                    tire.Model = prevtire.Model;
                    tire.Diameter = prevtire.Diameter;
                    tire.PressIndex = prevtire.PressIndex;
                    tire.TireTitle = tiretitle;
                    tire.Height = prevtire.Height;
                    tire.SpeedIndex = prevtire.SpeedIndex;
                    tire.Width = prevtire.Width;
                    tire.Country = prevtire.Country;
                }
            }
            else
            {
                tire.TireTitle = tiretitle;
                tire.Country = GetCountry(row);
                tire.Width = GetWidth(row);
                tire.Height = GetHeight(row);
                tire.Diameter = GetdDiameter(row);
                tire.SpeedIndex = GetSpeedIndex(row);
                tire.PressIndex = GetPressIndex(row);
                tire.ProductionYear = GetProductionYear(row);
                tire.Model = GetModel(row);
            }
            return tire;
        }

        public static ProductionYear GetProductionYear(DataRow row)
        {
            ReadCellSetting pysetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "ProductionYear")));
            ProductionYear prodYear = null;
            if (pysetting != null)
            {
                string pystring = row[pysetting.CellNumber - 1].ToString().Trim();
                Target pyTarget = pysetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "ProductionYear"));
                if (pyTarget != null)
                {
                    string pyvalue = "";
                    string pymask = pyTarget.Mask;
                    if (pymask != null)
                    {
                        Regex reg = new Regex(pymask, RegexOptions.Compiled);
                        pyTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(pystring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                pyvalue = val;
                            }
                        });
                        if (!pyvalue.IsNullOrWhiteSpace())
                        {
                            prodYear = Db.ProductionYears.ToList().FirstOrDefault(py => string.Equals(py.Year, pyvalue)) ??
                                     new ProductionYear
                                     {
                                         Year = pyvalue
                                     };
                            
                        }
                    }
                }
            }
            return prodYear;
            
        }
        public static Brand GetBrand(List<ReadCellSetting> brandssettings, DataRow row)
        {
            foreach (ReadCellSetting setting in brandssettings)
            {
                string brandstring = row[setting.CellNumber - 1].ToString().Trim();
                Target brandTarget = setting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Brand"));
                if (setting.Targets.Count > 1)
                {
                    string brandvalue = "";
                    string pimask = brandTarget.Mask;
                    if (pimask != null)
                    {
                        Regex reg = new Regex(pimask, RegexOptions.Compiled);
                        brandTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(brandstring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                brandvalue = val;
                            }
                        });
                        return Db.Brands.ToList().FirstOrDefault(b => string.Equals(b.BrandTitle, brandvalue)) ??
                               new Brand { BrandTitle = brandvalue };
                    }
                }
                else
                {
                    return Db.Brands.ToList().FirstOrDefault(b => string.Equals(b.BrandTitle, brandstring)) ??
                           new Brand {BrandTitle = brandstring};
                }
            }
            return null;
        }

        public static Season GetSeason(DataRow row)
        {

            ReadCellSetting seasonsetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "SeasonTitle")));
            Season season = null;
            SeasonTitle seasontitle = null;
            if (seasonsetting != null)
            {
                string seasonstring = row[seasonsetting.CellNumber - 1].ToString().Trim();
                if (!seasonstring.IsNullOrWhiteSpace())
                {
                    Target seasonTarget =
                        seasonsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "SeasonTitle"));
                    if (seasonTarget != null)
                    {
                        string seasvalue = "";
                        string seasmask = seasonTarget.Mask;
                        if (!seasmask.IsNullOrWhiteSpace())
                        {
                            Regex reg = new Regex(seasmask, RegexOptions.Compiled);
                            seasonTarget.Groups.ForEach(x =>
                            {
                                string val = reg.Match(seasonstring).Groups[x].Value;
                                if (!val.IsNullOrWhiteSpace())
                                {
                                    seasvalue = val;
                                }
                            });

                            seasontitle =
                                (Db.SeasonTitles.ToList().FirstOrDefault(st => string.Equals(st.Title, seasvalue)) ??
                                 Db.SeasonTitles.Include("SeasonTitleAlters")
                                     .ToList()
                                     .FirstOrDefault(
                                         at =>
                                             at.SeasonTitleAlters.Any(
                                                 ata => string.Equals(ata.TitleAlterValue, seasvalue)))) ??
                                new SeasonTitle
                                {
                                    Title = seasvalue,
                                    PriceLanguage = PriceLanguage
                                };
                        }
                        else
                        {
                            seasontitle =
                                Db.SeasonTitles.ToList().FirstOrDefault(st => string.Equals(st.Title, seasonstring)) ??
                                new SeasonTitle
                                {
                                    Title = seasonstring,
                                    PriceLanguage = PriceLanguage
                                };
                        }
                        season =
                            Db.Seasons.Include("SeasonTitles")
                                .ToList()
                                .FirstOrDefault(
                                    s => s.SeasonTitles.Any(st => string.Equals(st.Title, seasontitle.Title)));
                        if (season == null)
                        {
                            season = new Season();
                            season.SeasonTitles.Add(seasontitle);
                        }
                    }
                }
            }
            return season;
        }

        public static ProtectorType GetProtectorType(DataRow row)
        {
            ReadCellSetting protTypeSetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "ProtectorType")));
            ProtectorType ptType = null;
            if (protTypeSetting!=null)
            {
                string ptstring = row[protTypeSetting.CellNumber - 1].ToString().Trim();
                Target pTarget = protTypeSetting.Targets.FirstOrDefault(pt => string.Equals(pt.Entity, "ProtectorType"));
                if (pTarget!=null)
                {
                    string ptValue = "";
                    string ptMask = pTarget.Mask;
                    if (ptMask != null)
                    {
                        Regex reg = new Regex(ptMask, RegexOptions.Compiled);
                        pTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(ptstring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                ptValue = val;
                            }
                        });
                        if (!ptValue.IsNullOrWhiteSpace())
                        {
                            ptType = Db.ProtectorTypes.ToList().FirstOrDefault(pt => string.Equals(pt.Title, ptValue)) ??
                                     new ProtectorType { Title = ptValue };
                        }
                    }
                    else
                    {
                        if (!ptstring.IsNullOrWhiteSpace())
                        {
                            ptType = Db.ProtectorTypes.ToList().FirstOrDefault(pt => string.Equals(pt.Title, ptstring)) ??
                                 new ProtectorType { Title = ptstring };
                        }
                    }
                }
            }
            return ptType;
        }
        public static Model GetModel(DataRow row)
        {
            ReadCellSetting modelsetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Model")));
            Model model = null;
            List<ConvSign> convSigns = null;
            HomolAttribute ha = null;
            HomolAttribute homAttribute = null;
            Season season = null;
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
                        Regex reg = new Regex(mdmask, RegexOptions.Compiled);
                        modelTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(mdstring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                modvalue = val;
                            }
                        });
                        convSigns = GetConvSigns(ref modvalue, row);

                        ha = GetHomolAttributeFromModelString(ref modvalue);
                        modvalue = modvalue.Trim();
                        season = GetSeason(row);
                        model = Db.Models.ToList().FirstOrDefault(mod => string.Equals(mod.ModelTitle, modvalue)) ??
                                 new Model
                                 {
                                     ModelTitle = modvalue
                                 };
                        model.Homol = model.Homol ?? ha;
                        model.ConvSigns = model.ConvSigns ?? convSigns;
                        model.Season = model.Season ?? season;
                    }
                }
            }

            return model;
        }

        public static HomolAttribute GetHomolAttributeFromModelString(ref string modelstring)
        {
            List<HomolAttribute> homols = Db.HomolAttributes.ToList();
            foreach (HomolAttribute item in homols)
            {
                if (item.Key.StartsWith("*"))
                {
                    item.Key = @"\" + item.Key;
                }
                string pattern = string.Format(@"\s{0}\s?",item.Key);
                Regex reg = new Regex(pattern,RegexOptions.Compiled);
                if (reg.IsMatch(modelstring))
                {
                    modelstring = Regex.Replace(modelstring, pattern, " ");
                    return item;
                }
            }

            return null;
        }

        public static AutoType GetAutoType(DataRow row)
        {
            ReadCellSetting autotypesetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "AutoType")));
            AutoType autotype = null;
            if (autotypesetting != null)
            {
                string atstring = row[autotypesetting.CellNumber - 1].ToString().Trim();
                if (atstring.IsNullOrWhiteSpace()) return null;
                Target atTarget = autotypesetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "AutoType"));
                if (atTarget == null) return null;
                string atvalue = "";
                string atmask = atTarget.Mask;
                if (atmask != null)
                {
                    Regex reg = new Regex(atmask, RegexOptions.Compiled);
                    atTarget.Groups.ForEach(x =>
                    {
                        string val = reg.Match(atstring).Groups[x].Value;
                        if (!val.IsNullOrWhiteSpace())
                        {
                            atvalue = val;
                        }
                    });
                    if (!atvalue.IsNullOrWhiteSpace())
                    {
                        autotype =
                            (Db.AutoTypes.ToList().FirstOrDefault(at => string.Equals(at.TypeValue, atvalue)) ??
                             Db.AutoTypes.Include("AutoTypeAlters")
                                 .ToList()
                                 .FirstOrDefault(
                                     at => at.AutoTypeAlters.Any(ata => string.Equals(ata.AlterValue, atvalue)))) ??
                            new AutoType
                            {
                                TypeValue = atvalue
                            };
                    }
                }
                else
                {
                    autotype =
                        Db.AutoTypes.ToList().FirstOrDefault(at => string.Equals(at.TypeValue, atstring)) ??
                        new AutoType
                        {
                            TypeValue = atstring
                        };
                }
            }
            return autotype;
        }

        public static List<ConvSign> GetConvSigns(ref string modstring, DataRow row)
        {
            ReadCellSetting convSigSetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "ConvSign")));

            List<ConvSign> convSigns = new List<ConvSign>();
            if (convSigSetting != null)
            {
                Target csTarget = convSigSetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "ConvSign"));
                if (csTarget != null)
                {
                    List<string> targetsstrings =
                        new List<string>(new[] {modstring.Trim(), row[convSigSetting.CellNumber - 1].ToString()});
                    foreach (string item in targetsstrings)
                    {
                        List<string> csvalues = new List<string>();
                        string csmask = csTarget.Mask;
                        if (csmask != null)
                        {
                            Regex reg = new Regex(csmask, RegexOptions.Compiled);
                            csTarget.Groups.ForEach(x =>
                            {
                                string val = reg.Match(item).Groups[x].Value;
                                if (!val.IsNullOrWhiteSpace())
                                {
                                    csvalues.Add(val);
                                }
                            });
                            convSigns.AddRange(
                                csvalues.Select(
                                    sign =>
                                        (Db.ConvSigns.ToList().FirstOrDefault(cs => string.Equals(cs.Key, sign)) ??
                                         Db.ConvSigns.Include("ConvAlters")
                                             .ToList()
                                             .FirstOrDefault(
                                                 at => at.ConvAlters.Any(csa => string.Equals(csa.AlterKey, sign)))) ??
                                        new ConvSign
                                        {
                                            Key = sign
                                        }));
                        }
                    }

                    return convSigns;
                }
            }

            List<ConvSign> convs = Db.ConvSigns.Include("ConvAlters").ToList();

            foreach (ConvSign item in convs)
            {
                string pattern = string.Format(@"\s{0}\s?", item.Key);
                Regex reg = new Regex(pattern, RegexOptions.Compiled);
                if (reg.IsMatch(modstring))
                {
                    modstring = Regex.Replace(modstring, pattern, " ");
                    convSigns.Add(item);
                }
                foreach (ConvAlter alter in item.ConvAlters)
                {
                    string patalter = string.Format(@"\s{0}\s?", alter.AlterKey);

                    if (reg.IsMatch(modstring))
                    {
                        modstring = Regex.Replace(modstring, patalter, " ");
                        convSigns.Add(item);
                    }
                    
                }
            }
            //Извлечь все известные условные обозначения

            return convSigns;
        }

        public static SpeedIndex GetSpeedIndex(DataRow row)
        {
            ReadCellSetting sisetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
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
                    if (!simask.IsNullOrWhiteSpace())
                    {
                        Regex reg = new Regex(simask, RegexOptions.Compiled);
                        speedTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(sistring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                sivalue = val;
                            }
                        });
                        if (!sivalue.IsNullOrWhiteSpace())
                        {
                            speed = Db.SpeedIndexs.ToList().FirstOrDefault(si => string.Equals(si.Key, sivalue)) ??
                                     new SpeedIndex
                                     {
                                         Key = sivalue
                                     };
                            
                        }
                    }
                }
            }
            return speed;
        }
        public static PressIndex GetPressIndex(DataRow row)
        {
            ReadCellSetting pisetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
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
                    if (!pimask.IsNullOrWhiteSpace())
                    {
                        Regex reg = new Regex(pimask, RegexOptions.Compiled);
                        pressTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(pistring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                pivalue = val;
                            }
                        });
                        if (!pivalue.IsNullOrWhiteSpace())
                        {
                            press = Db.PressIndexs.ToList().FirstOrDefault(pi => string.Equals(pi.Key, pivalue)) ??
                                     new PressIndex
                                     {
                                         Key = pivalue
                                     };
                        }
                    }
                }
            }
            return press;
        }
        public static Diameter GetdDiameter(DataRow row)
        {
                        ReadCellSetting diamsetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Diameter")));
            Diameter diameter = null;
            if (diamsetting != null)
            {
                string diastring = row[diamsetting.CellNumber - 1].ToString().Trim();
                Target diaTarget = diamsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Diameter"));
                if (diaTarget != null)
                {
                    string dmvalue = "";
                    string dmmask = diaTarget.Mask;
                    if (!dmmask.IsNullOrWhiteSpace())
                    {
                        Regex reg = new Regex(dmmask, RegexOptions.Compiled);
                        diaTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(diastring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                dmvalue = val;
                            }
                        });
                    }
                    else
                    {
                        dmvalue = diastring;
                    }
                    diameter = Db.Diameters.ToList().FirstOrDefault(d => string.Equals(d.DSize, dmvalue)) ??
                             new Diameter
                             {
                                 DSize = dmvalue
                             };
                }
            }
            return diameter;
        }

        public static Width GetWidth(DataRow row)
        {

            ReadCellSetting widthsetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "Width")));
            Width width = null;
            if (widthsetting != null)
            {
                string widthstring = row[widthsetting.CellNumber - 1].ToString().Trim();
                Target widthTarget = widthsetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Width"));
                if (widthTarget != null)
                {
                    string widthvalue = "";
                    string widthmask = widthTarget.Mask;
                    if (!widthmask.IsNullOrWhiteSpace())
                    {
                        Regex reg = new Regex(widthmask, RegexOptions.Compiled);
                        widthTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(widthstring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                widthvalue = val;
                            }
                        });
                    }
                    else
                    {
                        widthvalue = widthstring;
                    }
                    double widthVal;
                    double.TryParse(widthvalue, out widthVal);
                    width = Db.Widths.ToList().FirstOrDefault(h => Math.Abs(h.Value - widthVal) < 0.001) ??
                            new Width
                            {
                                Value = widthVal
                            };
                }
            }
            return width;
        }

        public static Height GetHeight(DataRow row)
        {
            ReadCellSetting heightsetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
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
                    if (!heimask.IsNullOrWhiteSpace())
                    {
                        Regex reg = new Regex(heimask, RegexOptions.Compiled);
                        heiTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(heistring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                heivalue = val;
                            }
                        });
                    }
                    else
                    {
                        heivalue = heistring;
                    }
                    double heightVal;
                    if (!heivalue.IsNullOrWhiteSpace())
                    {
                        double.TryParse(heivalue, out heightVal);
                        height = Db.Heights.ToList().FirstOrDefault(h => Math.Abs(h.Value - heightVal) < 0.001) ??
                                 new Height
                                 {
                                     Value = heightVal
                                 };
                    }
                    else
                    {
                        height = Db.Heights.ToList().FirstOrDefault(h => Math.Abs(h.Value - 82.00) < 0.001);
                    }
                }
            }
            return height;
        }

        public static Country GetCountry(DataRow row)
        {
            Country country = null;
            ReadCellSetting countrysetting =
                CurSheetSetting.ReadCellSettings.FirstOrDefault(
                    rcs => rcs.Targets.Any(t => string.Equals(t.Entity, "CountryTitle")));
            if (countrysetting != null)
            {
                Target cuontryTarget =
                    countrysetting.Targets.FirstOrDefault(t => string.Equals(t.Entity, "CountryTitle"));
                if (cuontryTarget != null)
                {
                    string ctstring = row[countrysetting.CellNumber - 1].ToString().Trim();
                    if (ctstring.IsNullOrWhiteSpace()) return country;
                    string countrymask = cuontryTarget.Mask;
                    CountryTitle countryTitle = null;
                    if (!countrymask.IsNullOrWhiteSpace())
                    {
                        string csvalue = "";
                        Regex reg = new Regex(countrymask, RegexOptions.Compiled);
                        cuontryTarget.Groups.ForEach(x =>
                        {
                            string val = reg.Match(ctstring).Groups[x].Value;
                            if (!val.IsNullOrWhiteSpace())
                            {
                                csvalue = val;
                            }
                        });
                        countryTitle = Db.CountryTitles.Include("Country").ToList().FirstOrDefault(
                            ct => ct.PriceLanguage.Id == PriceLanguage.Id && string.Equals(ct.Title, csvalue));
                    }
                    else
                    {
                        countryTitle = Db.CountryTitles.Include("Country").ToList().FirstOrDefault(
                            ct => ct.PriceLanguage.Id == PriceLanguage.Id && string.Equals(ct.Title, ctstring));
                    }

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
            CityTitle cityTitle =
                Db.CityTitles.Include("City")
                    .ToList()
                    .FirstOrDefault(st => cityTarget != null && (st.PriceLanguage.Id == PriceLanguage.Id &&
                                                                 string.Equals(st.Title, cityTarget.Value)));
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
                        Title = row[celsetting.CellNumber-1].ToString().Trim(),
                        PriceLanguage = PriceLanguage
                    });
            }
            if (city == null || city.CityTitles.Count == 0)
            {
                logger.Trace("CityTitle was not recognised at line {0}", row.Table.Rows.IndexOf(row));
            }
            return city;
        }

        public static TireProposition ReadTireProposition(List<ReadCellSetting> celsettings, DataRow row)
        {

            TireProposition tireProposition = new TireProposition();
            bool wasRecognized = false;
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
                            tireProposition.TirePriceCode = row[rcs.CellNumber - 1].ToString().Trim();
                            wasRecognized = true;
                            break;
                        case "ExtendedData":
                            tireProposition.ExtendedData = row[rcs.CellNumber - 1].ToString().Trim();
                            wasRecognized = true;
                            break;
                        case "RegionCount":
                            int regcount;
                            int.TryParse(row[rcs.CellNumber - 1].ToString().Trim(), out regcount);
                            tireProposition.RegionCount = regcount;
                            wasRecognized = true;
                            break;
                        case "PartnersCount":
                            int partcount;
                            int.TryParse(row[rcs.CellNumber - 1].ToString().Trim(), out partcount);
                            tireProposition.PartnersCount = partcount;
                            wasRecognized = true;
                            break;
                        case "WaitingCount":
                            int wait;
                            int.TryParse(row[rcs.CellNumber - 1].ToString().Trim(), out wait);
                            tireProposition.WaitingCount = wait;
                            wasRecognized = true;
                            break;
                        case "ReservCount":
                            int rescount;
                            int.TryParse(row[rcs.CellNumber - 1].ToString().Trim(), out rescount);
                            tireProposition.ReservCount = rescount;
                            wasRecognized = true;
                            break;
                    }
                }
            }
            if (!wasRecognized)
            {
                logger.Trace("TireProposition was not recognised at line {0}", row.Table.Rows.IndexOf(row));
            }
            return tireProposition;
        }

        public static TirePrice ReadTirePrice(List<ReadCellSetting> celsettings, DataRow row)
        {
            TirePrice tirePrice = new TirePrice();
            bool wasRecognized = false;
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
                            double regprice;
                            double.TryParse(row[price.CellNumber - 1].ToString().Trim(), out regprice);
                            tirePrice.RegularPrice = regprice;
                            wasRecognized = true;
                            break;
                        case "DiscountPrice":
                            double disc;
                            double.TryParse(row[price.CellNumber - 1].ToString().Trim(), out disc);
                            tirePrice.DiscountPrice = disc;
                            wasRecognized = true;
                            break;
                        case "SpecialPrice":
                            double specpr;
                            double.TryParse(row[price.CellNumber - 1].ToString().Trim(), out specpr);
                            tirePrice.SpecialPrice = specpr;
                            wasRecognized = true;
                            break;
                    }
                }
                var curValue = price.Targets.FirstOrDefault(t => string.Equals(t.Entity, "Currency"));
                tirePrice.Currency = Db.Currencys.ToList().FirstOrDefault(
                    c =>
                        (curValue != null)
                            ? string.Equals(c.CurrencyTitle, curValue.Value)
                            : c.IsDefault);

            }
            if (!wasRecognized)
            {
                logger.Trace("TirePrice was not recognised at line {0}", row.Table.Rows.IndexOf(row));
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
                IExcelDataReader dataReader = null;

                if (path.EndsWith(".xls"))
                    dataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                else if (path.EndsWith(".xlsx"))
                    dataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                else
                    logger.Error("The file to be processed is not an Excel file");

                if (dataReader != null)
                {
                    dataReader.IsFirstRowAsColumnNames = isFirstRowAsColumnNames;
                }
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
                logger.Error("The worksheet {0} does not exist, has an incorrect name, or does not have any data in the worksheet", workSheetName);
            return workSheet;
        }

        private static IEnumerable<DataRow> GetData(string path, string workSheetName, bool isFirstRowAsColumnNames = true)
        {
            return from DataRow row in GetExcelWorkSheet(path, workSheetName, isFirstRowAsColumnNames).Rows select row;
        }
    }
}