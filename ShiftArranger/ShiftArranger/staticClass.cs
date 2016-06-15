using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    public static class staticClass
    {
        public static string getStringFromList<T>(this IEnumerable<T> list)
        {
            if (list == null) return "";
            string result = "";
            foreach (var i in list)
            {
                if (result == "")
                {
                    result = i.ToString();
                }
                else
                {
                    result += "," + i.ToString();
                }
            }
            return result;
        }
    }
}
