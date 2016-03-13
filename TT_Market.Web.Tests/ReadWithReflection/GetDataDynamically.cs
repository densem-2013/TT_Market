using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TT_Market.Core.Domains;
using TT_Market.Core.Identity;

namespace TT_Market.Web.Tests.ReadWithReflection
{
    /// <summary>
    /// Summary description for GetDataDynamically
    /// </summary>
    [TestClass]
    public class GetDataDynamically
    {
        private readonly string _path =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Core\DBinitial\Read";

        private readonly ApplicationDbContext context;

        public GetDataDynamically()
        {
            context = new ApplicationDbContext();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void ReadDataFromDb()
        {
            //var type = BuildManager.GetType("TT_Market.Core.Domains.Season", false);
            //var type = Type.GetType("TT_Market.Core.Domains.Season");
            Assembly assembly = Assembly.LoadFrom("TT_Market.Core.dll");
            var type = assembly.GetType("TT_Market.Core.Domains.Season");
            context.Set(type).Load();
            var collection = context.Set(type).Local;
            //.Local.Cast<object>().ToList()
            int oldcount = collection.Count;
            var newEntity = Activator.CreateInstance(type);
            type.InvokeMember("SeasonTitle", BindingFlags.SetProperty, Type.DefaultBinder, newEntity,
                new Object[] {"Весна"});
            collection.Add(newEntity);
            context.SaveChanges();
            int newCount = collection.Count;
            Assert.IsTrue(newCount == oldcount + 1);
        }

        [TestMethod]
        public void PriceListXMLDownLoadParse()
        {

            string path = _path + "PriceSettingsInitial.xml";
            XmlDocument doc = new XmlDocument();//rss/channel/item
            doc.Load(path);
            XmlNodeList priceLists = doc.SelectNodes("descendant::PriceReadSetting");
            List<PriceReadSetting> lists = new List<PriceReadSetting>();
            foreach (XmlNode price in priceLists)
            {
                DateTime dt = DateTime.Parse(price.SelectSingleNode("InsertDate").InnerText);
                string fn = price.SelectSingleNode("FileName").InnerText;
                try
                {
                    List<PriceLanguage> plangs = context.PriceLanguages.ToList();
                    PriceLanguage plg = plangs.FirstOrDefault(
                            pl => string.Equals(pl.LanguageName, price.SelectSingleNode("PriceLanguage").InnerText));
                    Agent aq = context.Agents.ToList()
                        .FirstOrDefault(a => string.Equals(a.AgentTitle, price.SelectSingleNode("Agent").InnerText));
                    string trmask = Newtonsoft.Json.JsonConvert.SerializeXmlNode(price.SelectSingleNode("PriceReadSetting"));
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
            Assert.IsTrue(lists.Count > 0);
        }
        [TestMethod]
        public void  ParseTransformMask()
        {
            string path =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Web\App_Data\Uploads\Price160202New.xls";
            //context.Set(typeof(PriceReadSetting)).Load();
            //string filename = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            string transmask =
                "{\"ReadSettings\""+
            ":{\"Sheets\":{\"Sheet\":{\"@Name\":\"Лист1\",\"@SkipColumn\":\"\",\"InsertDate\":{\"Row\":\"1\",\"Column\":\"1\"},\"TitleRows\":{\"Row\":[{\"@RowNumber\":\"5\",\"Columns\":{\"Column\":[{\"@OrderNumber\":\"0\",\"@RowSpan\":\"1\",\"@ColSpan\":\"3\",\"Transform\":null},{\"@OrderNumber\":\"3\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"Brand\",\"property\":\"BrandTitle\",\"group\":null,\"mask\":null,\"norecognized\":null,\"alternative\":null}}}},{\"@OrderNumber\":\"4\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":[{\"Targets\":{\"target\":[{\"type\":\"Width\",\"property\":\"Value\",\"group\":\"11|1\",\"alternative\":null},{\"type\":\"Height\",\"property\":\"Value\",\"group\":\"12|2\",\"alternative\":null},{\"type\":\"Diameter\",\"property\":\"DSize\",\"group\":\"13|5|6\",\"alternative\":null},{\"type\":\"SpeedIndex\",\"property\":\"Value\",\"group\":\"14|7|8\",\"alternative\":null},{\"type\":\"Model\",\"property\":\"ModelTitle\",\"group\":\"9\",\"alternative\":null}]},\"mask\":{\"#cdata-section\":'(^\\d{2,3}?)\\/(\\d{2,3}?)(((R\\d{2,3}[C]?)|(R\\d{2,3}\\.{1}\\d{1})?)\\s+(?>(\\d{2,3}[A-Z]{1})|(\\d{2,3}\\/\\d{2,3}[A-Z]{1}))\\s+(.*?)\n)|((^\\d{2,3}?)\\/(\\d{2,3}?)(R\\d{2,3}[.,]{1}\\d{1}?)(?>(\\d{2,3}[A-Z]{1})|(\\d{2,3}\\/\\d{2,3}[A-Z]{1}))(.*?)\n)'},\"norecognized\":\"No recognized Model!!!\"},null]},{\"@OrderNumber\":\"5\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"Season\",\"property\":\"SeasonTitle\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"ExtendedData\":\"No recognized Season!!!\"}},{\"@OrderNumber\":\"6\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"AutoType\",\"property\":\"TypeValue\",\"group\":null,\"alternative\":{\"alter\":[{\"@value\":\"Внедорожник\",\"#text\":\"4x4\"},{\"@value\":\"Легковой\",\"#text\":\"Легковые\"},{\"@value\":\"Грузовой\",\"#text\":\"Грузовые\"},{\"@value\":\"Легкогрузовой\",\"#text\":\"LT\"}]}}},\"mask\":null,\"norecognized\":\"No recognized AutoType!!!\"}},{\"@OrderNumber\":\"7\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"Season\",\"property\":\"SeasonTitle\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"No recognized Season!!!\"}},{\"@OrderNumber\":\"8\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"Country\",\"property\":\"CountryTitle\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"No recognized Country!!!\"}},{\"@OrderNumber\":\"9\",\"@RowSpan\":\"1\",\"@ColSpan\":\"1\",\"Transform\":null},{\"@OrderNumber\":\"10\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"TireProposition\",\"property\":\"SpecialPrice\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"\"No recognized SpecialPrice!!!\"}},{\"@OrderNumber\":\"11\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"TireProposition\",\"property\":\"DiscountPrice\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"No recognized DiscountPrice!!!\"}},{\"@OrderNumber\":\"12\",\"@RowSpan\":\"2\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"TireProposition\",\"property\":\"RegularPrice\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"No recognized RegularPrice!!!\"}}]}},{\"@RowNumber\":\"6\",\"Columns\":{\"Column\":[{\"@OrderNumber\":\"0\",\"@RowSpan\":\"1\",\"@ColSpan\":\"1\",\"Transform\":null},{\"@OrderNumber\":\"1\",\"@RowSpan\":\"1\",\"@ColSpan\":\"1\",\"Transform\":null},{\"@OrderNumber\":\"2\",\"@RowSpan\":\"1\",\"@ColSpan\":\"1\",\"Transform\":null},{\"@OrderNumber\":\"9\",\"@RowSpan\":\"1\",\"@ColSpan\":\"1\",\"Transform\":{\"Targets\":{\"target\":{\"type\":\"TireProposition\",\"property\":\"Stock\",\"group\":null,\"alternative\":null}},\"mask\":null,\"norecognized\":\"No recognized Stock!!!\"}}]}}]}}}}}";

            string filename = "Price160202New.xls";
            //List<PriceReadSetting> plist = context.PriceReadSettings.Where(pl => string.Equals(pl.FileName, filename)).ToList();
            //List<PriceReadSetting> plist = context.Set(typeof(PriceReadSetting)).Include(\"Agent\").Include(\"PriceLanguage\").Cast<PriceReadSetting>().ToList();
            //if (plist.Count > 0)
            //{
                PriceReadSetting pset = new PriceReadSetting
                {
                    TransformMask = transmask
                };
                JObject jobj = JObject.Parse(pset.TransformMask);
                IEnumerable<string> shmames = jobj.SelectToken("ReadSettings.Sheets.Sheet.@Name").Select(st => (string)st).Where(n => n != null).ToArray();
                foreach (string sheetname in ((JArray)jobj.SelectToken("ReadSettings.Sheets.Sheet")).Select(c => (string)c).ToList())
                {
                    //XSSFSheet sh = (XSSFSheet)wb.GetSheet(sheetname);
                    //string dsname = data.DataSetName;
                    string sh = sheetname;
                }

            //}
            //Assert.IsTrue(lists.Count > 0);
        }
    }

}
