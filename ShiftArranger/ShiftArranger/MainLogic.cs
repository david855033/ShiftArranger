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

        public void setArrangedDutyToZero(IEnumerable<DoctorInformation> InputdoctorList)
        {
            foreach (var d in InputdoctorList)
            {
                d.arrangedHolidayDuty = 0;
                d.arrangedNonHolidayDuty = 0;
                d.arrangedWeekendDuty = 0;
            }
        }

        List<resultGroup> resultPool;

        public void arrange()
        {
            int times = 1000;
            resultPool = new List<resultGroup>();
            bool fail = true;
            int i = 0;
            while (fail && i++ <= times)
            {
                var result = arrangeOne();
                if (result != null)
                {
                    resultPool.Add(result);
                }
            }
            if (resultPool.Count == 0)
            {
                System.Windows.MessageBox.Show("無可行解");
            }
            resultPool.Sort();
            dateList = resultPool.First().dateList;
            doctorList = resultPool.First().doctorList;
            CountActualDutyDay();
            System.Windows.MessageBox.Show($"運算{times}次 共找出{resultPool.Count}組可行解.");
        }
        public void CountActualDutyDay()
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

        public resultGroup arrangeOne()
        {
            int score = 0;
            int bias = Rand.getRand(daysInThisMonths);
            var newDateList = new DateListFactory(daysInThisMonths, weekDayOfTheFirstDay, Holidays, WardSets.allWards).getDateList();
            var newDoctorList = doctorList.getCopyOfDoctorList();
            setArrangedDutyToZero(newDoctorList);
            var rankDutyCounter = new DutyCounterForSameRank();

            //預先塞班
            for (int i = 0; i < daysInThisMonths; i++)
            {
                var doctorWantThisDay = newDoctorList.FindAll(x => x.absoluteWantThisDay.Contains(i + 1));
                foreach (var d in doctorWantThisDay)
                {
                    var availableWard = newDateList.Find(x => x.wardType == d.mainWard);
                    if (availableWard != null)
                    {
                        availableWard.dutyDoctor[i] = d.ID;
                    }
                    else
                    {
                        var anotherAvailableWard = newDateList.First(x => d.capableOf.Contains(x.wardType));
                        if (anotherAvailableWard != null)
                        {
                            anotherAvailableWard.dutyDoctor[i] = d.ID;
                        }
                    }
                }
            }
            for (int i = 0; i < daysInThisMonths; i++)
            {
                int j = (i + bias) % daysInThisMonths;
                foreach (var ward in WardSets.allWards)
                {
                    var currentDateList = newDateList.First(x => x.wardType == ward);
                    var dateType = currentDateList.dateType[j];

                    if (currentDateList.dutyDoctor[j] != "")
                    {
                        continue;
                    }

                    var query = from q in newDoctorList
                                where q.capableOf.Contains(ward) &&
                                (dateType == DateType.Holiday ? (q.holidayDuty > q.arrangedHolidayDuty) :  //若為假日還有假日班可用
                                (q.nonHolidayDuty > q.arrangedNonHolidayDuty))                             //若為平日還有平日班可用
                                select q;
                    var AvailableDoctorList = new List<DoctorInformation>(query);
                    var priorityDoctorList = new List<DoctorInformation>();
                    var SubOptimalDoctorList = new List<DoctorInformation>();
                    var VerySubOptimalDoctorList = new List<DoctorInformation>();


                    //排除絕對不要這天
                    AvailableDoctorList.RemoveAll(x => x.absoluteAvoidThisDay.Contains(j + 1));

                    //排除同一天已經安排
                    AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j, x.ID, newDateList) == true);

                    //排除連續值班(j-1day)
                    if (j > 0)
                    {
                        AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j - 1, x.ID, newDateList) == true);
                    }
                    //排除連續值班(j+1 day)
                    if (j < daysInThisMonths - 1)
                    {
                        AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j + 1, x.ID, newDateList) == true);
                    }

                    //往前排除五天內三班
                    if (j > 4)
                    {
                        AvailableDoctorList.RemoveAll(x =>
                        DateInformation.isDoctorInThisDay(j - 2, x.ID, newDateList) == true
                        && DateInformation.isDoctorInThisDay(j - 4, x.ID, newDateList) == true
                        );
                    }
                    //往後排除五天內三班
                    if (j < daysInThisMonths - 4)
                    {
                        AvailableDoctorList.RemoveAll(x =>
                        DateInformation.isDoctorInThisDay(j + 2, x.ID, newDateList) == true
                        && DateInformation.isDoctorInThisDay(j + 4, x.ID, newDateList) == true
                        );
                    }

                    //一周不可超過兩班
                    {
                        int weekdayOfJ = (j + weekDayOfTheFirstDay - 1) % 7 + 1;
                        int j_start = Math.Max(0, j - (weekdayOfJ - 1));
                        int j_end = Math.Min(daysInThisMonths - 1, j_start + 6);
                        var doctorToBeRemoved = new List<DoctorInformation>();
                        foreach (var doctor in AvailableDoctorList)
                        {
                            int dutySum = 0;
                            for (int h = j_start; h <= j_end; h++)
                            {
                                if (DateInformation.isDoctorInThisDay(h, doctor.ID, newDateList))
                                {
                                    dutySum++;
                                    if (dutySum >= 2)
                                    {
                                        doctorToBeRemoved.Add(doctor);
                                        break;
                                    }
                                }
                            }
                        }
                        AvailableDoctorList.RemoveAll(x => doctorToBeRemoved.Contains(x));
                    }

                    //不符合主要病房
                    var doctorListNotMainWard = AvailableDoctorList.FindAll(x => x.mainWard != ward);
                    SubOptimalDoctorList.AddRange(doctorListNotMainWard);
                    AvailableDoctorList.RemoveAll(x => doctorListNotMainWard.Contains(x));

                    //長幼有序運算

                    //三天值兩班

                    //相對不喜歡這天


                    //填入
                    if (AvailableDoctorList.Count > 0)
                    {
                        DoctorInformation DoctorToBeAssign = AvailableDoctorList.getRandomElement<DoctorInformation>();
                        currentDateList.dutyDoctor[j] = DoctorToBeAssign.ID;

                        if (dateType == DateType.Holiday)
                        {
                            DoctorToBeAssign.arrangedHolidayDuty++;
                            rankDutyCounter.setHolidayCount(DoctorToBeAssign.doctorType, DoctorToBeAssign.arrangedHolidayDuty);
                        }
                        else
                        {
                            if (dateType == DateType.Weekend)
                                DoctorToBeAssign.arrangedWeekendDuty++;
                            DoctorToBeAssign.arrangedNonHolidayDuty++;
                            rankDutyCounter.setWorkdayCount(DoctorToBeAssign.doctorType, DoctorToBeAssign.arrangedNonHolidayDuty);
                        }
                    }
                    else if (SubOptimalDoctorList.Count > 0)
                    {
                        DoctorInformation DoctorToBeAssign = SubOptimalDoctorList.getRandomElement<DoctorInformation>();
                        currentDateList.dutyDoctor[j] = DoctorToBeAssign.ID;

                        if (dateType == DateType.Holiday)
                        {
                            DoctorToBeAssign.arrangedHolidayDuty++;
                            rankDutyCounter.setHolidayCount(DoctorToBeAssign.doctorType, DoctorToBeAssign.arrangedHolidayDuty);
                        }
                        else
                        {
                            if (dateType == DateType.Weekend)
                                DoctorToBeAssign.arrangedWeekendDuty++;
                            DoctorToBeAssign.arrangedNonHolidayDuty++;
                            rankDutyCounter.setWorkdayCount(DoctorToBeAssign.doctorType, DoctorToBeAssign.arrangedNonHolidayDuty);
                        }
                        score--;

                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return new resultGroup() { score = score, doctorList = newDoctorList, dateList = newDateList };
        }


    }

    public class resultGroup : IComparable
    {
        public IEnumerable<DoctorInformation> doctorList;
        public IEnumerable<DateInformation> dateList;
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

        static public bool isDoctorInThisDay(int index, string ID, IEnumerable<DateInformation> theList)
        {
            foreach (var dateInfo in theList)
            {
                if (dateInfo.dutyDoctor[index] != null && dateInfo.dutyDoctor[index] == ID)
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
        public int arrangedWeekendDuty;
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
