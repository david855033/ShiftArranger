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
                    var AvailableDoctorList = new List<DoctorInformation>(query);
                    var priorityDoctorList = new List<DoctorInformation>();
                    var SubOptimalDoctorList = new List<DoctorInformation>();
                    var VerySubOptimalDoctorList = new List<DoctorInformation>();


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


                    //長幼有序運算
                    List<int> dutyShouldBeArrangeList = new List<int>();
                    var doctorListIsLeastArranged = new List<DoctorInformation>();
                    int maxDutyShouldBeArrange = 0;
                    foreach (var d in AvailableDoctorList)
                    {
                        int dutyShouldBeArrange = rankDutyCounter.getExpectTotalDuty(d.doctorType)
                            - (d.arrangedHolidayDuty + d.arrangedNonHolidayDuty);
                        maxDutyShouldBeArrange = Math.Max(maxDutyShouldBeArrange, dutyShouldBeArrange);
                    }
                    foreach (var d in AvailableDoctorList)
                    {
                        int dutyShouldBeArrange = rankDutyCounter.getExpectTotalDuty(d.doctorType)
                            - (d.arrangedHolidayDuty + d.arrangedNonHolidayDuty);
                        if (dutyShouldBeArrange == maxDutyShouldBeArrange)
                        {
                            priorityDoctorList.Add(d);
                        }
                    }
                    AvailableDoctorList.RemoveAll(x => priorityDoctorList.Contains(x));


                    //不符合主要病房
                    var doctorListNotMainWard = AvailableDoctorList.FindAll(x => x.mainWard != ward);
                    SubOptimalDoctorList.AddRange(doctorListNotMainWard);
                    AvailableDoctorList.RemoveAll(x => doctorListNotMainWard.Contains(x));

                    //QOD一次
                    var doctorListQoD = AvailableDoctorList.FindAll(x =>
                    (j >= 2 && DateInformation.isDoctorInThisDay(j - 2, x.ID, dateList) == true) ||
                    (j < daysInThisMonths - 2 && DateInformation.isDoctorInThisDay(j + 2, x.ID, dateList) == true));
                    SubOptimalDoctorList.AddRange(doctorListQoD);
                    AvailableDoctorList.RemoveAll(x => doctorListQoD.Contains(x));

                    //相對不喜歡這天
                    var doctorListRelativeAvoid = AvailableDoctorList.FindAll(x => x.relativeAvoidThisDay.Contains(j + 1));
                    SubOptimalDoctorList.AddRange(doctorListRelativeAvoid);
                    AvailableDoctorList.RemoveAll(x => doctorListRelativeAvoid.Contains(x));

                    //W5 count

                    //不能兩個R1

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
                    else if (VerySubOptimalDoctorList.Count > 0)
                    {
                        DoctorInformation DoctorToBeAssign = VerySubOptimalDoctorList.getRandomElement<DoctorInformation>();
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
                        score += 3;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            //結尾計算
            MainLogic.CountActualDutyDay(doctorList, dateList, daysInThisMonths);
            Dictionary<DoctorType, int> RankDutyDayTableMax = new Dictionary<DoctorType, int>();
            Dictionary<DoctorType, int> RankDutyDayTableMin = new Dictionary<DoctorType, int>();
            foreach (var doctorType in DoctorTypeSets.allDoctorTypes)
            {
                var query = from q in doctorList
                            where q.doctorType == doctorType
                            select q;
                int maxTotalDutyDay = 0;
                int minTotalDutyDay = 100;
                foreach (var d in query)
                {
                    if (d.nonHolidayDuty + d.holidayDuty < 5) continue;
                    int TotalDutyDay = d.arrangedHolidayDuty + d.arrangedNonHolidayDuty;
                    maxTotalDutyDay = Math.Max(TotalDutyDay, maxTotalDutyDay);
                    minTotalDutyDay = Math.Min(TotalDutyDay, minTotalDutyDay);
                }
                RankDutyDayTableMax.Add(doctorType, maxTotalDutyDay);
                RankDutyDayTableMin.Add(doctorType, minTotalDutyDay);
                if (maxTotalDutyDay - minTotalDutyDay > 1)
                {
                    score += 30;
                }
            }
            if (RankDutyDayTableMin[DoctorType.R3] > RankDutyDayTableMax[DoctorType.R2]) score += 30;
            if (RankDutyDayTableMin[DoctorType.R2] > RankDutyDayTableMax[DoctorType.R1]) score += 30;
            return new resultGroup() { score = score, doctorListResult = doctorList, dateListResult = dateList };
        }

    }
}
