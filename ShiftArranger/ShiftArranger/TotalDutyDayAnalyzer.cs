using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    public class TotalDutyDayAnalyzer
    {
        const int DAY_LIMIT_IN_MONTH = 800;
        IEnumerable<DateInformation> dateList;
        IEnumerable<DoctorInformation> doctorList;
        List<DoctorType> sortTypelist;
        Dictionary<DoctorType, int> DoctorCountByType;
        DayCount totalDayCount;
        Dictionary<DoctorType, List<Doctor_DayCount>> doctorDayCountByType;
        List<Doctor_DayCount> doctor_DayCountList = new List<Doctor_DayCount>();
        public TotalDutyDayAnalyzer(IEnumerable<DateInformation> dateList, IEnumerable<DoctorInformation> doctorList)
        {
            this.dateList = dateList;
            this.doctorList = doctorList;
        }

        public List<Doctor_DayCount> getDoctor_DayCountList()
        {
            sumUpDay();
            checkDoctorCountByType();
            sortDoctorByType();
            splitDaysIntoDoctors();
            makeDoctor_DayCountList();
            return doctor_DayCountList;
        }
        
        public  void makeDoctor_DayCountList()
        {
            doctor_DayCountList = new List<Doctor_DayCount>();
            foreach (var type in sortTypelist)
            {
                foreach (var d in doctorDayCountByType[type])
                {
                    doctor_DayCountList.Add(d);
                }
            }
        }

        void sumUpDay()
        {
            totalDayCount = new DayCount();
            totalDayCount.holiday = (from q in dateList
                                     where q.dateType == DateType.Holiday
                                     select q).Count();
            totalDayCount.weekend = (from q in dateList
                                     where q.dateType == DateType.Weekend
                                     select q).Count();
            totalDayCount.workday = (from q in dateList
                                     where q.dateType == DateType.Workday
                                     select q).Count();
        }
        void checkDoctorCountByType()
        {
            DoctorCountByType = new Dictionary<DoctorType, int>();
            foreach (var doctor in doctorList)
            {
                if (DoctorCountByType.ContainsKey(doctor.doctorType))
                {
                    DoctorCountByType[doctor.doctorType]++;
                }
                else
                {
                    DoctorCountByType.Add(doctor.doctorType, 1);
                }
            }
        }
        void sortDoctorByType()
        {
            sortTypelist = DoctorCountByType.Keys.ToList();
            sortTypelist.Sort();
            doctorDayCountByType = new Dictionary<DoctorType, List<Doctor_DayCount>>();
            foreach (var t in sortTypelist)
            {
                var query = from q in doctorList
                            where q.doctorType == t
                            select q;
                doctorDayCountByType.Add(t, new List<Doctor_DayCount>());
                foreach (var doctor in query)
                {
                    doctorDayCountByType[t].Add(new Doctor_DayCount() { doctor = doctor });
                }
            }
        }
        void splitDaysIntoDoctors()
        {
            double averageDayPerPerson = (double)totalDayCount.totalday / doctorList.Count();
            int remainingHolidays = totalDayCount.holiday;
            int remainingNonHolidays = totalDayCount.nonholiday;
            bool hasRemaningDay = true;

            int bypassSortGroup = sortTypelist.Count;
            while (hasRemaningDay)
            {
                bypassSortGroup--;
                for (int i = 0; i < sortTypelist.Count && hasRemaningDay; i++)
                {
                    if (i > bypassSortGroup && bypassSortGroup >= 0)
                    {
                        continue;
                    }
                    List<Doctor_DayCount> thisPool = doctorDayCountByType[sortTypelist[i]];
                    for (int k = 0; k < thisPool.Count; k++)
                    {
                        if (thisPool[k].totalDay >= DAY_LIMIT_IN_MONTH)
                            continue;
                        if (remainingNonHolidays > 0)
                        {
                            thisPool[k].nonHoliday++;
                            remainingNonHolidays--;
                        }
                        else if (remainingHolidays > 0)
                        {
                            thisPool[k].holiday++;
                            remainingHolidays--;
                        }
                        else
                        {
                            hasRemaningDay = false;
                            break;
                        }
                    }
                }
            }
        }
    }
    public class dayHolder
    {
        public int holidays;
        public int non_holidys;
    }

    public class Doctor_DayCount
    {
        public DoctorInformation doctor;
        public int holiday;
        public int nonHoliday;
        public int totalDay { get { return holiday + nonHoliday; } }
    }

    public class DayCount
    {
        public int holiday;
        public int weekend;
        public int workday;
        public int nonholiday
        {
            get
            {
                return weekend + workday;
            }
        }
        public int totalday
        {
            get
            {
                return weekend + workday + holiday;
            }
        }
    }
}
