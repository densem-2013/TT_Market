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
using TT_Market.Web.Models;

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
            string filename = "Price160202New.xls";
            List<PriceReadSetting> plist = context.PriceReadSettings.Where(pl => string.Equals(pl.FileName, filename)).ToList();
            //List<PriceReadSetting> plist = context.Set(typeof(PriceReadSetting)).Include("Agent").Include("PriceLanguage").Cast<PriceReadSetting>().ToList();
            if (plist.Count > 0)
            {
                PriceReadSetting pset = new PriceReadSetting
                {
                    //TransformMask = @"{"ReadSettings":{"Sheets":{"Sheet":{"@Name":"Лист1","@SkipColumn":"","InsertDate":{"Row":"1","Column":"1"},"TitleRows":{"Row":[{"@RowNumber":"5","Columns":{"Column":[{"@OrderNumber":"0","@RowSpan":"1","@ColSpan":"3","Transform":null},{"@OrderNumber":"3","@RowSpan":"2","@ColSpan":"1","Transform":"{ toarray:[{\"Type\":\"Brand\",\"Property\":\"BrandTitle\"}],\"mask\":\"\"}"},{"@OrderNumber":"4","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":"\r\n                  \"targets\":[{\"Type\":\"Width\",\"Property\":\"Value\",\"Group\":\"11|1\"},{\"Type\":\"Height\",\"Property\":\"Value\",\"Group\":\"12|2\"},{\"Type\":\"Diameter\",\"Property\":\"DSize\",\"Group\":\"13|5|6\"},{\"Type\":\"SpeedIndex\",\"Property\":\"Value\",\"Group\":\"14|7|8\"},{\"Type\":\"Model\",\"Property\":\"ModelTitle\",\"Group\":\"9\"}],\r\n                    \"mask\":\"(^\\d{2,3}?)\\/(\\d{2,3}?)(((R\\d{2,3}[C]?)|(R\\d{2,3}\\.{1}\\d{1})?)\\s+(?>(\\d{2,3}[A-Z]{1})|(\\d{2,3}\\/\\d{2,3}[A-Z]{1}))\\s+(.*?)\\n)|((^\\d{2,3}?)\\/(\\d{2,3}?)(R\\d{2,3}[.,]{1}\\d{1}?)(?>(\\d{2,3}[A-Z]{1})|(\\d{2,3}\\/\\d{2,3}[A-Z]{1}))(.*?)\\n)\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized Model!!!\"},\r\n                    \"alternative\":\"\" \r\n                    "}},{"@OrderNumber":"5","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"Season\",\"Property\":\"SeasonTitle\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized Season!!!\"},\r\n                    \"alternative\":\"\" \r\n                    "}},{"@OrderNumber":"6","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"AutoType\",\"Property\":\"TypeValue\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized AutoType!!!\"},\r\n                    \"alternative\":[\"Внедорожник\":[\"4x4\"],\"Легковой\":[\"Легковые\"],\"Грузовой\":[\"Грузовые\"],\"Легкогрузовой\":[\"LT\"]]\r\n                    "}},{"@OrderNumber":"7","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"Season\",\"Property\":\"SeasonTitle\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized Season!!!\"},\r\n                    \"alternative\":[\"Всесезонные\":[\"всесез\"]]\r\n                    "}},{"@OrderNumber":"8","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"Country\",\"Property\":\"CountryTitle\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized Country!!!\"},\r\n                    \"alternative\":\"\"\r\n                    "}},{"@OrderNumber":"9","@RowSpan":"1","@ColSpan":"1","Transform":null},{"@OrderNumber":"10","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":"\r\n                    \"targets\":[{\"Type\":\"MPT\",\"Property\":\"SpecialPrice\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized SpecialPrice!!!\"},\r\n                    \"alternative\":\"\"\r\n                    "}},{"@OrderNumber":"11","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"MPT\",\"Property\":\"DiscountPrice\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized DiscountPrice!!!\"},\r\n                    \"alternative\":\"\"\r\n                    "}},{"@OrderNumber":"12","@RowSpan":"2","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"MPT\",\"Property\":\"RegularPrice\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized RegularPrice!!!\"},\r\n                    \"alternative\":\"\"\r\n                    "}}]}},{"@RowNumber":"6","Columns":{"Column":[{"@OrderNumber":"0","@RowSpan":"1","@ColSpan":"1","Transform":null},{"@OrderNumber":"1","@RowSpan":"1","@ColSpan":"1","Transform":null},{"@OrderNumber":"2","@RowSpan":"1","@ColSpan":"1","Transform":null},{"@OrderNumber":"9","@RowSpan":"1","@ColSpan":"1","Transform":{"#cdata-section":" \r\n                    \"targets\":[{\"Type\":\"MPT\",\"Property\":\"Stock\"}],\r\n                    \"mask\":\"\",\r\n                    \"norecognized\":{\"ExtendedData\":\"No recognized Stock!!!\"},\r\n                    \"alternative\":\"\"\r\n                    "}}]}}]}}}}}"';
                    // ;
                };
                JObject jobj = JObject.Parse(plist.First().TransformMask);
                foreach (string sheetname in ((JArray)jobj["ReadSettings"]["Sheets"]["Sheet"]["@Name"]).Select(c => (string)c).ToList())
                {
                    //XSSFSheet sh = (XSSFSheet)wb.GetSheet(sheetname);
                    //string dsname = data.DataSetName;
                    string sh = sheetname;
                }

            }
            //Assert.IsTrue(lists.Count > 0);
        }
    }

}
