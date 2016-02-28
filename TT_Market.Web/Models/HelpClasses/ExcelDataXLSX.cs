using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
//using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using TT_Market.Core.Domains;
using TT_Market.Web.Models;

namespace TT_Market.Web.HelpClasses
{
    public class ExcelDataXLSX
    {

        private string _path;

        private readonly string xmlFilename = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
                                              @"TT_Market.Core\DBinitial\ReadSettings.xml";

        private readonly string xmlsearchwords = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
                                                 @"TT_Market.Core\DBinitial\SearchWords.xml";

        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ExcelDataXLSX(string path)
        {
            _path = path;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public int[] GetCellMergedAttributes(XSSFSheet ws, ICell cell)
        {
            int colSpan = 1;
            int rowSpan = 1;

            if (cell.IsMergedCell)
            {
                for (int i = 0; i < ws.NumMergedRegions; i++)
                {
                    CellRangeAddress cellRangeAddress = ws.GetMergedRegion(i);
                    if (cellRangeAddress.FirstRow == cell.RowIndex && cellRangeAddress.FirstColumn == cell.ColumnIndex)
                    {
                        colSpan = cellRangeAddress.LastColumn - cellRangeAddress.FirstColumn;
                        rowSpan = cellRangeAddress.LastRow - cellRangeAddress.FirstRow;

                    }
                    bool emptyXcell = (cellRangeAddress.FirstRow < cell.RowIndex &&
                                       cellRangeAddress.LastRow >= cell.RowIndex)
                        ? true
                        : false;
                    bool emptyYcell = (cellRangeAddress.FirstColumn < cell.ColumnIndex &&
                                       cellRangeAddress.LastColumn >= cell.ColumnIndex)
                        ? true
                        : false;

                    if (emptyXcell)
                    {
                        colSpan = 0;
                    }

                    if (emptyYcell)
                    {
                        rowSpan = 0;
                    }
                }
            }
            return new[] {rowSpan, colSpan};
        }

        public void WriteColumnNamestoXml(XmlDocument xmlDoc)
        {

            var xml = XDocument.Load(xmlsearchwords);
            if (xml.Root != null)
            {
                List<string> wordcollection = xml.Root.Descendants("Word").Select(el => el.Value).ToList();

                byte[] file = File.ReadAllBytes(_path);
                MemoryStream ms = new MemoryStream(file);
                // using (ExcelPackage package = new ExcelPackage(ms))
                //{
                XSSFWorkbook wb = null;
                try
                {
                    wb = new XSSFWorkbook(ms);
                }
                catch (Exception)
                {

                    throw;
                }
                XmlElement pricenode = xmlDoc.CreateElement("Price");
                XmlElement fn_node = xmlDoc.CreateElement("FileName");
                fn_node.InnerText = _path;
                pricenode.AppendChild(fn_node);
                XmlElement ld_node = xmlDoc.CreateElement("LoadDate");
                ld_node.InnerText = DateTime.UtcNow.ToShortDateString();
                pricenode.AppendChild(ld_node);
                XmlElement wss_node = xmlDoc.CreateElement("WorkSheets");
                pricenode.AppendChild(wss_node);

                DataFormatter dataFormatter = new DataFormatter(CultureInfo.CurrentCulture);

                for (int n = 0; n < wb.NumberOfSheets; n++)
                {
                    XSSFSheet sheet = (XSSFSheet) wb.GetSheetAt(n);

                    XmlElement wsNode = xmlDoc.CreateElement("WorkSheet");
                    XmlAttribute nameAttr = xmlDoc.CreateAttribute("Agent_Title");
                    nameAttr.Value = sheet.SheetName;
                    wsNode.Attributes.Append(nameAttr);
                    wss_node.AppendChild(wsNode);

                    //int? titlerow = GetTitleRow(sheet, dataFormatter, wordcollection);

                    //if (titlerow != null)
                    //{
                    //    int i = 0;
                    //    I
                    //    while (dataFormatter.FormatCellValue(sheet.GetRow((int)titlerow).GetCell(i)) != String.Empty)
                    //    {

                    //        i++;
                    //    }
                    //}

                    //            // ... Cell by cell...
                    //            ExcelRangeBase curcell = workSheet.Cells[row, col];
                    //            string cellValue = curcell.Text; // This got me the actual value I needed.
                    //            if (wordcollection.Contains(cellValue))
                    //            {
                    //                for (int i = 1; i < GetCellMergedAttributes(curcell)[0]; i++)
                    //                {
                    //                    ExcelRangeBase titlecell = workSheet.Cells[row + i, 1];
                    //                    int[] attr = GetCellMergedAttributes(titlecell);
                    //                    if (attr != null)
                    //                    {
                    //                        XmlElement tr_node = xmlDoc.CreateElement("TitleRow");
                    //                        XmlAttribute ordnumAttr = xmlDoc.CreateAttribute("row"),
                    //                            rowspanAttr = xmlDoc.CreateAttribute("rowspan");
                    //                        ordnumAttr.Value = row.ToString();
                    //                        tr_node.Attributes.Append(ordnumAttr);
                    //                        rowspanAttr.Value = attr[0].ToString();
                    //                        wsNode.AppendChild(tr_node);
                    //                        for (int find = 1; find < end.Column; find++)
                    //                        {

                    //                            ExcelRangeBase columncell = workSheet.Cells[row + i, find];
                    //                            int[] attr_column = GetCellMergedAttributes(titlecell);
                    //                            if (attr_column != null)
                    //                            {
                    //                                XmlElement c_node = xmlDoc.CreateElement("Column");
                    //                                XmlAttribute ordNumAttr = xmlDoc.CreateAttribute("ordnumber"),
                    //                                    colspanAttr = xmlDoc.CreateAttribute("colspan");
                    //                                ordNumAttr.Value = find.ToString();
                    //                                c_node.Attributes.Append(ordNumAttr);
                    //                                colspanAttr.Value = attr_column[1].ToString();
                    //                                c_node.Attributes.Append(colspanAttr);
                    //                                tr_node.AppendChild(c_node);

                    //                                XmlElement t_node = xmlDoc.CreateElement("Title");
                    //                                t_node.InnerText = columncell.Text;
                    //                                c_node.AppendChild(t_node);

                    //                            }

                    //                        }
                    //                    }

                    //                }
                    //            }
                    //        }
                    if (xmlDoc.DocumentElement != null) xmlDoc.DocumentElement.AppendChild(pricenode);
                }
            }
        }

        public IEnumerable<Brand> GetTitleCells(XSSFSheet sh, DataFormatter dataFormatter)
        {
            //List<string> wordcollection = _db.PriceColumns.Select(c => c.Country).ToList();

            List<Brand> tcels=new List<Brand>();

            WorkSheet ws = new WorkSheet
            {
                Title = sh.SheetName
            };
            for (int row = 0; row <= 15; row++)
            {
                // Row by row...
                for (int col = 0; col <= sh.GetRow(row).Cells.Count; col++)
                {
                    ICell cell = sh.GetRow(row).GetCell(col);

                    string value = dataFormatter.FormatCellValue(cell);

                    // Max Value RowSpan in TitleCells for determine the need to take the next row for analize

                    int maxRowSpan = 1;

                    if (wordcollection.Contains(value))
                    {
                        for (int i = 0; i < sh.GetRow(row).LastCellNum; i++)
                        {
                            ICell titlecell = sh.GetRow(row).GetCell(i);
                            string titleCellValue = dataFormatter.FormatCellValue(titlecell);

                            if (!String.Equals(titleCellValue, String.Empty))
                            {
                                int[] mergAttribs = GetCellMergedAttributes(sh, titlecell);

                                maxRowSpan = mergAttribs[1] <= maxRowSpan ? maxRowSpan : mergAttribs[1];

                                Brand titleCell;
                                if (!wordcollection.Contains(value))
                                {
                                    titleCell = new Brand
                                    {
                                        WorkSheet = ws
                                    };
                                }
                                else
                                {
                                    titleCell = _db.PriceColumns.First(tc => String.Equals(tc.Country, value));
                                }
                            }
                            else
                            {
                                //TitleCells ended
                                    break;
                            }
                        }
                        //return row;
                    }
                }

            }
            return null;
        }

        public void StartDownLoad(string filename)
        {
            byte[] file = File.ReadAllBytes(filename);
            MemoryStream ms = new MemoryStream(file);
            XSSFWorkbook wb = new XSSFWorkbook(ms);

            PriceList Price = new PriceList
            {
                DownLoadDate = DateTime.UtcNow,
                FileName = filename,
                Agent = GetProvider(),
                ReadSetting = GetReadSetting(),
                WorkSheets = GetWorkSheets().ToList()
            };

        }

        public IEnumerable<WorkSheet> GetWorkSheets()
        {
            return null;
        }

        public Agent GetProvider()
        {
            return new Agent();
        }

        public ReadSetting GetReadSetting()
        {
            return new ReadSetting();
        }
    }
}
