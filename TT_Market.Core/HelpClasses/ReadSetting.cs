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
        public static Dictionary<string, ReadSheetSetting> GetReadSetting( JObject jobj)
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
            //db.SaveChanges();
            return records;
        }

        private static int GetRecordsFromJson(JToken sourceToken, Dictionary<string, ReadSheetSetting> targetDict)
        {
            string sheetname = sourceToken.Value<string>("@Name");

            bool rnhasVal = sourceToken.SelectToken("TitleRow")["@RowNumber"] != null;
            bool rsHasVal = sourceToken.SelectToken("TitleRow")["@RowSpan"] != null;
            int? rn = (int?)sourceToken.SelectToken("TitleRow")["@RowNumber"];
            int? rs = (int?)sourceToken.SelectToken("TitleRow")["@RowSpan"];
            string trpat = sourceToken.SelectToken("TitleRow.Condition").Value<string>("#text");
            StartRow startRow = new StartRow
            {
                TitleRow =  rn,
                TitleRowSpan = rs,
                RewievColumn =
                    (int?)
                        (sourceToken.SelectToken("TitleRow")["Column"] ?? null),
                TitleRowPattern = trpat
            };
            var ercol = sourceToken.SelectToken("EndRow")["Column"].HasValues;
            EndRow endRow = new EndRow
            {
                RewievColumn = (int?) (sourceToken.SelectToken("EndRow")["Column"]),
                StopReadPattern =
                    (sourceToken.SelectToken("EndRow")["Condition"] != null)
                        ? sourceToken.SelectToken("EndRow")["Condition"].Value<string>("#cdata-section")
                        : null
            };
            ReadSheetSetting readSeting = new ReadSheetSetting
            {
                SheetName = sheetname,
                StartRow = startRow,
                SkipColumn = (int?) sourceToken["SkipColumn"],
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
                ReadCellSetting cellSetting = new ReadCellSetting
                {
                    CellNumber = (int)item["@OrderNumber"],
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
