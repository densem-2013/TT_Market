using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CreateInitialXML
{
    public class HelpForXML
    {
        public static void CreateInitialDoc(string entity)
        {

            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            // Create the root element
            XmlNode rootNode = xmlDoc.CreateElement(entity + "s");
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            xmlDoc.AppendChild(rootNode);
            foreach (string brand in File.ReadAllLines(entity.ToLower() + "s.txt").Distinct())
            {
                if (brand.Trim() != String.Empty)
                {
                    XmlNode brandNode = xmlDoc.CreateElement(entity);
                    brandNode.InnerText = brand;
                    rootNode.AppendChild(brandNode);
                }
            }
            string savepath = AppDomain.CurrentDomain.BaseDirectory.Replace(@"CreateInitialXML\bin\Debug", "") +
                              @"TT_Market.Core\DBinitial\Read" + entity + "sInitial.xml";
            xmlDoc.Save(savepath);
        }

        public static void GetValuesWithRegex(string regstr, string entity)
        {
            string source = File.ReadAllText(entity.ToLower() + "s.txt");
            Regex reg = new Regex(regstr);
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            // Create the root element
            XmlNode rootNode = xmlDoc.CreateElement(entity + "s");
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            xmlDoc.AppendChild(rootNode);
            foreach (Match m in reg.Matches(source))
            {
                if (m.Value.Trim() != String.Empty)
                {
                    XmlNode brandNode = xmlDoc.CreateElement(entity);
                    brandNode.InnerText = m.Groups[1].Value;
                    rootNode.AppendChild(brandNode);
                }

            }
            string savepath = AppDomain.CurrentDomain.BaseDirectory.Replace(@"CreateInitialXML\bin\Debug", "") +
                              @"TT_Market.Core\DBinitial\Read" + entity + "sInitial.xml";
            xmlDoc.Save(savepath);
        }
        public static void GetValuesWithRegexGrops(string regstr, string entity,string groups,string delim)
        {
            string source = File.ReadAllText(entity.ToLower() + "s.txt");
            Regex reg = new Regex(regstr, RegexOptions.Multiline);
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            // Create the root element
            XmlNode rootNode = xmlDoc.CreateElement(entity + "s");
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            xmlDoc.AppendChild(rootNode);
            int[] groupsArray = groups.Split(delim.ToCharArray()).Select(Int32.Parse).ToArray();
            List<string> targets=new List<string>();
            foreach (Match m in reg.Matches(source))
            {
                if (m.Value.Trim() != String.Empty)
                {

                    for (int i = 0; i < groupsArray.Length; i++)
                    {
                        if ((m.Groups[groupsArray[i]]).Length!=0)
                        {
                            targets.Add(m.Groups[groupsArray[i]].Value);
                        }
                    }
                }

            }
            foreach (string item in targets.Distinct())
            {
                XmlNode brandNode = xmlDoc.CreateElement(entity);
                brandNode.InnerText = item;
                rootNode.AppendChild(brandNode);
            }
            string savepath = AppDomain.CurrentDomain.BaseDirectory.Replace(@"CreateInitialXML\bin\Debug", "") +
                              @"TT_Market.Core\DBinitial\Read" + entity + "sInitial.xml";
            xmlDoc.Save(savepath);
        }

        public static void AddValuesIfNotExists(string entity)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string savepath = AppDomain.CurrentDomain.BaseDirectory.Replace(@"CreateInitialXML\bin\Debug", "") +
                              @"TT_Market.Core\DBinitial\Read" + entity + "sInitial.xml";
            xmlDoc.Load(savepath);

            // Create the root element
            XmlNode rootNode = xmlDoc.DocumentElement;
            List<string> collection =
                rootNode.SelectNodes("descendant::" + entity).Cast<XmlNode>().Select(node => node.InnerText).ToList();

            foreach (string item in File.ReadAllLines("addNew_" + entity.ToLower() + "s.txt").Distinct())
            {
                if (!collection.Contains(item) && item.Trim() != String.Empty)
                {
                    XmlNode node = xmlDoc.CreateElement(entity);
                    node.InnerText = item;
                    rootNode.AppendChild(node);
                }
            }
            xmlDoc.Save(savepath);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            // HelpForXML.CreateInitialDoc("Brand");
            //HelpForXML.CreateInitialDoc("Width");
            //HelpForXML.CreateInitialDoc("Height");
            //HelpForXML.CreateInitialDoc("Diameter");
            string regstr = @"<[^>]*>(.*?)\s*</[^>]*>"; // Содержимое между любыми тегами
            //HelpForXML.GetValuesWithRegex(regstr, "Width");
            //HelpForXML.GetValuesWithRegex(regstr, "Diameter");
            //HelpForXML.CreateInitialDoc("Country");
            //HelpForXML.AddValuesIfNotExists("Brand");
            string reggroups =
                @"(^\d{2,3}?)\/(\d{2,3}?)(((R\d{2,3}[C]?)|(R\d{2,3}\.{1}\d{1})?)\s+(?>(\d{2,3}[A-Z]{1})|(\d{2,3}\/\d{2,3}[A-Z]{1}))\s+(.*?)\n)|((^\d{2,3}?)\/(\d{2,3}?)(R\d{2,3}[.,]{1}\d{1}?)(?>(\d{2,3}[A-Z]{1})|(\d{2,3}\/\d{2,3}[A-Z]{1}))(.*?)\n)";
            HelpForXML.GetValuesWithRegexGrops(reggroups,"SpeedIndex","14,7,8",",");
        }
    }
}
