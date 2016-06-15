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
        public DateListFactory(int daysInThisMonth, IEnumerable<int> holidays, IEnumerable<WardType> wardTypes)
        {
            this.daysInThisMonth = daysInThisMonth;
            this.holidays = holidays;
            this.wardTypes = wardTypes;
        }
        public List<DateInformation> getDateList()
        {
            var dateList = new List<DateInformation>();
            for (int i = 1; i <= daysInThisMonth; i++)
            {
                DateType setDateTypeTo = DateType.Workday;
                if (holidays.Contains(i))
                {
                    setDateTypeTo = DateType.Holiday;
                }
                else if (holidays.Contains(i + 1))
                {
                    setDateTypeTo = DateType.Weekend;
                }
                foreach (var w in wardTypes)
                {
                    dateList.Add(
                        new DateInformation()
                        {
                            date = i,
                            dateType = setDateTypeTo,
                            wardType = w
                        });
                }
            }
            return dateList;
        }
    }
}
