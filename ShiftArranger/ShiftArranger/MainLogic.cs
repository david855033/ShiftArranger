using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftArranger
{
    public class MainLogic
    {
        public IEnumerable<DateInformation> dateList = new List<DateInformation>();
        public IEnumerable<DoctorInformation> doctorList = new List<DoctorInformation>();
        public IEnumerable<Doctor_DayCount> doctor_DayCountList = new List<Doctor_DayCount>();
        public int daysInThisMonths, weekDayOfTheFirstDay;
        public IEnumerable<int> Holidays= new List<int>();
        

        public void arrange()
        {
            var dateListFactory = new DateListFactory(31, new int[] { 2, 3, 9, 10, 16, 17, 23, 24, 30, 31 }, WardSets.allWards);
            dateList = dateListFactory.getDateList();
            var doctorListFactory = new DoctorListFactory();
            doctorList = doctorListFactory.getDoctorList();

            var totalDutyDayAnalyzer = new TotalDutyDayAnalyzer(dateList, doctorList);
            doctor_DayCountList = totalDutyDayAnalyzer.getDoctor_DayCountList();
        }
    }

    public class DateInformation
    {
        public int date;
        public DateType dateType;
        public DoctorInformation dutyDoctor;
        public WardType wardType;
    }

    public class DoctorInformation
    {
        public string ID;
        public string name;
        public IEnumerable<int> absoluteAvoidThisDay;
        public IEnumerable<int> absoluteWantThisDay;
        public IEnumerable<int> relativeAvoidThisDay;
        public IEnumerable<int> relativeWantThisDay;
        public WardType mainWard;
        public IEnumerable<WardType> capableOf;
        public DoctorType doctorType;
    }
    public class WardShiftInformation
    {
        public WardType ward;
        public int holidayShift;
        public int nonHolidayShift;
        public int availableDoctor;
    }
    
    public enum WardType
    {
        NICU, PICU, A093, A091, NBR
    }
    public static class WardSets
    {
        public static WardType[] allWards = new WardType[] { WardType.A091, WardType.A093, WardType.NBR, WardType.NICU, WardType.PICU };
        public static WardType[] NICU = new WardType[] { WardType.NICU };
    }
    public enum DateType
    {
        Holiday,
        Weekend,
        Workday
    }
    public enum DoctorType
    {
        PGY, R1, R2, R3, Fellow
    }
}
