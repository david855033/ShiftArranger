using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{

    class DateListFactory
    {
        IEnumerable<int> holidays;
        IEnumerable<WardType> wardTypes;
        int daysInThisMonth;
        int firstWeekDayofThisMonth;
        public DateListFactory(int daysInThisMonth,int firstWeekDayofThisMonth, IEnumerable<int> holidays, IEnumerable<WardType> wardTypes)
        {
            this.firstWeekDayofThisMonth = firstWeekDayofThisMonth;
            this.daysInThisMonth = daysInThisMonth;
            this.holidays = holidays;
            this.wardTypes = wardTypes;
        }
        public List<DateInformation> getDateList()
        {
            var dateList = new List<DateInformation>();
            foreach (var w in WardSets.allWards)
            {
                var toAdd = new DateInformation();
                for (int i = 0; i < daysInThisMonth; i++)
                {
                    toAdd.wardType = w;
                    DateType setDateTypeTo = DateType.Workday;
                    if (holidays.Contains(i + 1))
                    {
                        setDateTypeTo = DateType.Holiday;
                    }
                    else if (holidays.Contains(i + 2))
                    {
                        setDateTypeTo = DateType.Weekend;
                    }
                    toAdd.dateType[i] = setDateTypeTo;
                }
                dateList.Add(toAdd);
            }
            return dateList;
        }
    }
}
