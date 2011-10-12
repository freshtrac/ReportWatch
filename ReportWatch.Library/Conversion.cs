using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportWatch.Library
{
    public class Conversion
    {

        public static decimal StringToDecimal(string s)
        {

            decimal d = 0.0M;

            s = Regex.Replace(s, "[^-+0123456789.eE]", "");
            if (s != null && s.Length > 0)
            {
                try
                {
                    d = Decimal.Parse(s);
                }
                catch
                {
                    // If it cannot be parsed, then let it be zero.
                }
            }
            return d;
        }

        public static Int16 StringToInt16(string s)
        {

            Int16 i = 0;

            s = Regex.Replace(s, "[^-+0123456789.]", "");
            if (s != null && s.Length > 0)
            {
                try
                {
                    i = Int16.Parse(s);
                }
                catch
                {
                    // If it cannot be parsed, then let it be zero.
                }
            }
            return i;
        }

        public static Int32 StringToInt32(string s)
        {

            int i = 0;

            s = Regex.Replace(s, "[^-+0123456789.]", "");
            if (s != null && s.Length > 0)
            {
                try
                {
                    i = Int32.Parse(s);
                }
                catch
                {
                    // If it cannot be parsed, then let it be zero.
                }
            }
            return i;
        }

        public static Int64 StringToInt64(string s)
        {

            Int64 i = 0;

            s = Regex.Replace(s, "[^-+0123456789.]", "");
            if (s != null && s.Length > 0)
            {
                try
                {
                    i = Int64.Parse(s);
                }
                catch
                {
                    // If it cannot be parsed, then let it be zero.
                }
            }
            return i;
        }
    }
}