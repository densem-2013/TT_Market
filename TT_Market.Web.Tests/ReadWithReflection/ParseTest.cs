using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TT_Market.Core.DAL;
using TT_Market.Core.Domains;
using TT_Market.Core.HelpClasses;
using TT_Market.Core.Identity;

namespace TT_Market.Web.Tests.ReadWithReflection
{
    [TestClass]
    public class ParseTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PriceDocument> _priceDocsRepository;
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private static readonly string _pathXls =
            AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web.Tests\bin\Debug", "") +
            @"TT_Market.Web\App_Data\Uploads\Price160202New.xls";

        public ParseTest()
        {
            _unitOfWork = new UnitOfWork();
            _priceDocsRepository = _unitOfWork.Repository<PriceDocument>();
        }

        [TestMethod]
        public void JsonTest()
        {
            string filename = _pathXls.Substring(_pathXls.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var firstOrDefault =
                _db.PriceDocuments.Include("PriceReadSetting")
                    .FirstOrDefault(pd => string.Equals(pd.FileName, filename));
            Dictionary<string, ReadSheetSetting> records = new Dictionary<string, ReadSheetSetting>();
            if (firstOrDefault != null)
            {
                PriceReadSetting priceReadSetting = firstOrDefault.PriceReadSetting;
                if (priceReadSetting != null)
                {
                    byte[] file = File.ReadAllBytes(_pathXls);
                    MemoryStream ms = new MemoryStream(file);
                    PriceDocument priceDocument = new PriceDocument
                    {
                        DownLoadDate = DateTime.UtcNow,
                        FileName = filename
                    };

                    JObject jobj = JObject.Parse(priceReadSetting.TransformMask);


                    JToken sheetToken = jobj.SelectToken("Sheets.Sheet");
                    if (sheetToken.Type == JTokenType.Array)
                    {
                        string[] sheetNames = sheetToken.Select(sh => (string) sh.SelectToken("@Name")).ToArray();
                        foreach (JToken shToken in sheetToken.Children())
                        {
                            GetRecordsFromJson(shToken, records);
                        }
                    }
                    else
                    {
                        string sheetName = sheetToken.Value<string>("@Name");
                        GetRecordsFromJson(sheetToken, records);
                    }
                }
            }
            //Debug.Assert(records != null, "records != null");
            Assert.IsTrue(records != null && records.Count > 0);

        }

        private static int GetRecordsFromJson(JToken sourceToken,
            Dictionary<string, ReadSheetSetting> targetDict)
        {
            string sheetname = sourceToken.Value<string>("@Name");
            StartRow startRow = new StartRow
            {
                StartReadRow =
                    (int?)
                        ((sourceToken.SelectToken("TitleRow")["@RowNumber"].HasValues &&
                          sourceToken.SelectToken("TitleRow")["@RowSpan"].HasValues)
                            ? (int?) sourceToken.SelectToken("TitleRow")["@RowNumber"] +
                              (int?) sourceToken.SelectToken("TitleRow")["@RowSpan"]
                            : null),
                RewievColumn =
                    (int?)
                        ((sourceToken.SelectToken("TitleRow")["Column"].HasValues)
                            ? sourceToken.SelectToken("TitleRow")["Column"]
                            : null),
                TitleRowPattern = (string) sourceToken.SelectToken("TitleRow")["Condition"]
            };

            EndRow endRow = new EndRow
            {
                RewievColumn =
                    (int?)
                        ((sourceToken["EndRow"].SelectToken("Column").HasValues)
                            ? sourceToken["EndRow"].SelectToken("Column")
                            : null),
                StopReadPattern = (string) sourceToken.SelectToken("EndRow")["Condition"]
            };
            ReadSheetSetting readSeting = new ReadSheetSetting
            {
                SheetName = sheetname,
                StartRow = startRow,
                EndRow = endRow
            };

            var columnToken = sourceToken.SelectToken("Columns.Column").Children();
            List<ReadCellSetting> cellSettings = new List<ReadCellSetting>();
            foreach (JToken item in columnToken)
            {
                JToken targetToken = item.SelectToken("Targets.target");
                List<Target> targets = new List<Target>();
                if (targetToken.Type == JTokenType.Array)
                {
                    targets.AddRange(targetToken.Children().Select(ParseTarget));
                }
                else
                {
                    targets.Add(ParseTarget(targetToken));
                }
                JToken maskToken = item.SelectToken("mask");

                string mmsk = (maskToken != null && maskToken.HasValues)
                    ? maskToken.Value<string>("#cdata-section")
                    : string.Empty;

                ReadCellSetting cellSetting = new ReadCellSetting
                {
                    CellNumber = (int) item["@OrderNumber"],
                    Mask = mmsk,
                    Targets = targets
                };
                cellSettings.Add(cellSetting);
            }
            readSeting.ReadCellSettings = cellSettings;

            targetDict.Add(sheetname, readSeting);
            return targetDict.Count;
        }

        private static Target ParseTarget(JToken tok)
        {
            string groupstring = (string) tok["group"];
            JToken alterToken = tok["alternative"];
            Dictionary<string, string> alterDict = new Dictionary<string, string>();
            if (alterToken.HasValues)
            {
                foreach (JToken alt in alterToken.SelectToken("alter").Children())
                {
                    alterDict.Add(alt["@value"].Value<string>(), alt["#text"].Value<string>());
                }
            }
            string strEntity = (string) tok["type"];
            string tpString = (string) tok["property"];
            return new Target
            {
                Entity = strEntity,
                TypeProperty = tpString,
                GroupNumbers =
                    (!string.Equals(groupstring, string.Empty))
                        ? groupstring.Split('|').Select(int.Parse).ToList()
                        : null,
                AlterValue = alterDict
            };

        }
    }
}
