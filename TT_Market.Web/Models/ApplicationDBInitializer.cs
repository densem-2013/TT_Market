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
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
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

        private static readonly Func<string, double> parseDouble = str =>
        {
            double num = Double.Parse(str);
            return num;
        };

        public static void InitializeIdentityForEF(ApplicationDbContext context)
        {
            LoadAgentsFromXml(context);
            LoadAutoTypesFromXml(context);
            //LoadBrandsFromXml(context);
            LoadCountryFromXml(context);
            LoadCurrencyFromXml(context);
            LoadDiametersFromXml(context);
            LoadHeightsFromXml(context);
            LoadLanguagesFromXml(context);
            //LoadSeasonsFromXml(context);
            LoadWidthsFromXml(context);
            LoadSpeedIndexesFromXml(context);
            LoadPressIndexesFromXml(context);
            //LoadProtectorTypeFromXml(context);
            LoadConvSignsFromXml(context);
            //LoadHomolAttriFromXml(context);
            LoadModelFromXML(context);
            LoadPricesReadSettingsFromXml(context);
        }

        public static void LoadAgentsFromXml(ApplicationDbContext context)
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

        public static IEnumerable<XElement> LoadAutoTypesFromXml(ApplicationDbContext context)
        {
            //string path = _path + "AutoTypesInitial.xml";
            //var xml = XDocument.Load(path);
            //var collection = xml.Root.Descendants("AutoType");
            //AutoType[] autotypes = collection.ToList().Select(a => new AutoType
            //{
            //    TypeValue = a.Value
            //}).ToArray();
            string path = _path.Replace("Read", @"From1C\for.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.приминимость");
            var loadAutoTypesFromXml = collection as XElement[] ?? collection.ToArray();
            AutoType[] autotypes = loadAutoTypesFromXml.ToList().Select(a => new AutoType
            {
                TypeValue = a.Element("Description").Value
            }).ToArray();
            context.AutoTypes.AddRange(autotypes);
            context.SaveChanges();
            return loadAutoTypesFromXml;
        }

        public static IEnumerable<XElement> LoadBrandsFromXml(ApplicationDbContext context)
        {
            //string path = _path + "BrandsInitial.xml";
            //var xml = XDocument.Load(path);
            //var collection = xml.Root.Descendants("Brand");
            //Brand[] brands = collection.ToList().Select(a => new Brand
            //{
            //    BrandTitle = a.Value
            //}).ToArray();
            string path = _path.Replace("Read", @"From1C\brand.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.производители");
            var loadBrandsFromXml = collection as XElement[] ?? collection.ToArray();
            Brand[] brands = loadBrandsFromXml.ToList().Select(a => new Brand
            {
                BrandTitle = a.Element("Description").Value
            }).ToArray();

            context.Brands.AddRange(brands);
            context.SaveChanges();
            return loadBrandsFromXml;
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

        public static IEnumerable<XElement> LoadSeasonsFromXml(ApplicationDbContext context)
        {
            string path = _path + "SeasonsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Season");
            var loadSeasonsFromXml = collection as XElement[] ?? collection.ToArray();
            Season[] seasons = loadSeasonsFromXml.ToList().Select(a => new Season
            {
                SeasonTitle = a.Value
            }).ToArray();

            context.Seasons.AddRange(seasons);
            context.SaveChanges();
            return loadSeasonsFromXml;
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
            //string path = _path + "SpeedIndexsInitial.xml";
            string path = _path.Replace("Read", @"From1C\speed.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.ИндексСкорости");
            SpeedIndex[] spindexes = collection.ToList().Select(a => new SpeedIndex
            {
                Key = a.Element("Description").Value,
                Value = a.Element("Значение").Value
            }).ToArray();

            context.SpeedIndexs.AddRange(spindexes);
            context.SaveChanges();
        }

        public static void LoadPressIndexesFromXml(ApplicationDbContext context)
        {
            //string path = _path + "SpeedIndexsInitial.xml";
            string path = _path.Replace("Read", @"From1C\press.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.ИндексНагрузки");
            PressIndex[] prindexes = collection.ToList().Select(a => new PressIndex
            {

                Key = a.Element("Description").Value,
                Value = Int32.Parse("Значение")
            }).ToArray();

            context.PressIndexs.AddRange(prindexes);
            context.SaveChanges();
        }

        public static IEnumerable<XElement> LoadProtectorTypeFromXml(ApplicationDbContext context)
        {
            //string path = _path + "SpeedIndexsInitial.xml";
            string path = _path.Replace("Read", @"From1C\type.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.Типпротектора");
            var loadProtectorTypeFromXml = collection as XElement[] ?? collection.ToArray();
            ProtectorType[] prottypes = loadProtectorTypeFromXml.ToList().Select(a => new ProtectorType
            {
                Title = a.Element("Description").Value
            }).ToArray();

            context.ProtectorTypes.AddRange(prottypes);
            context.SaveChanges();
            return loadProtectorTypeFromXml;
        }

        public static IEnumerable<XElement> LoadConvSignsFromXml(ApplicationDbContext context)
        {
            //string path = _path + "SpeedIndexsInitial.xml";
            string path = _path.Replace("Read", @"From1C\uslov.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.УслОбозначения");
            var loadConvSignsFromXml = collection as XElement[] ?? collection.ToArray();
            ConvSign[] convindexes = loadConvSignsFromXml.ToList().Select(a => new ConvSign
            {

                Key = a.Element("Description").Value,
                Value = a.Element("Значение").Value
            }).ToArray();

            context.ConvSigns.AddRange(convindexes);
            context.SaveChanges();
            return loadConvSignsFromXml;
        }
        public static IEnumerable<XElement> LoadHomolAttriFromXml(ApplicationDbContext context)
        {
            //string path = _path + "SpeedIndexsInitial.xml";
            string path = _path.Replace("Read", @"From1C\omolog.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.Омологации");
            var loadHomolAttriFromXml = collection as XElement[] ?? collection.ToArray();
            HomolAttribute[] homindexes = loadHomolAttriFromXml.ToList().Select(a => new HomolAttribute
            {

                Key = a.Element("Description").Value,
                Value = (a.Element("Значение")!=null)?a.Element("Значение").Value:null
            }).ToArray();

            context.HomolAttributes.AddRange(homindexes);
            context.SaveChanges();
            return loadHomolAttriFromXml;
        }

        public static void LoadModelFromXML(ApplicationDbContext context)
        {
            IEnumerable<XElement> Brands = LoadBrandsFromXml(context);
            IEnumerable<XElement> Seasons = LoadSeasonsFromXml(context);
            IEnumerable<XElement> ProtectorTypes = LoadProtectorTypeFromXml(context);
            IEnumerable<XElement> Homols = LoadHomolAttriFromXml(context);

            //string path = _path + "SpeedIndexsInitial.xml";
            string path = _path.Replace("Read", @"From1C\model.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.модели");
            List<Model> models=new List<Model>();
            foreach (XElement item in collection)
            {
                Model model=new Model();
                model.ModelTitle = item.Element("Description").Value;
                string homol =
                    Homols.FirstOrDefault(h => string.Equals(h.Element("Ref").Value, item.Element("приминимость").Value)).Value;
                model.Homol = context.HomolAttributes.FirstOrDefault(ha => string.Equals(ha.Key, homol));
                string protType =
                    ProtectorTypes.FirstOrDefault(p => string.Equals(p.Element("Ref").Value, item.Element("типпротектора").Value)).Value;
                model.ProtectorType = context.ProtectorTypes.FirstOrDefault(pt => string.Equals(pt.Title, protType));
                string season =
                    Seasons.FirstOrDefault(s => string.Equals(s.Element("Ref").Value, item.Element("сезон").Value)).Value;
                model.Season = context.Seasons.FirstOrDefault(sea => string.Equals(sea.SeasonTitle, season));
                string brandtitle =
                    Brands.FirstOrDefault(b => string.Equals(b.Element("Ref").Value, item.Element("производитель").Value)).Value;
                Brand brand = context.Brands.FirstOrDefault(br => string.Equals(br.BrandTitle, brandtitle));
                model.Brand=brand;
                brand.Models.Add(model);
            }

            context.Models.AddRange(models);
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
                //DateTime dt = DateTime.Parse(PriceDocument.SelectSingleNode("InsertDate").InnerText);
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
                        //FileName = fn,
                        //PriceLanguage = plg,
                        //Agent = aq,
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