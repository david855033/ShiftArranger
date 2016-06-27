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
        public void setHolidayCount(DoctorType d, int c)
        {
            if (c > holidayCount[d])
            {
                holidayCount[d] = c;
            }
        }
        public void setWorkdayCount(DoctorType d, int c)
        {
            if (c > workdayCount[d])
            {
                workdayCount[d] = c;
            }
        }

        public int getExpectTotalDuty(DoctorType d)
        {
            if (d == DoctorType.PGY)
            {
                return holidayCount[d] + workdayCount[d];
            }
            else
            {
                int R1Have = holidayCount[DoctorType.R1] + workdayCount[DoctorType.R1];
                int R2Have = holidayCount[DoctorType.R2] + workdayCount[DoctorType.R2];
                int R3Have = holidayCount[DoctorType.R3] + workdayCount[DoctorType.R3];
                int R1ShouldHave = Math.Max(Math.Max(R1Have, R2Have + 1), R3Have + 2);
                if (d == DoctorType.R1)
                {
                    return R1ShouldHave;
                }
                else if (d == DoctorType.R2)
                {
                    return R1ShouldHave - 1;
                }
                else {
                    return R1ShouldHave - 2;
                }
            }
        }
    }
}

