using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT_Market.Core.HelpClasses
{
    public static class HelpReadFunc
    {
        public static bool IsEqualEntity(string first, string second, double permisVariation)
        {
            char[] firstarray = first.Trim().ToCharArray();
            char[] secondarray = second.Trim().ToCharArray();
            bool firstIsmax = firstarray.Length/secondarray.Length >= 1;
            int maxchar, minchar, misCompareCount = 0;
            if (firstIsmax)
            {
                maxchar = firstarray.Length;
                minchar = secondarray.Length;
            }
            else
            {
                maxchar = secondarray.Length;
                minchar = firstarray.Length;
            }
            for (int i = 0; i < minchar; i++)
            {
                if (firstarray[i] != secondarray[i])
                {
                    misCompareCount++;
                }
            }
            return (misCompareCount + maxchar - minchar)/maxchar <= (1-permisVariation);
        }

        public static bool ChekStartReadRow(StartRow startrow, DataRow datarow)
        {
            if (startrow.RewievColumn == null) return false;
            string content = datarow[(int) startrow.RewievColumn].ToString().Trim();
            return string.Equals(content, startrow.TitleRowPattern);
        }
    }
}
