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
    }

    public class DateInformation
    {
        public DateType[] dateType = new DateType[31];
        public DoctorInformation[] dutyDoctor = new DoctorInformation[31];
        public WardType wardType;
    }

    public class DoctorInformation
    {
        public string ID;
        public string name;
        public DoctorType doctorType;
        public WardType mainWard;
        public IEnumerable<WardType> capableOf;
        public int holidayDuty;
        public int nonHolidayDuty;
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
            newDoctor.absoluteWantThisDay = split[7].getIntListFromString(out fail);
            newDoctor.absoluteAvoidThisDay = split[8].getIntListFromString(out fail);
            newDoctor.relativeWantThisDay = split[9].getIntListFromString(out fail);
            newDoctor.relativeAvoidThisDay = split[10].getIntListFromString(out fail);
            return newDoctor;
        }
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
    public enum DoctorType
    {
        PGY, R1, R2, R3, Fellow
    }
}
