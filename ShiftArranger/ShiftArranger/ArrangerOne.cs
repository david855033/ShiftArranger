using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    class ArrangerOne
    {
        List<DateInformation> dateList = new List<DateInformation>();
        List<DoctorInformation> doctorList = new List<DoctorInformation>();
        int daysInThisMonths, weekDayOfTheFirstDay; List<int> Holidays;
        public void setDateList(int daysInThisMonths, int weekDayOfTheFirstDay, IEnumerable<int> Holidays)
        {
            this.dateList = new DateListFactory(daysInThisMonths, weekDayOfTheFirstDay, Holidays, WardSets.allWards).getDateList();
            this.daysInThisMonths = daysInThisMonths;
            this.weekDayOfTheFirstDay = weekDayOfTheFirstDay;
            this.Holidays = new List<int>(Holidays);
        }

        public void setDoctorList(IEnumerable<DoctorInformation> doctorList)
        {
            this.doctorList = doctorList.getCopyOfDoctorList();
        }

        public resultGroup Calculate()
        {
            int score = 0;
            int bias = Rand.getRand(daysInThisMonths);
            var rankDutyCounter = new DutyCounterForSameRank();

            //預先塞班
            for (int i = 0; i < daysInThisMonths; i++)
            {
                var doctorWantThisDay = doctorList.FindAll(x => x.absoluteWantThisDay.Contains(i + 1));
                foreach (var d in doctorWantThisDay)
                {
                    var availableWard = dateList.Find(x => x.wardType == d.mainWard);
                    if (availableWard != null)
                    {
                        availableWard.dutyDoctor[i] = d.ID;
                    }
                    else
                    {
                        var anotherAvailableWard = dateList.First(x => d.capableOf.Contains(x.wardType));
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
                    var currentDateList = dateList.First(x => x.wardType == ward);
                    var dateType = currentDateList.dateType[j];

                    if (currentDateList.dutyDoctor[j] != "")
                    {
                        continue;
                    }

                    var query = from q in doctorList
                                where q.capableOf.Contains(ward) &&
                                (dateType == DateType.Holiday ? (q.holidayDuty > q.arrangedHolidayDuty) :  //若為假日還有假日班可用
                                (q.nonHolidayDuty > q.arrangedNonHolidayDuty))                             //若為平日還有平日班可用
                                select q;
                    var priorityDoctorList = new List<DoctorInformation>();
                    var prioritySubOptimalDoctorList = new List<DoctorInformation>();
                    var AvailableDoctorList = new List<DoctorInformation>(query);
                    var SubOptimalDoctorList = new List<DoctorInformation>();


                    //排除絕對不要這天
                    AvailableDoctorList.RemoveAll(x => x.absoluteAvoidThisDay.Contains(j + 1));

                    //排除同一天已經安排
                    AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j, x.ID, dateList) == true);

                    //排除連續值班(j-1day)
                    if (j > 0)
                    {
                        AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j - 1, x.ID, dateList) == true);
                    }
                    //排除連續值班(j+1 day)
                    if (j < daysInThisMonths - 1)
                    {
                        AvailableDoctorList.RemoveAll(x => DateInformation.isDoctorInThisDay(j + 1, x.ID, dateList) == true);
                    }

                    //往前排除五天內三班
                    if (j > 3)
                    {
                        AvailableDoctorList.RemoveAll(x =>
                        DateInformation.isDoctorInThisDay(j - 2, x.ID, dateList) == true
                        && DateInformation.isDoctorInThisDay(j - 4, x.ID, dateList) == true
                        );
                    }
                    //往後排除五天內三班
                    if (j < daysInThisMonths - 4)
                    {
                        AvailableDoctorList.RemoveAll(x =>
                        DateInformation.isDoctorInThisDay(j + 2, x.ID, dateList) == true
                        && DateInformation.isDoctorInThisDay(j + 4, x.ID, dateList) == true
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
                                if (DateInformation.isDoctorInThisDay(h, doctor.ID, dateList))
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

                    //不可兩個新手R1在CU
                    if (currentDateList.wardType == WardType.NICU)
                    {
                        var dutyDoctorAtPICU = dateList.Find(x => x.wardType == WardType.PICU).dutyDoctor[j];
                        if (dutyDoctorAtPICU.Length == 2 &&
                            doctorList.Find(x => x.ID.Substring(0, 1) == dutyDoctorAtPICU.Substring(0, 1)).doctorType == DoctorType.R1)
                        {
                            AvailableDoctorList.RemoveAll(x => x.doctorType == DoctorType.R1 && x.ID.Length == 2);
                        }
                    }

                    //長幼有序運算+ 空班多的優先
                    List<int> dutyShouldBeArrangeList = new List<int>();
                    var doctorListIsLeastArranged = new List<DoctorInformation>();
                    int maxDutyShouldBeArrange = 0;
                    List<string> doctorFirstWord = new List<string>();
                    Dictionary<string, int> DoctorFirstWordAndDutyTable = new Dictionary<string, int>();
                    foreach (var d in AvailableDoctorList)
                    {
                        if (!DoctorFirstWordAndDutyTable.ContainsKey(d.ID.Substring(0, 1)))
                            DoctorFirstWordAndDutyTable.Add(d.ID.Substring(0, 1), 0);
                    }
                    foreach (var d in DoctorFirstWordAndDutyTable.Keys.ToList())
                    {
                        var matchDoctorQuery = from q in AvailableDoctorList
                                               where q.ID.Substring(0, 1) == d
                                               select q;
                        int arrangedHolidayDuty = 0, arrangedNonHolidayDuty = 0; var doctorType = new DoctorType();
                        foreach (var sameDoctor in matchDoctorQuery)
                        {
                            arrangedHolidayDuty += sameDoctor.arrangedHolidayDuty;
                            arrangedNonHolidayDuty += sameDoctor.arrangedNonHolidayDuty;
                            doctorType = sameDoctor.doctorType;
                        }
                        DoctorFirstWordAndDutyTable[d] = arrangedHolidayDuty + arrangedNonHolidayDuty;
                        int dutyShouldBeArrange = rankDutyCounter.getExpectTotalDuty(doctorType)
                            - (DoctorFirstWordAndDutyTable[d]);
                        maxDutyShouldBeArrange = Math.Max(maxDutyShouldBeArrange, dutyShouldBeArrange);
                    }
                    foreach (var d in DoctorFirstWordAndDutyTable.Keys.ToList())
                    {
                        int dutyShouldBeArrange = rankDutyCounter.getExpectTotalDuty(AvailableDoctorList.Find(x => x.ID.StartsWith(d)).doctorType)
                            - (DoctorFirstWordAndDutyTable[d]);
                        if (dutyShouldBeArrange >= maxDutyShouldBeArrange - 1)
                        {
                            priorityDoctorList.Add(AvailableDoctorList.Find(x => x.ID.Substring(0, 1) == d));
                        }
                    }
                    AvailableDoctorList.RemoveAll(x => priorityDoctorList.Contains(x));

                    //不符合主要病房
                    var doctorListNotMainWard = AvailableDoctorList.FindAll(x => x.mainWard != ward);
                    SubOptimalDoctorList.AddRange(doctorListNotMainWard);
                    AvailableDoctorList.RemoveAll(x => doctorListNotMainWard.Contains(x));
                    if (priorityDoctorList.Count > 1)
                    {
                        var doctorListToExcludeInPriority = priorityDoctorList.FindAll(x => x.mainWard != ward);
                        prioritySubOptimalDoctorList.AddRange(doctorListToExcludeInPriority);
                        priorityDoctorList.RemoveAll(x => doctorListToExcludeInPriority.Contains(x));
                    }

                    //QOD一次
                    var doctorListQoD = AvailableDoctorList.FindAll(x =>
                    (j >= 2 && DateInformation.isDoctorInThisDay(j - 2, x.ID, dateList) == true) ||
                    (j < daysInThisMonths - 2 && DateInformation.isDoctorInThisDay(j + 2, x.ID, dateList) == true));
                    SubOptimalDoctorList.AddRange(doctorListQoD);
                    AvailableDoctorList.RemoveAll(x => doctorListQoD.Contains(x));
                    if (priorityDoctorList.Count > 1)
                    {
                        var doctorListToExcludeInPriority = priorityDoctorList.FindAll(x =>
                        (j >= 2 && DateInformation.isDoctorInThisDay(j - 2, x.ID, dateList) == true) ||
                        (j < daysInThisMonths - 2 && DateInformation.isDoctorInThisDay(j + 2, x.ID, dateList) == true));
                        prioritySubOptimalDoctorList.AddRange(doctorListToExcludeInPriority);
                        priorityDoctorList.RemoveAll(x => doctorListToExcludeInPriority.Contains(x));
                    }


                    //相對不喜歡這天
                    var doctorListRelativeAvoid = AvailableDoctorList.FindAll(x => x.relativeAvoidThisDay.Contains(j + 1));
                    SubOptimalDoctorList.AddRange(doctorListRelativeAvoid);
                    AvailableDoctorList.RemoveAll(x => doctorListRelativeAvoid.Contains(x));
                    if (priorityDoctorList.Count > 1)
                    {
                        var doctorListToExcludeInPriority = priorityDoctorList.FindAll(x => x.relativeAvoidThisDay.Contains(j + 1));
                        prioritySubOptimalDoctorList.AddRange(doctorListToExcludeInPriority);
                        priorityDoctorList.RemoveAll(x => doctorListToExcludeInPriority.Contains(x));
                    }

                    //填入
                    if (priorityDoctorList.Count > 0)
                    {
                        DoctorInformation DoctorToBeAssign = priorityDoctorList.getRandomElement<DoctorInformation>();
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
                    else if (prioritySubOptimalDoctorList.Count > 0)
                    {
                        DoctorInformation DoctorToBeAssign = prioritySubOptimalDoctorList.getRandomElement<DoctorInformation>();
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
                        score++;
                    }
                    else if (AvailableDoctorList.Count > 0)
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
                        score++;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            //結尾計算 同階級要公平
            MainLogic.CountActualDutyDay(doctorList, dateList, daysInThisMonths);
            Dictionary<DoctorType, int> RankDutyDayTableMax = new Dictionary<DoctorType, int>();
            Dictionary<DoctorType, int> RankDutyDayTableMin = new Dictionary<DoctorType, int>();
            foreach (var doctorType in DoctorTypeSets.allDoctorTypes)
            {
                var query = from q in doctorList
                            where q.doctorType == doctorType
                            select q.ID.Substring(0,1);
                int maxTotalDutyDay = 0;
                int minTotalDutyDay = 100;
                foreach (var s in query)
                {
                    int TotalDutyDay = (from q in doctorList where q.ID.Substring(0, 1) == s select q.arrangedHolidayDuty).Sum() +
                        (from q in doctorList where q.ID.Substring(0, 1) == s select q.arrangedNonHolidayDuty).Sum();
                    maxTotalDutyDay = Math.Max(TotalDutyDay, maxTotalDutyDay);
                    minTotalDutyDay = Math.Min(TotalDutyDay, minTotalDutyDay);
                }
                RankDutyDayTableMax.Add(doctorType, maxTotalDutyDay);
                RankDutyDayTableMin.Add(doctorType, minTotalDutyDay);
                if (maxTotalDutyDay - minTotalDutyDay == 1)
                {
                    score += 10;
                }else if (maxTotalDutyDay - minTotalDutyDay == 2)
                {
                    score += 50;
                }
                else if (maxTotalDutyDay - minTotalDutyDay > 2)
                {
                    score += 100;
                }
            }
            if (RankDutyDayTableMax[DoctorType.R3] > RankDutyDayTableMin[DoctorType.R2]) score += 50;
            if (RankDutyDayTableMax[DoctorType.R2] - RankDutyDayTableMin[DoctorType.R3] > 1) score += 50;

            if (RankDutyDayTableMax[DoctorType.R2] > RankDutyDayTableMin[DoctorType.R1]) score += 50;
            if (RankDutyDayTableMax[DoctorType.R1] - RankDutyDayTableMin[DoctorType.R2] > 1) score += 50;

            bool AllR3Equal = true; int Duty = 0;
            foreach (var d in doctorList.Where(x => x.doctorType == DoctorType.R3))
            {
                int newDuty = (from q in doctorList
                               where q.ID.Substring(0, 1) == d.ID.Substring(0, 1)
                               select q.holidayDuty).Sum() +
                       (from q in doctorList
                        where q.ID.Substring(0, 1) == d.ID.Substring(0, 1)
                        select q.nonHolidayDuty).Sum();
                if (Duty == 0) { Duty = newDuty; }
                else if (Duty != newDuty) { AllR3Equal = false; break; }
            }
            if (!AllR3Equal) score += 40;

            bool AllR2Equal = true; Duty = 0;
            foreach (var d in doctorList.Where(x => x.doctorType == DoctorType.R2))
            {
                int newDuty = (from q in doctorList
                               where q.ID.Substring(0, 1) == d.ID.Substring(0, 1)
                               select q.arrangedHolidayDuty).Sum() +
                       (from q in doctorList
                        where q.ID.Substring(0, 1) == d.ID.Substring(0, 1)
                        select q.arrangedNonHolidayDuty).Sum();
                if (Duty == 0) { Duty = newDuty; }
                else if (Duty != newDuty) { AllR2Equal = false; break; }
            }
            if (!AllR2Equal) score += 25;

            //W5
            Dictionary<string, int> W5Count = new Dictionary<string, int>();
            foreach (var wardDate in dateList)
            {
                for (int i = 0; i < daysInThisMonths; i++)
                {
                    if (wardDate.dateType[i] == DateType.Weekend)
                    {
                        if (W5Count.ContainsKey(wardDate.dutyDoctor[i].Substring(0, 1)))
                        {
                            W5Count[wardDate.dutyDoctor[i].Substring(0, 1)]++;
                        }
                        else
                        {
                            W5Count.Add(wardDate.dutyDoctor[i].Substring(0, 1), 1);
                        }
                    }
                }
            }
            foreach (var c in W5Count)
            {
                if (c.Value > 1) { score += 1; }
                else if (c.Value > 2) { score += 10; }
            }

            //值得班多 假日班要少
            foreach (var d in doctorList.FindAll(x=>x.doctorType == DoctorType.PGY))
            {
                if (d.arrangedNonHolidayDuty == rankDutyCounter.getMaxWorkdayCount(d.doctorType) &&
                   d.arrangedHolidayDuty == rankDutyCounter.getMaxHolidayCount(d.doctorType))
                {
                    score += 20;
                }
            }

            return new resultGroup() { score = score, doctorListResult = doctorList, dateListResult = dateList };
        }

    }
}
