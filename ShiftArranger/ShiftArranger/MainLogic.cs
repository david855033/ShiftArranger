using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    public static class Rand
    {
        static Random rnd = new Random(DateTime.Now.Second);
        public static int getRand(int i) { return rnd.Next(i); }
    }
    public class MainLogic
    {
        public IEnumerable<DateInformation> dateList = new List<DateInformation>();
        public IEnumerable<DoctorInformation> doctorList = new List<DoctorInformation>();
        public int daysInThisMonths, weekDayOfTheFirstDay;
        public IEnumerable<int> Holidays = new List<int>();

        public void initializedDoctors()
        {
            var doctorListFactory = new DoctorListFactory();
            doctorList = doctorListFactory.getDoctorList();
        }

        public void initializeDate()
        {
            var dateListFactory = new DateListFactory(daysInThisMonths, weekDayOfTheFirstDay, Holidays, WardSets.allWards);
            dateList = dateListFactory.getDateList();
        }

        public void setArrangedDutyToZero()
        {
            foreach (var d in doctorList)
            {
                d.arrangedHolidayDuty = 0;
                d.arrangedNonHolidayDuty = 0;
            }
        }
        public void arrange()
        {
            bool fail=true;
            while (fail)
            {
                try
                {
                    arrangeOne();
                    fail = false;
                }
                catch
                {
                    fail = true;
                }
            }
        }
        public void arrangeOne()
        {
            int bias = Rand.getRand(daysInThisMonths);
            setArrangedDutyToZero();
            for (int i = 0; i < daysInThisMonths; i++)
            {
                int j = (i + bias) % daysInThisMonths;
                foreach (var ward in WardSets.allWards)
                {
                    var currentDateList = dateList.First(x => x.wardType == ward);
                    var dateType = currentDateList.dateType[j];
                    var query = from q in doctorList
                                where q.capableOf.Contains(ward) &&
                                (dateType == DateType.Holiday ? (q.holidayDuty > q.arrangedHolidayDuty) :  //若為假日還有假日班可用
                                (q.nonHolidayDuty > q.arrangedNonHolidayDuty))                             //若為平日還有平日班可用
                                select q;
                    var AvailableDoctorList = new List<DoctorInformation>(query);
                    //排除同一天已經安排
                    AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j, x.ID, dateList) == true);

                    //排除連續值班
                    if (j > 0)
                    {
                        AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j - 1, x.ID, dateList) == true);
                    }

                    //填入
                    var DoctorToBeAssign = AvailableDoctorList.getRandomElement<DoctorInformation>();
                    currentDateList.dutyDoctor[j] = DoctorToBeAssign.ID;
                    if (dateType == DateType.Holiday)
                    {
                        DoctorToBeAssign.arrangedHolidayDuty++;
                    }
                    else
                    {
                        DoctorToBeAssign.arrangedNonHolidayDuty++;
                    }

                }
            }
        }
    }

    public class DateInformation
    {
        public WardType wardType;
        public DateType[] dateType = new DateType[31];
        public string[] dutyDoctor = new string[31];

        static public bool isDoctorInThisDay(int index, string ID, IEnumerable<DateInformation> theList)
        {
            foreach (var dateInfo in theList)
            {
                if (dateInfo.dutyDoctor[index] == ID)
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(wardType.ToString());
            for (int i = 0; i < 31; i++)
            {
                result.Append("\t" + dateType[i].ToString());
                result.Append("," + dutyDoctor[i]);
            }
            return result.ToString();
        }

        public static DateInformation loadFromString(string input)
        {
            var newDateInformation = new DateInformation();
            string[] split = input.Split('\t');
            bool fail;
            newDateInformation.wardType = split[0].getWardFromString(out fail);
            for (int i = 0; i < 31; i++)
            {
                newDateInformation.dateType[i] = split[i + 1].Split(',')[0].getDateTypeFromString(out fail);
                newDateInformation.dutyDoctor[i] = split[i + 1].Split(',')[1];
            }

            return newDateInformation;
        }
    }

    public class DoctorInformation : IComparable
    {
        public string ID;
        public string name;
        public DoctorType doctorType;
        public WardType mainWard;
        public IEnumerable<WardType> capableOf;
        public int holidayDuty;
        public int nonHolidayDuty;
        public int arrangedHolidayDuty;
        public int arrangedNonHolidayDuty;
        public IEnumerable<int> absoluteWantThisDay;
        public IEnumerable<int> absoluteAvoidThisDay;
        public IEnumerable<int> relativeWantThisDay;
        public IEnumerable<int> relativeAvoidThisDay;

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(ID);
            result.Append("\t" + name);
            result.Append("\t" + doctorType);
            result.Append("\t" + mainWard);
            result.Append("\t" + capableOf.getStringFromList());
            result.Append("\t" + holidayDuty);
            result.Append("\t" + nonHolidayDuty);
            result.Append("\t" + absoluteWantThisDay.getStringFromList());
            result.Append("\t" + absoluteAvoidThisDay.getStringFromList());
            result.Append("\t" + relativeWantThisDay.getStringFromList());
            result.Append("\t" + relativeAvoidThisDay.getStringFromList());
            return result.ToString();
        }
        public static DoctorInformation loadFromString(string input)
        {
            var newDoctor = new DoctorInformation();
            var split = input.Split('\t');
            bool fail;
            newDoctor.ID = split[0];
            newDoctor.name = split[1];
            newDoctor.doctorType = split[2].getDoctorTypeFromString(out fail);
            newDoctor.mainWard = split[3].getWardFromString(out fail);
            newDoctor.capableOf = split[4].getWardListFromString(out fail);
            newDoctor.holidayDuty = split[5].getIntFromString(out fail);
            newDoctor.nonHolidayDuty = split[6].getIntFromString(out fail);
            newDoctor.absoluteWantThisDay = split[7].getIntListFromString(out fail);
            newDoctor.absoluteAvoidThisDay = split[8].getIntListFromString(out fail);
            newDoctor.relativeWantThisDay = split[9].getIntListFromString(out fail);
            newDoctor.relativeAvoidThisDay = split[10].getIntListFromString(out fail);
            return newDoctor;
        }

        public int CompareTo(object obj)
        {
            var that = (obj as DoctorInformation);
            int result = this.doctorType.CompareTo(that.doctorType);
            if (result != 0) return -result;
            return this.ID.CompareTo(that.ID);
        }
    }

    public enum WardType
    {
        NICU, PICU, A093, A091, NBR
    }
    public static class WardSets
    {
        public static WardType[] allWards = new WardType[] { WardType.PICU, WardType.NICU, WardType.A091, WardType.A093, WardType.NBR };
        public static WardType[] NICU = new WardType[] { WardType.NICU };
    }
    public static class DoctorTypeSets
    {
        public static DoctorType[] allDoctorTypes =
            new DoctorType[] { DoctorType.Fellow, DoctorType.PGY, DoctorType.R1, DoctorType.R2, DoctorType.R3 };
    }
    public enum DateType
    {
        Holiday,
        Weekend,
        Workday
    }
    public static class DateTypeSets
    {
        public static DateType[] allDateTypes =
            new DateType[] { DateType.Holiday, DateType.Weekend, DateType.Workday };
    }
    public enum DoctorType
    {
        PGY, R1, R2, R3, Fellow
    }
}
