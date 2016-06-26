using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    class DutyCounterForSameRank
    {
        Dictionary<DoctorType, int> holidayCount = new Dictionary<DoctorType, int>();
        Dictionary<DoctorType, int> workdayCount = new Dictionary<DoctorType, int>();
        public DutyCounterForSameRank()
        {
            foreach (var d in DoctorTypeSets.allDoctorTypes)
            {
                holidayCount.Add(d, 0);
                workdayCount.Add(d, 0);
            }
        }
        public void setHolidayCount(DoctorType w, int c)
        {
            if (c > holidayCount[w])
            {
                holidayCount[w] = c;
            }
        }
        public void setWorkdayCount(DoctorType w, int c)
        {
            if (c > workdayCount[w])
            {
                workdayCount[w] = c;
            }
        }
        public int setHolidayCount(DoctorType w)
        {
            return holidayCount[w];
        }
        public int getWorkday(DoctorType w)
        {
            return workdayCount[w];
        }
    }
}

