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
        public static List<WardType> getWardListFromString(this string input, out bool fail)
        {
            var result = new List<WardType>();
            foreach (var ward in WardSets.allWards)
            {
                if (input.IndexOf(ward.ToString()) >= 0)
                {
                    result.Add(ward);
                }
            }
            fail = result.Count == 0;
            return result;
        }
        public static WardType getWardFromString(this string input, out bool fail)
        {
            foreach (var ward in WardSets.allWards)
            {
                if (input.IndexOf(ward.ToString()) >= 0)
                {
                    fail = false;
                    return ward;
                }
            }
            fail = true;
            return WardType.A091;
        }
        public static DoctorType getDoctorTypeFromString(this string input, out bool fail)
        {
            foreach (var doctorType in DoctorTypeSets.allDoctorTypes)
            {
                if (input.IndexOf(doctorType.ToString()) >= 0)
                {
                    fail = false;
                    return doctorType;
                }
            }
            fail = true;
            return DoctorType.PGY;
        }

        public static int getIntFromString(this string input, out bool fail)
        {
            int result;
            fail = !int.TryParse(input, out result);
            return result;
        }
        public static List<int> getIntListFromString(this string input, out bool fail)
        {
            string[] split = input.Split(',');
            List<int> result = new List<int>();
            bool success = true;
            fail = false;
            foreach (var s in split)
            {
                int i;
                success = int.TryParse(s, out i);
                if (success && i < 31 && i >0)
                {
                    result.Add(i);
                }
            }
            result = result.Distinct().ToList();
            result.Sort();
            return result;
        }
    }
}
