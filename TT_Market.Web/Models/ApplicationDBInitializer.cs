using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using TT_Market.Core.Domains;
using WebGrease.Css.Extensions;

namespace TT_Market.Web.Models
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        //private static readonly string _path =
        //    AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
        //    @"TT_Market.Core\DBinitial\Read";

        private static readonly string _path =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web\bin", "") +
            @"TT_Market.Core\DBinitial\Read";
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        static readonly Func<string, double> parseDouble = str =>
        {
            double num = Double.Parse(str);
            return num;
        };
        public static void InitializeIdentityForEF(ApplicationDbContext context)
        {
            LoadAgentsFromXml(context);
            LoadAutoTypesFromXml(context);
            LoadBrandsFromXml(context);
            LoadCountryFromXml(context);
            LoadCurrencyFromXml(context);
            LoadDiametersFromXml(context);
            LoadHeightsFromXml(context);
            LoadLanguagesFromXml(context);
            LoadSeasonsFromXml(context);
            LoadWidthsFromXml(context);
            LoadSpeedIndexesFromXml(context);
            LoadPricesReadSettingsFromXml(context);
        }

        public static void LoadAgentsFromXml( ApplicationDbContext context)
        {
            string path = _path + "AgentsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Agent");
            Agent[] agents = collection.ToList().Select(a => new Agent
            {
                AgentTitle = a.Element("Title").Value,
                City = a.Element("City").Value,
                Phone = a.Element("Phone").Value,
                Email = a.Element("Email").Value
            }).ToArray();

            context.Agents.AddRange(agents);
            context.SaveChanges();

        }
        public static void LoadAutoTypesFromXml(ApplicationDbContext context)
        {
            string path = _path + "AutoTypesInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("AutoType");
            AutoType[] autotypes = collection.ToList().Select(a => new AutoType
            {
                TypeValue = a.Value
            }).ToArray();

            context.AutoTypes.AddRange(autotypes);
            context.SaveChanges();
        }
        public static void LoadBrandsFromXml(ApplicationDbContext context)
        {
            string path = _path + "BrandsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Brand");
            Brand[] brands = collection.ToList().Select(a => new Brand
            {
                BrandTitle = a.Value
            }).ToArray();

            context.Brands.AddRange(brands);
            context.SaveChanges();
        }
        public static void LoadCountryFromXml(ApplicationDbContext context)
        {
            string path = _path + "CountrysInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Country");
            Country[] countries = collection.ToList().Select(a => new Country
            {
                CountryTitle = a.Value
            }).ToArray();

            context.Countrys.AddRange(countries);
            context.SaveChanges();
        }
        public static void LoadCurrencyFromXml(ApplicationDbContext context)
        {
            string path = _path + "CurrencysInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Currency");
            Currency[] currencies = collection.ToList().Select(a => new Currency
            {
                CurrencyTitle = a.Value
            }).ToArray();

            context.Currencys.AddRange(currencies);
            context.SaveChanges();
        }
        public static void LoadDiametersFromXml(ApplicationDbContext context)
        {
            string path = _path + "DiametersInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Diameter");
            Diameter[] diameters = collection.ToList().Select(a => new Diameter
            {
                DSize = a.Value
            }).ToArray();

            context.Diameters.AddRange(diameters);
            context.SaveChanges();
        }
        public static void LoadHeightsFromXml(ApplicationDbContext context)
        {

            string path = _path + "HeightsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Height");
            Height[] heights = collection.ToList().Select(a => new Height
            {
                Value = parseDouble(a.Value)
            }).ToArray();

            context.Heights.AddRange(heights);
            context.SaveChanges();
        }
        public static void LoadLanguagesFromXml(ApplicationDbContext context)
        {
            string path = _path + "LanguagesInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Language");
            PriceLanguage[] priceLanguages = collection.ToList().Select(a => new PriceLanguage
            {
                LanguageName = a.Value
            }).ToArray();

            context.PriceLanguages.AddRange(priceLanguages);
            context.SaveChanges();
        }
        public static void LoadSeasonsFromXml(ApplicationDbContext context)
        {
            string path = _path + "SeasonsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Season");
            Season[] seasons = collection.ToList().Select(a => new Season
            {
                SeasonTitle = a.Value
            }).ToArray();

            context.Seasons.AddRange(seasons);
            context.SaveChanges();
        }
        public static void LoadWidthsFromXml(ApplicationDbContext context)
        {
            string path = _path + "WidthsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Width");
            Width[] widths = collection.ToList().Select(a => new Width
            {
                Value = parseDouble(a.Value)
            }).ToArray();

            context.Widths.AddRange(widths);
            context.SaveChanges();
        }
        public static void LoadSpeedIndexesFromXml(ApplicationDbContext context)
        {
            string path = _path + "SpeedIndexsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("SpeedIndex");
            SpeedIndex[] spindexes = collection.ToList().Select(a => new SpeedIndex
            {
                Value = a.Value
            }).ToArray();

            context.SpeedIndexs.AddRange(spindexes);
            context.SaveChanges();
        }
        public static void LoadPricesReadSettingsFromXml(ApplicationDbContext context)
        {
            context.SaveChanges();
            string path = _path + "PriceReadSettingsInitial.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList priceLists = doc.SelectNodes("descendant::PriceReadSetting");
            List<PriceReadSetting> lists = new List<PriceReadSetting>();
            foreach (XmlNode price in priceLists)
            {
                //DateTime dt = DateTime.Parse(price.SelectSingleNode("InsertDate").InnerText);
                string fn = price.SelectSingleNode("FileName").InnerText;
                try
                {
                    //List<PriceLanguage> plangs = context.PriceLanguages;
                    string nodelangvalue = price.SelectSingleNode("PriceLanguage").InnerText;
                    PriceLanguage plg = context.PriceLanguages.ToList().FirstOrDefault(
                            pl => string.Equals(pl.LanguageName, nodelangvalue));
                    string nodeagentvalue = price.SelectSingleNode("Agent").InnerText;
                    Agent aq = context.Agents.ToList()
                        .FirstOrDefault(a => string.Equals(a.AgentTitle, nodeagentvalue));
                    string trmask = Newtonsoft.Json.JsonConvert.SerializeXmlNode(price.SelectSingleNode("ReadSettings"));
                    PriceReadSetting pricelist = new PriceReadSetting
                    {
                        FileName = fn,
                        PriceLanguage = plg,
                        Agent = aq,
                        TransformMask = trmask
                    };
                    lists.Add(pricelist);

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }
            context.PriceReadSettings.AddRange(lists);
            context.SaveChanges();
        }
    }
}