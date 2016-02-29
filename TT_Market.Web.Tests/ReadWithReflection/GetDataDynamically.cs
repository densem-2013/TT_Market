using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }

}
