using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using TT_Market.Core.Domains;

namespace TT_Market.Core.Identity
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext context)
        {
            string _path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Core\bin\Debug", "") + @"TT_Market.Core\DBinitial\Read";
            InitializeIdentityForEf(_path,context);
            base.Seed(context);
        }

        private static readonly Func<string, double> parseDouble = str =>
        {
            double num = Double.Parse(str);
            return num;
        };

        public static void InitializeIdentityForEf(string path, ApplicationDbContext context)
        
        {
            LoadCountryFromXml(path,context);
            LoadCitiesFromXml(path,context);
            LoadAgentsFromXml(path, context);
            LoadAutoTypesFromXml(path, context);

            LoadCurrencyFromXml(path, context);
            LoadDiametersFromXml(path, context);
            LoadHeightsFromXml(path, context);
            LoadLanguagesFromXml(path, context);

            LoadWidthsFromXml(path, context);
            LoadSpeedIndexesFromXml(path, context);
            LoadPressIndexesFromXml(path, context);

            LoadConvSignsFromXml(path, context);

            LoadModelFromXML(path, context);

            LoadPriceDocInitials(path, context);
            LoadPricesReadSettingsFromXml(path, context);

            AddStartUserIdentityCridentials(context);
        }

        public static void LoadPriceDocInitials(string path, ApplicationDbContext context)
        {

            path = path + "PriceDocumentsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("PriceDocument");
            List<Agent> agents = context.Agents.ToList();
            List<PriceLanguage> priceLanguages = context.PriceLanguages.ToList();
            var pdlist= (from item in collection
                let xElement = item.Element("Filename")
                where xElement != null
                select new 
                {
                    FileName = xElement.Value,
                    Language=item.Element("Language").Value,
                    Agent=item.Element("Agent").Value
                }).ToList();
            List<PriceDocument> pds=pdlist.Select(x =>new PriceDocument
            {
                DownLoadDate = DateTime.Now,
                FileName = x.FileName,
                Agent = agents.FirstOrDefault(a=>string.Equals(a.AgentTitle,x.Agent)),
                PriceLanguage = priceLanguages.FirstOrDefault(pl=>string.Equals(pl.LanguageName,x.Language))
            }).ToList();
            context.PriceDocuments.AddRange(pds);
            context.SaveChanges();
        }
        public static void LoadCitiesFromXml(string path, ApplicationDbContext context)
        {

            path = path + "CitiesInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("City");
            foreach (XElement item in collection)
            {
                City city = context.Citys.FirstOrDefault(c => string.Equals(c.CityTitle, item.Value));
                if (city == null)
                {
                    Country country =
                        context.Countrys.ToList().FirstOrDefault(
                            c => string.Equals(c.CountryTitle, item.Attribute("Country").Value));
                    city = new City
                    {
                        CityTitle = item.Value,
                        Country = country
                    };
                    context.Citys.Add(city);
                    if (country != null && country.Cities == null)
                    {
                        country.Cities = new List<City>();
                    }
                    if (country != null)
                        if (!country.Cities.Contains(city))
                        {
                            country.Cities.Add(city);
                        }
                }
            }
            context.SaveChanges();
        }

        public static void LoadAgentsFromXml(string path, ApplicationDbContext context)
        {
            path = path + "AgentsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Agent");
            Agent[] agents = collection.ToList().Select(a =>
            {
                var phoneElement = a.Element("Phone");
                var emailElement = a.Element("Email");
                var cityelement = a.Element("City");
                    var xElement1 = a.Element("Title");
                    if (xElement1 != null)
                        return new Agent
                        {
                            AgentTitle = xElement1.Value,
                            City = context.Citys.ToList().FirstOrDefault(c => cityelement != null && string.Equals(c.CityTitle, cityelement.Value)),
                            Phone = (phoneElement!=null)?phoneElement.Value:null,
                            Email = (emailElement!=null)?emailElement.Value:null
                        };
                return null;
            }).ToArray();

            context.Agents.AddRange(agents);
            context.SaveChanges();

        }

        public static Dictionary<string, string> LoadAutoTypesFromXml(string path, ApplicationDbContext context)
        {

            path = path.Replace("Read", @"From1C\for.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.приминимость");
            var loadAutoTypesFromXml = collection as XElement[] ?? collection.ToArray();
            Dictionary<string,string> atsDictionary=new Dictionary<string, string>();
            AutoType[] autotypes = loadAutoTypesFromXml.ToList().Select(a =>
            {
                var element = a.Element("Description");
                AutoType atype = null;
                if (element != null)
                {
                    atype = new AutoType
                    {
                        TypeValue = element.Value
                    };
                    var xElement = a.Element("Ref");
                    if (xElement != null) atsDictionary.Add(xElement.Value,atype.TypeValue);
                }
                return atype;
            }).ToArray();
            context.AutoTypes.AddRange(autotypes.Distinct());
            context.SaveChanges();
            return atsDictionary;
        }

        public static Dictionary<string, string> LoadBrandsFromXml(string path, ApplicationDbContext context)
        {

            path = path.Replace("Read", @"From1C\brand.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.производители");
            var loadBrandsFromXml = collection as XElement[] ?? collection.ToArray();
            Dictionary<string,string> branDictionary=new Dictionary<string, string>();
            Brand[] brands = loadBrandsFromXml.ToList().Select(a =>
            {
                Brand brand = new Brand
                {
                    BrandTitle = a.Element("Description").Value
                };
                branDictionary.Add(a.Element("Ref").Value,brand.BrandTitle);
                return brand;
            }).ToArray();

            context.Brands.AddRange(brands);
            context.SaveChanges();
            return branDictionary;
        }

        public static void LoadCountryFromXml(string path, ApplicationDbContext context)
        {
            path = path + "CountrysInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Country");
            Country[] countries = collection.ToList().Select(a => new Country
            {
                CountryTitle = a.Value
            }).ToArray();

            context.Countrys.AddRange(countries);
            context.SaveChanges();
        }

        public static void LoadCurrencyFromXml(string path, ApplicationDbContext context)
        {
            path = path + "CurrencysInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Currency");
            Currency[] currencies = collection.ToList().Select(a => new Currency
            {
                CurrencyTitle = a.Value
            }).ToArray();

            context.Currencys.AddRange(currencies);
            context.SaveChanges();
        }

        public static void LoadDiametersFromXml(string path, ApplicationDbContext context)
        {
            path = path + "DiametersInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Diameter");
            Diameter[] diameters = collection.ToList().Select(a => new Diameter
            {
                DSize = a.Value
            }).ToArray();

            context.Diameters.AddRange(diameters);
            context.SaveChanges();
        }

        public static void LoadHeightsFromXml(string path, ApplicationDbContext context)
        {

            path = path + "HeightsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Height");
            Height[] heights = collection.ToList().Select(a => new Height
            {
                Value = parseDouble(a.Value)
            }).ToArray();

            context.Heights.AddRange(heights);
            context.SaveChanges();
        }

        public static void LoadLanguagesFromXml(string path, ApplicationDbContext context)
        {
            path = path + "LanguagesInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Language");
            PriceLanguage[] priceLanguages = collection.ToList().Select(a => new PriceLanguage
            {
                LanguageName = a.Value
            }).ToArray();

            context.PriceLanguages.AddRange(priceLanguages);
            context.SaveChanges();
        }

        public static Dictionary<string,string> LoadSeasonsFromXml(string path, ApplicationDbContext context)
        {
            path = path.Replace("Read", @"From1C\season.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.Сезон");
            var loadSeasonsFromXml = collection as XElement[] ?? collection.ToArray();
            Dictionary<string,string> seaDictionary=new Dictionary<string, string>();
            Season[] seasons = loadSeasonsFromXml.ToList().Select(a =>
            {
                Season season = new Season
                {
                    SeasonTitle = a.Element("Description").Value
                };
                var xElement = a.Element("Ref");
                if (xElement != null) seaDictionary.Add(xElement.Value,season.SeasonTitle);
                return season;
            }).ToArray();

            context.Seasons.AddRange(seasons);
            context.SaveChanges();
            return seaDictionary;
        }

        public static void LoadWidthsFromXml(string path, ApplicationDbContext context)
        {
            path = path + "WidthsInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("Width");
            Width[] widths = collection.ToList().Select(a => new Width
            {
                Value = parseDouble(a.Value)
            }).ToArray();

            context.Widths.AddRange(widths);
            context.SaveChanges();
        }

        public static void LoadSpeedIndexesFromXml(string path, ApplicationDbContext context)
        {
            path = path.Replace("Read", @"From1C\speed.xml");
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

        public static void LoadPressIndexesFromXml(string path, ApplicationDbContext context)
        {
            path = path.Replace("Read", @"From1C\press.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.ИндексНагрузки");
            PressIndex[] prindexes = collection.ToList().Select(a =>
            {
                var xElement = a.Element("Description");
                if (xElement != null)
                {
                    return new PressIndex
                    {
                        Key = xElement.Value,
                        Value = a.Element("Значение").Value
                    };
                }
                return null;
            }).ToArray();

            context.PressIndexs.AddRange(prindexes);
            context.SaveChanges();
        }

        public static Dictionary<string,string> LoadProtectorTypeFromXml(string path, ApplicationDbContext context)
        {
            //path = path + "SpeedIndexsInitial.xml";
            path = path.Replace("Read", @"From1C\type.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.Типпротектора");
            var loadProtectorTypeFromXml = collection as XElement[] ?? collection.ToArray();
            Dictionary<string,string> prots=new Dictionary<string, string>();
            ProtectorType[] prottypes = loadProtectorTypeFromXml.ToList().Select(a =>
            {
                ProtectorType prottype = new ProtectorType
                {
                    Title = a.Element("Description").Value
                };
                prots.Add(a.Element("Ref").Value, prottype.Title);
                return prottype;
            }).ToArray();

            context.ProtectorTypes.AddRange(prottypes);
            context.SaveChanges();
            return prots;
        }

        public static IEnumerable<XElement> LoadConvSignsFromXml(string path, ApplicationDbContext context)
        {
            //path = path + "SpeedIndexsInitial.xml";
            path = path.Replace("Read", @"From1C\uslov.xml");
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

        public static void LoadHomolAttriFromXml(string path, ApplicationDbContext context)
        {
            //path = path + "SpeedIndexsInitial.xml";
            path = path.Replace("Read", @"From1C\omolog.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.Омологации");
            var loadHomolAttriFromXml = collection as XElement[] ?? collection.ToArray();
            //Dictionary<string, string> refs = new Dictionary<string, string>();
            HomolAttribute[] homls = loadHomolAttriFromXml.ToList().Select(a =>
            {
                var xElement = a.Element("Значение");
                HomolAttribute newhomol = null;
                if (xElement != null)
                {
                    var element = a.Element("Description");
                    if (element != null)
                    {
                        newhomol = new HomolAttribute
                        {
                            Key = element.Value,
                            Value = xElement.Value
                        };
                        //var xElement1 = a.Element("Ref");
                        //if (xElement1 != null) refs.Add(xElement1.Value, newhomol.Key);
                    }
                }
                return newhomol;
            }).ToArray();

            path = path.Replace(@"From1C\omolog.xml", "ReadHomolAttrbutesInitial.xml");

            xml = XDocument.Load(path);
            collection = xml.Root.Descendants("HomolAttribute");
            loadHomolAttriFromXml = collection as XElement[] ?? collection.ToArray();
            HomolAttribute[] addhomls = loadHomolAttriFromXml.ToList().Select(a =>
            {
                var xElement = a.Element("Description");
                if (xElement != null)
                {
                    var element = a.Element("Value");
                    if (element != null)
                        return new HomolAttribute
                        {
                            Key = element.Value,
                            Value = xElement.Value
                        };
                }
                return null;
            }).ToArray();
            IEnumerable<string> exsists = homls.Select(h => h.Key);
            addhomls = addhomls.Where(hom => !exsists.Contains(hom.Key)).Concat(homls).OrderBy(ha => ha.Key).ToArray();

            context.HomolAttributes.AddRange(addhomls);
            context.SaveChanges();
            //return refs;
        }

        public static void LoadModelFromXML(string path, ApplicationDbContext context)
        {
            Dictionary<string, string> Brands = LoadBrandsFromXml(path, context);
            Dictionary<string, string> Seasons = LoadSeasonsFromXml(path, context);
            Dictionary<string, string> ProtectorTypes = LoadProtectorTypeFromXml(path, context);
            Dictionary<string, string> autotypes = LoadAutoTypesFromXml(path, context);

            //path = path + "SpeedIndexsInitial.xml";
            path = path.Replace("Read", @"From1C\model.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.модели");
            List<Model> models = new List<Model>();
            List<AutoType> autoTypes = context.AutoTypes.ToList();
            List<ProtectorType> protectorTypes = context.ProtectorTypes.ToList();
            List<Season> seasons = context.Seasons.ToList();
            List<Brand> brands = context.Brands.ToList();

            foreach (XElement item in collection)
            {
                Model model = new Model();
                var xElement = item.Element("Description");
                if (xElement != null)
                {
                    model.ModelTitle = xElement.Value;

                    var atypeatr = item.Element("приминимость");
                    string autotype =
                        autotypes.FirstOrDefault(h => atypeatr != null && string.Equals(h.Key, atypeatr.Value)).Value;

                    if (autotype != String.Empty)
                    {
                        model.AutoType = autoTypes.FirstOrDefault(at => string.Equals(at.TypeValue, autotype));
                    }

                    var ptype = item.Element("типпротектора");
                    string prottype = ProtectorTypes.FirstOrDefault(pt => ptype != null && string.Equals(pt.Key, ptype.Value)).Value;
                    if (prottype != String.Empty)
                    {
                        model.ProtectorType = protectorTypes.FirstOrDefault(pt => string.Equals(pt.Title, prottype));
                    }

                    var seas = item.Element("сезон");
                    string season = Seasons.FirstOrDefault(s => string.Equals(s.Key, seas.Value)).Value;
                    if (season != String.Empty)
                    {
                        model.Season = seasons.FirstOrDefault(sea => string.Equals(sea.SeasonTitle, season));
                    }

                    var branditem = item.Element("производитель");
                    string brandtitle = Brands.FirstOrDefault(b => string.Equals(b.Key, branditem.Value)).Value;
                    if (brandtitle != String.Empty)
                    {
                        Brand brand = brands.FirstOrDefault(br => string.Equals(br.BrandTitle, brandtitle));
                        if (brand != null)
                        {
                            model.Brand = brand;
                            if (brand.Models == null)
                            {
                                brand.Models = new List<Model>();
                            }
                            brand.Models.Add(model);
                        }
                    }
                }
                models.Add(model);
            }

            context.Models.AddRange(models);
            context.SaveChanges();
        }

        public static void LoadPricesReadSettingsFromXml(string path, ApplicationDbContext context)
        {

            path = path + "PriceReadSettingsInitial.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList priceLists = doc.SelectNodes("descendant::PriceReadSetting");
            List<PriceReadSetting> lists = new List<PriceReadSetting>();
            foreach (XmlNode readset in priceLists)
            {

                var selectSingleNode = readset.SelectSingleNode("FileName");
                if (selectSingleNode != null)
                {
                    string fn = selectSingleNode.InnerText;
                    try
                    {
                        string trmask = JsonConvert.SerializeXmlNode(readset);
                        PriceReadSetting pricers = new PriceReadSetting
                        {
                            TransformMask = trmask
                        };
                        PriceDocument pdoc = context.PriceDocuments.FirstOrDefault(pd => string.Equals(pd.FileName, fn));
                        if (pricers.PriceDocuments==null)
                        {
                            pricers.PriceDocuments=new List<PriceDocument>();
                        }
                        pricers.PriceDocuments.Add(pdoc);
                        lists.Add(pricers);

                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
                }
            }
            context.PriceReadSettings.AddRange(lists);
            context.SaveChanges();
        }

        public static void AddStartUserIdentityCridentials(ApplicationDbContext context)
        {

            var userManager = new ApplicationUserManager(new UserStore<User>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("Administrator"))
            {
                roleManager.Create(new UserRole
                {
                    Name = "Administrator",
                    Description = "All Rights in Application"
                });
            }
            if (!roleManager.RoleExists("Provider"))
            {
                roleManager.Create(new UserRole
                {
                    Name = "Provider",
                    Description = "Have Right Uploat PriceDocuments to the System"
                });
            }
            if (!roleManager.RoleExists("RegisteredUser"))
            {
                roleManager.Create(new UserRole
                {
                    Name = "RegisteredUser",
                    Description = "Have Right Review Uploaded PraceDocuments from frontend"
                });
            }
        }
    }
}