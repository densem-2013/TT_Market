using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Excel;
using OfficeOpenXml;

namespace TT_Market.Core.HelpClasses
{
    public class FileImport
    {
        private string _path;

        private readonly string xmlFilename = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
                                              @"TT_Market.Core\DBinitial\ReadSettings.xml";

        private readonly string xmlsearchwords = AppDomain.CurrentDomain.BaseDirectory.Replace(@"TT_Market.Web", "") +
                                                 @"TT_Market.Core\DBinitial\SearchWords.xml";

        public FileImport(string path)
        {
            _path = path;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Take Rowspan and Colspan attributes fot given cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns></returns>
        public int[] GetCellMergedAttributes(ExcelRangeBase currentCell)
        {
            int colSpan = 1;
            int rowSpan = 1;

            //check if this is the start of a merged cell
            ExcelAddress cellAddress = new ExcelAddress(currentCell.Address);

            var mCellsResult = (from c in currentCell.Worksheet.MergedCells
                                let addr = new ExcelAddress(c)
                                where cellAddress.Start.Row >= addr.Start.Row &&
                                      cellAddress.End.Row <= addr.End.Row &&
                                      cellAddress.Start.Column >= addr.Start.Column &&
                                      cellAddress.End.Column <= addr.End.Column
                                select addr);

            if (mCellsResult.Count() > 0)
            {
                var mCells = mCellsResult.First();

                //if the cell and the merged cell do not share a common start address then skip this cell as it's already been covered by a previous item
                if (mCells.Start.Address != cellAddress.Start.Address)
                    return null;

                if (mCells.Start.Column != mCells.End.Column)
                {
                    colSpan += mCells.End.Column - mCells.Start.Column;
                }

                if (mCells.Start.Row != mCells.End.Row)
                {
                    rowSpan += mCells.End.Row - mCells.Start.Row;
                }
            }
            return new[] { rowSpan, colSpan };
        }

        public void WriteXML()
        {

            XmlDocument xmlDoc = new XmlDocument();

            if (File.Exists(xmlFilename))
            {
                try
                {

                    xmlDoc.LoadXml(xmlFilename);
                }
                catch (Exception)
                {

                    throw;
                }
                XmlNodeList fileNameNodes = xmlDoc.SelectNodes("/Prices/Price/FileName");
                bool hasfname = false;
                if (fileNameNodes != null)
                    foreach (XmlNode node in fileNameNodes)
                    {
                        if (string.Equals(node.InnerText, _path))
                        {
                            hasfname = true;
                        }
                        if (!hasfname)
                        {
                            WriteColumnNamestoXml(xmlDoc);
                        }
                    }
                else
                {
                    WriteColumnNamestoXml(xmlDoc);
                }
            }
            else
            {
                //(1) the xml declaration is recommended, but not mandatory
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);

                //(2) string.Empty makes cleaner code
                XmlElement element1 = xmlDoc.CreateElement(string.Empty, "Prices", string.Empty);
                xmlDoc.AppendChild(element1);

                WriteColumnNamestoXml(xmlDoc);

            }
            xmlDoc.Save(xmlFilename);
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
                ExcelPackage package = null;
                try
                {
                    package = new ExcelPackage(ms);
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

                foreach (ExcelWorksheet workSheet in package.Workbook.Worksheets)
                {
                    XmlElement ws_node = xmlDoc.CreateElement("WorkSheet");
                    XmlAttribute NameAttr = xmlDoc.CreateAttribute("Agent_Title");
                    NameAttr.Value = workSheet.Name;
                    ws_node.Attributes.Append(NameAttr);
                    wss_node.AppendChild(ws_node);

                    var start = workSheet.Dimension.Start;
                    var end = workSheet.Dimension.End;
                    for (int row = start.Row; row <= 15; row++)
                    {
                        // Row by row...
                        for (int col = start.Column; col <= end.Column; col++)
                        {
                            // ... Cell by cell...
                            ExcelRangeBase curcell = workSheet.Cells[row, col];
                            string cellValue = curcell.Text; // This got me the actual value I needed.
                            if (wordcollection.Contains(cellValue))
                            {
                                for (int i = 1; i < GetCellMergedAttributes(curcell)[0]; i++)
                                {
                                    ExcelRangeBase titlecell = workSheet.Cells[row + i, 1];
                                    int[] attr = GetCellMergedAttributes(titlecell);
                                    if (attr != null)
                                    {
                                        XmlElement tr_node = xmlDoc.CreateElement("TitleRow");
                                        XmlAttribute ordnumAttr = xmlDoc.CreateAttribute("row"),
                                            rowspanAttr = xmlDoc.CreateAttribute("rowspan");
                                        ordnumAttr.Value = row.ToString();
                                        tr_node.Attributes.Append(ordnumAttr);
                                        rowspanAttr.Value = attr[0].ToString();
                                        ws_node.AppendChild(tr_node);
                                        for (int find = 1; find < end.Column; find++)
                                        {

                                            ExcelRangeBase columncell = workSheet.Cells[row + i, find];
                                            int[] attr_column = GetCellMergedAttributes(titlecell);
                                            if (attr_column != null)
                                            {
                                                XmlElement c_node = xmlDoc.CreateElement("Column");
                                                XmlAttribute ordNumAttr = xmlDoc.CreateAttribute("ordnumber"),
                                                    colspanAttr = xmlDoc.CreateAttribute("colspan");
                                                ordNumAttr.Value = find.ToString();
                                                c_node.Attributes.Append(ordNumAttr);
                                                colspanAttr.Value = attr_column[1].ToString();
                                                c_node.Attributes.Append(colspanAttr);
                                                tr_node.AppendChild(c_node);

                                                XmlElement t_node = xmlDoc.CreateElement("Title");
                                                t_node.InnerText = columncell.Text;
                                                c_node.AppendChild(t_node);

                                            }

                                        }
                                    }

                                }
                            }
                        }
                    }

                }
                if (xmlDoc.DocumentElement != null) xmlDoc.DocumentElement.AppendChild(pricenode);
                // }
            }
        }


        public IExcelDataReader getExcelReader()
        {
            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
            // to get started. This is how we avoid dependencies on ACE or Interop:
            FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read);

            // We return the interface, so that
            IExcelDataReader reader = null;
            try
            {
                if (_path.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                if (_path.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<string> getWorksheetNames()
        {
            var reader = this.getExcelReader();
            var workbook = reader.AsDataSet();
            var sheets = from DataTable sheet in workbook.Tables select sheet.TableName;
            return sheets;
        }
        public IEnumerable<DataRow> getData(string sheet, bool firstRowIsColumnNames = true)
        {
            var reader = this.getExcelReader();
            reader.IsFirstRowAsColumnNames = firstRowIsColumnNames;
            var workSheet = reader.AsDataSet().Tables[sheet];
            var rows = from DataRow a in workSheet.Rows select a;
            return rows;
        }
    }
}
