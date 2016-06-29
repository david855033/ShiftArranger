using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public string display { get; set; }
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

        public void setArrangedDutyToZero(IEnumerable<DoctorInformation> InputdoctorList)
        {
            foreach (var d in InputdoctorList)
            {
                d.arrangedHolidayDuty = 0;
                d.arrangedNonHolidayDuty = 0;
                d.arrangedWeekendDuty = 0;
            }
        }

        List<List<resultGroup>> resultPools;
        int count;

        public void arrange(int loops, int times)
        {
            resultPools = new List<List<resultGroup>>();
            List<Thread> threads = new List<Thread>();
            DateTime now = DateTime.Now;
            count = 0;
            for (int i = 0; i < loops; i++)
            {
                var thisPool = new List<resultGroup>();
                resultPools.Add(thisPool);
                Thread thread = new Thread(() => oneSplitWork(times, thisPool));
                thread.IsBackground = true;
                thread.Start();
                threads.Add(thread);
            }
            bool wait = true;
            while (wait)
            {
                Thread.Sleep(500);

                threads.RemoveAll(x => !x.IsAlive);
                if (threads.Count == 0)
                    wait = false;
            }
            List<resultGroup> finalResultPool = new List<resultGroup>();
            foreach (var p in resultPools)
            {
                finalResultPool.AddRange(p);
            }
            if (finalResultPool.Count == 0)
            {
                var r = $"運算{times * loops}次 無可行解 耗時{Math.Round(DateTime.Now.Subtract(now).TotalSeconds, 1)}秒.";
                System.Windows.MessageBox.Show(r);
                display = $"[{DateTime.Now}]" + r;
            }
            else
            {
                finalResultPool.Sort();
                dateList = finalResultPool.First().dateListResult;
                doctorList = finalResultPool.First().doctorListResult;
                var r = $"運算{count}次 共找出{finalResultPool.Count}組可行解 最佳解缺陷值: {finalResultPool.First().score} 耗時{Math.Round(DateTime.Now.Subtract(now).TotalSeconds, 1)}秒.";
                System.Windows.MessageBox.Show(r);
                display = $"[{DateTime.Now}]"+r;
            }
            finalResultPool.Clear();
        }
        void oneSplitWork(int times, List<resultGroup> thisPool)
        {
            int i = 0;
            while (i++ < times)
            {
                var arranger = new ArrangerOne();
                arranger.setDateList(daysInThisMonths, weekDayOfTheFirstDay, Holidays);
                arranger.setDoctorList(doctorList);
                var result = arranger.Calculate();
                if (result != null)
                {
                    thisPool.Add(result);
                }
                count++;
            }
        }
      
        public static void CountActualDutyDay(IEnumerable<DoctorInformation> doctorList, IEnumerable<DateInformation> dateList, int daysInThisMonths)
        {
            foreach (var doctor in doctorList)
            {
                doctor.arrangedHolidayDuty = 0;
                doctor.arrangedNonHolidayDuty = 0;
                foreach (var ward in dateList)
                {
                    for (int i = 0; i < daysInThisMonths; i++)
                    {
                        if (ward.dutyDoctor[i] == doctor.ID)
                        {
                            if (ward.dateType[i] == DateType.Holiday)
                            {
                                doctor.arrangedHolidayDuty++;
                            }
                            else
                            {
                                doctor.arrangedNonHolidayDuty++;
                            }
                        }
                    }
                }
            }
        }



    }

    public class resultGroup : IComparable
    {
        public IEnumerable<DoctorInformation> doctorListResult;
        public IEnumerable<DateInformation> dateListResult;
        public int score;

        public int CompareTo(object obj)
        {
            return this.score.CompareTo((obj as resultGroup).score);
        }
    }


    public class DateInformation
    {
        public WardType wardType;
        public DateType[] dateType = new DateType[31];
        public string[] dutyDoctor = new string[31];

        static public bool isDoctorInThisDay(int index, string ID, IEnumerable<DateInformation> theDateListOfWard)
        {
            foreach (var dateInfo in theDateListOfWard)
            {
                if (dateInfo.dutyDoctor[index] != null
                    && dateInfo.dutyDoctor[index] != ""
                    && dateInfo.dutyDoctor[index].Substring(0, 1) == ID.Substring(0, 1))
                {
                    return true;
                }
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
        public int arrangedHolidayDuty = 0;
        public int arrangedNonHolidayDuty = 0;
        public int arrangedWeekendDuty = 0;
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
            result.Append("\t" + arrangedHolidayDuty);
            result.Append("\t" + arrangedNonHolidayDuty);
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
            newDoctor.arrangedHolidayDuty = split[7].getIntFromString(out fail);
            newDoctor.arrangedNonHolidayDuty = split[8].getIntFromString(out fail);
            newDoctor.absoluteWantThisDay = split[9].getIntListFromString(out fail);
            newDoctor.absoluteAvoidThisDay = split[10].getIntListFromString(out fail);
            newDoctor.relativeWantThisDay = split[11].getIntListFromString(out fail);
            newDoctor.relativeAvoidThisDay = split[12].getIntListFromString(out fail);
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
