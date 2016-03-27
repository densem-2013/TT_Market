using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TT_Market.Core.Domains;
using TT_Market.Core.Identity;

namespace TT_Market.Core.HelpClasses
{
    public static class ReadSetting
    {
        public static Dictionary<string, ReadSheetSetting> GetReadSetting(ApplicationDbContext db, JObject jobj)
        {
            Dictionary<string, ReadSheetSetting> records = new Dictionary<string, ReadSheetSetting>();


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
            db.SaveChanges();
            return records;
        }

        private static int GetRecordsFromJson(JToken sourceToken, Dictionary<string, ReadSheetSetting> targetDict)
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
                //JToken maskToken = item.SelectToken("mask");

                //string mmsk = (maskToken != null && maskToken.HasValues)
                //    ? maskToken.Value<string>("#cdata-section")
                //    : string.Empty;

                ReadCellSetting cellSetting = new ReadCellSetting
                {
                    CellNumber = (int) item["@OrderNumber"],
                    //Mask = mmsk,
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
            string groupstring = (string)tok["group"];
            string strEntity = (string)tok["type"];
            string tpString = (string)tok["property"];
            string valueString = (string)tok["value"];
            JToken maskToken = tok.SelectToken("mask");

            string mmsk = (maskToken != null && maskToken.HasValues)
                ? maskToken.Value<string>("#cdata-section")
                : string.Empty;
            int grIndex = (groupstring != null) ? groupstring.IndexOf("|", StringComparison.Ordinal) : -1;
            List<string> groups = (groupstring != null && !string.Equals(groupstring, string.Empty))
                ? ((grIndex < 0)
                    ? new List<string>(new[] { groupstring })
                    : groupstring.Split('|').ToList())
                : null;
            return new Target
            {
                Entity = strEntity,
                TypeProperty = tpString,
                Mask = mmsk,
                Groups = groups,
                Value = valueString
            };

        }
    }
}
