using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
   
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
