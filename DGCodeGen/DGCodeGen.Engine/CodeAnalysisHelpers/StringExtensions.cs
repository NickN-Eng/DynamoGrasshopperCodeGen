using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string str)
        {
            if (str.Length == 1)
                return str.ToUpper();
            else
                return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string FirstCharToLower(this string str)
        {
            if (str.Length == 1)
                return str.ToLower();
            else
                return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
