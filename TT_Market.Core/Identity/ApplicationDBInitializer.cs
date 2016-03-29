using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
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
            LoadLanguagesFromXml(path, context);
            LoadCountryFromXml(path,context);
            LoadCitiesFromXml(path,context);
            LoadAgentsFromXml(path, context);
            LoadAutoTypesFromXml(path, context);

            LoadCurrencyFromXml(path, context);
            LoadDiametersFromXml(path, context);
            LoadHeightsFromXml(path, context);

            LoadWidthsFromXml(path, context);
            LoadSpeedIndexesFromXml(path, context);
            LoadPressIndexesFromXml(path, context);

            LoadConvSignsFromXml(path, context);

            LoadModelFromXML(path, context);

            //LoadPriceDocInitials(path, context);
            LoadPricesReadSettingsFromXml(path, context);

            LoadHomolAttriFromXml(path, context);

            AddStartUserIdentityCridentials(context);
        }


        public static void LoadCitiesFromXml(string path, ApplicationDbContext context)
        {

            path = path + "CitiesInitial.xml";
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("City");
            foreach (XElement item in collection)
            {
                List<CityTitle> cityTitles = item.Elements("Lang").ToList().Select(lang => new CityTitle
                {
                    Title = lang.Value,
                    PriceLanguage =
                        context.PriceLanguages.ToList().FirstOrDefault(
                            pl => string.Equals(pl.LanguageName, lang.Attribute("Name").Value))
                }).ToList();
                //Country country =
                //    context.Countrys.ToList().FirstOrDefault(
                //        c => c.CountryTitles.Any(ct => string.Equals(ct.Title, item.Attribute("Country").Value)));
                City city = new City
                {
                    //Country = country,
                    CityTitles = cityTitles
                };
                context.Citys.Add(city);
                //if (country != null && country.Cities == null)
                //{
                //    country.Cities = new List<City>();
                //}
                //if (country != null)
                //    if (!country.Cities.Contains(city))
                //    {
                //        country.Cities.Add(city);
                //    }

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
                            Phone = (phoneElement!=null)?phoneElement.Value:null,
                            Email = (emailElement!=null)?emailElement.Value:null
                        };
                return null;
            }).ToArray();

            context.Agents.AddRange(agents);
            context.SaveChanges();

        }

        public static Dictionary<string, AutoType> LoadAutoTypesFromXml(string path, ApplicationDbContext context)
        {

            path = path.Replace("Read", @"From1C\for.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.приминимость");
            var loadAutoTypesFromXml = collection as XElement[] ?? collection.ToArray();
            Dictionary<string, AutoType> atsDictionary = new Dictionary<string, AutoType>();
            AutoType[] autotypes = loadAutoTypesFromXml.ToList().Select(a =>
            {
                var element = a.Element("Description");
                var altel = a.Elements("alterkey");
                List<AutoTypeAlter> alters = null;
                if (altel.Any())
                {
                    alters=new List<AutoTypeAlter>();
                    foreach (XElement item in altel)
                    {
                        alters.Add(new AutoTypeAlter
                        {
                            AlterValue = item.Value
                        });
                    }

                }
                AutoType atype = null;
                if (element != null)
                {
                    atype = new AutoType
                    {
                        TypeValue = element.Value,
                        AutoTypeAlters = alters
                    };
                    var xElement = a.Element("Ref");
                    if (xElement != null) atsDictionary.Add(xElement.Value,atype);
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
            Country[] countries = collection.ToList().Select(a =>
            {
                List<CountryTitle> countryTitles = a.Elements("Lang").ToList().Select(lang => new CountryTitle
                {
                    Title = lang.Value,
                    PriceLanguage = context.PriceLanguages.ToList().FirstOrDefault(pl=>string.Equals(pl.LanguageName,lang.Attribute("Name").Value))
                }).ToList();
                return new Country
                {
                    CountryTitles = countryTitles
                };
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
                IsDefault = bool.Parse(a.Attribute("isdefault").Value),
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
                IsDefault = bool.Parse(a.Attribute("isdefault").Value),
                LanguageName = a.Value
            }).ToArray();

            context.PriceLanguages.AddRange(priceLanguages);
            context.SaveChanges();
        }

        public static Dictionary<string,Season> LoadSeasonsFromXml(string path, ApplicationDbContext context)
        {
            path = path.Replace("Read", @"From1C\season.xml");
            var xml = XDocument.Load(path);
            var collection = xml.Root.Descendants("CatalogObject.Сезон");
            var loadSeasonsFromXml = collection as XElement[] ?? collection.ToArray();
            Dictionary<string, Season> seaDictionary = new Dictionary<string, Season>();
            Season[] seasons = loadSeasonsFromXml.Select(a =>
            {
                var altel = a.Elements("alterkey");
                List<SeasonTitleAlter> alters = null;
                if (altel.Any())
                {
                    alters = new List<SeasonTitleAlter>();
                    foreach (XElement item in altel)
                    {
                        alters.Add(new SeasonTitleAlter
                        {
                            TitleAlterValue = item.Value
                        });
                    }

                }
                PriceLanguage langdefault = context.PriceLanguages.ToList().FirstOrDefault(pl => pl.IsDefault);
                List<SeasonTitle> sesSeasonTitles=new List<SeasonTitle>();

                var element = a.Element("Description");
                if (element != null)
                    sesSeasonTitles.Add(new SeasonTitle
                    {
                        Title = element.Value,
                        PriceLanguage = langdefault,
                        SeasonTitleAlters = alters
                        
                    });

                sesSeasonTitles.AddRange(from langelem in a.Elements("Lang")
                    let lang = context.PriceLanguages.ToList().FirstOrDefault(pl => string.Equals(langelem.Attribute("Name").Value, pl.LanguageName))
                    select new SeasonTitle
                    {
                        Title = langelem.Value, PriceLanguage = lang
                    });

                Season season = new Season
                {
                    SeasonTitles = sesSeasonTitles
                };
                var xElement = a.Element("Ref");
                if (xElement != null) seaDictionary.Add(xElement.Value,season);
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
            ConvSign[] convindexes = loadConvSignsFromXml.ToList().Select(a =>
            {
                var altel = a.Elements("alterkey");
                List<ConvAlter> alters = null;
                var xElements = altel as XElement[] ?? altel.ToArray();
                if (xElements.Any())
                {
                    alters = xElements.Select(item => new ConvAlter
                    {
                        AlterKey = item.Value
                    }).ToList();
                }
                ConvSign conv = null;
                var xElement = a.Element("Description");
                if (xElement != null)
                {
                    var convSign = a.Element("Значение");
                    if (convSign != null)
                        conv = new ConvSign
                        {

                            Key = xElement.Value,
                            Value = convSign.Value,
                            ConvAlters = alters
                        };
                }
                return conv;
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
            List<string> exsists=new List<string>();
            //exsists = homls.Where(hk=>hk.Key!=String.Empty).Select(h => h.Key).ToList();
            for (int i = 0; i < homls.Length; i++)
            {
                string key = homls.ElementAt(i).Key;
                exsists.Add(key);
            }

            try
            {
                addhomls = addhomls.Where(hom => !exsists.Contains(hom.Key)).Concat(homls).OrderBy(ha => ha.Key).ToArray();

            }
            catch (Exception)
            {
                    
                throw;
            }

            context.HomolAttributes.AddRange(addhomls);
            context.SaveChanges();
            //return refs;
        }

        public static void LoadModelFromXML(string path, ApplicationDbContext context)
        {
            Dictionary<string, string> Brands = LoadBrandsFromXml(path, context);
            Dictionary<string, Season> Seasons = LoadSeasonsFromXml(path, context);
            Dictionary<string, string> ProtectorTypes = LoadProtectorTypeFromXml(path, context);
            Dictionary<string, AutoType> autotypes = LoadAutoTypesFromXml(path, context);

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
                    AutoType autoType = autotypes.FirstOrDefault(at => string.Equals(at.Key, atypeatr.Value)).Value;
                    model.AutoType = autoType;

                    var ptype = item.Element("типпротектора");
                    string prottype =
                        ProtectorTypes.FirstOrDefault(pt => ptype != null && string.Equals(pt.Key, ptype.Value)).Value;
                    if (prottype != String.Empty)
                    {
                        model.ProtectorType = protectorTypes.FirstOrDefault(pt => string.Equals(pt.Title, prottype));
                    }

                    var seas = item.Element("сезон");
                    Season season = Seasons.FirstOrDefault(s => string.Equals(s.Key, seas.Value)).Value;
                    model.Season = season;

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

        //public static void LoadPriceDocInitials(string path, ApplicationDbContext context)
        //{

        //    path = path + "PriceDocumentsInitial.xml";
        //    var xml = XDocument.Load(path);
        //    var collection = xml.Root.Descendants("PriceDocument");
        //    List<Agent> agents = context.Agents.ToList();
        //    List<PriceLanguage> priceLanguages = context.PriceLanguages.ToList();
        //    var pdlist = (from item in collection
        //                  let xElement = item.Element("Filename")
        //                  where xElement != null
        //                  let element = item.Element("Agent")
        //                  where element != null
        //                  let xElement1 = item.Element("Language")
        //                  where xElement1 != null
        //                  select new
        //                  {
        //                      FileName = xElement.Value,
        //                      Language = xElement1.Value,
        //                      Agent = element.Value
        //                  }).ToList();
        //    List<PriceDocument> pds = pdlist.Select(x => new PriceDocument
        //    {
        //        DownLoadDate = DateTime.Now,
        //        FileName = x.FileName,
        //        Agent = agents.FirstOrDefault(a => string.Equals(a.AgentTitle, x.Agent)),
        //        PriceLanguage =
        //            priceLanguages.ToList().FirstOrDefault(pl => string.Equals(pl.LanguageName, x.Language)) ??
        //            priceLanguages.ToList().FirstOrDefault(pl => pl.IsDefault)
        //    }).ToList();
        //    context.PriceDocuments.AddRange(pds);
        //    context.SaveChanges();
        //}
        public static void LoadPricesReadSettingsFromXml(string path, ApplicationDbContext context)
        {
            List<string> filenames = Directory.EnumerateFiles(path.Replace(@"DBinitial\Read", @"DocReadSettings")).ToList();
            List<PriceReadSetting> plist=new List<PriceReadSetting>();
            foreach (string file in filenames)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);
                XmlNode priceList = doc.DocumentElement;
                var selectSingleNode = priceList.SelectSingleNode("FileName");
                if (selectSingleNode != null)
                {
                    string fn = selectSingleNode.InnerText;

                    XmlNode sheets = priceList.SelectSingleNode("Sheets");
                    var singleNode = priceList.SelectSingleNode("Agent");
                    if (singleNode != null)
                    {
                        string agent = singleNode.InnerText;
                        XmlNode lang = priceList.SelectSingleNode("PriceLanguage");
                        string trmask = JsonConvert.SerializeXmlNode(sheets);
                        List<Agent> agents = context.Agents.ToList();
                        PriceReadSetting pricers = new PriceReadSetting
                        {
                            FileName = fn,
                            PriceLanguage = context.PriceLanguages.ToList().FirstOrDefault(pl => lang != null && string.Equals(pl.LanguageName, lang.InnerText)),
                            Agent = agents.FirstOrDefault(a => string.Equals(a.AgentTitle, agent)),
                            TransformMask = trmask
                        };
                        plist.Add(pricers);
                        //context.PriceReadSettings.AddOrUpdate(prs => prs.FileName, pricers);
                    }
                }
            }
            context.PriceReadSettings.AddRange(plist);
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