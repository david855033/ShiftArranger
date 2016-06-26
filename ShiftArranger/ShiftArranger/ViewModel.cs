using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShiftArranger
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Property Change Behavior
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        static Brush whiteBrush = new SolidColorBrush(Colors.White);
        static Brush yellowBrush = new SolidColorBrush(Colors.Yellow);
        static Brush blueBrush = new SolidColorBrush(Colors.LightBlue);
        static Brush redBrush = new SolidColorBrush(Colors.Red);
        static Brush greenBrush = new SolidColorBrush(Colors.LawnGreen);
        static Brush lightGreenBrush = new SolidColorBrush(Colors.YellowGreen);
        static Brush blackBrush = new SolidColorBrush(Colors.Black);

        MainLogic mainLogic;
        public ViewModel(MainLogic mainLogic)
        {
            this.mainLogic = mainLogic;
            _daysInThisMonth = 31;
            _firstWeekDayOfThisMonth = 1;
            _additionalHolidays = new List<int>();
        }

        public class DoctorInformationView
        {
            public void setAllBrush(Brush brush)
            {
                ID_Color = brush;
                name_Color = brush;
                absoluteAvoidThisDay_Color = brush;
                absoluteWantThisDay_Color = brush;
                relativeAvoidThisDay_Color = brush;
                relativeWantThisDay_Color = brush;
                mainWard_Color = brush;
                capableOf_Color = brush;
                doctorType_Color = brush;
                totalDuty_Color = brush;
                holidayDuty_Color = brush;
                nonHolidayDuty_Color = brush;
            }
            private string _ID;
            public string ID
            {
                get { return _ID; }
                set
                {
                    _ID = value;
                    ID_Color = yellowBrush;
                    if (_ID?.Length > 2)
                        ID_Color = redBrush;
                }
            }
            public Brush ID_Color { get; set; }

            private string _name;
            public string name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    name_Color = yellowBrush;
                }
            }
            public Brush name_Color { get; set; }

            private string _absoluteAvoidThisDay;
            public string absoluteAvoidThisDay
            {
                get { return _absoluteAvoidThisDay; }
                set
                {
                    _absoluteAvoidThisDay = value;
                    absoluteAvoidThisDay_Color = yellowBrush;
                }
            }
            public Brush absoluteAvoidThisDay_Color { get; set; }

            private string _absoluteWantThisDay;
            public string absoluteWantThisDay
            {
                get { return _absoluteWantThisDay; }
                set
                {
                    _absoluteWantThisDay = value;
                    absoluteWantThisDay_Color = yellowBrush;
                }
            }
            public Brush absoluteWantThisDay_Color { get; set; }

            private string _relativeAvoidThisDay;
            public string relativeAvoidThisDay
            {
                get { return _relativeAvoidThisDay; }
                set
                {
                    _relativeAvoidThisDay = value;
                    relativeAvoidThisDay_Color = yellowBrush;
                }
            }
            public Brush relativeAvoidThisDay_Color { get; set; }

            private string _relativeWantThisDay;
            public string relativeWantThisDay
            {
                get { return _relativeWantThisDay; }
                set
                {
                    _relativeWantThisDay = value;
                    relativeWantThisDay_Color = yellowBrush;
                }
            }
            public Brush relativeWantThisDay_Color { get; set; }

            private string _mainWard;
            public string mainWard
            {
                get { return _mainWard; }
                set
                {
                    _mainWard = value;
                    mainWard_Color = yellowBrush;
                }
            }
            public Brush mainWard_Color { get; set; }

            private string _capableOf;
            public string capableOf
            {
                get { return _capableOf; }
                set
                {
                    _capableOf = value;
                    capableOf_Color = yellowBrush;
                }
            }
            public Brush capableOf_Color { get; set; }

            private string _doctorType;
            public string doctorType
            {
                get { return _doctorType; }
                set
                {
                    _doctorType = value;
                    doctorType_Color = yellowBrush;
                }
            }
            public Brush doctorType_Color { get; set; }

            private string _totalDuty;
            public string totalDuty
            {
                get { return _totalDuty; }
                set
                {
                    _totalDuty = value;
                    totalDuty_Color = yellowBrush;
                }
            }
            public Brush totalDuty_Color { get; set; }

            private string _holidayDuty;
            public string holidayDuty
            {
                get { return _holidayDuty; }
                set
                {
                    _holidayDuty = value;
                    holidayDuty_Color = yellowBrush;
                }
            }
            public Brush holidayDuty_Color { get; set; }

            private string _nonHolidayDuty;
            public string nonHolidayDuty
            {
                get { return _nonHolidayDuty; }
                set
                {
                    _nonHolidayDuty = value;
                    nonHolidayDuty_Color = yellowBrush;
                }
            }
            public Brush nonHolidayDuty_Color { get; set; }

            public string arrangedHolidayDuty { get; set; }
            public string arrangedNonHolidayDuty { get; set; }
            public string arrangedTotalHolidayDuty { get; set; }
        }
        private ObservableCollection<DoctorInformationView> _doctorList;
        public ObservableCollection<DoctorInformationView> doctorList
        {
            get
            {
                return _doctorList;
            }
            set
            {
                _doctorList = value;
                OnPropertyChanged(nameof(doctorList));
            }
        }
        public void refreshDoctorList()
        {
            doctorList = new ObservableCollection<DoctorInformationView>();
            foreach (var d in mainLogic.doctorList)
            {
                var toAdd = new DoctorInformationView()
                {
                    ID = d.ID,
                    name = d.name,
                    absoluteAvoidThisDay = d.absoluteAvoidThisDay.getStringFromList(),
                    absoluteWantThisDay = d.absoluteWantThisDay.getStringFromList(),
                    relativeAvoidThisDay = d.relativeAvoidThisDay.getStringFromList(),
                    relativeWantThisDay = d.relativeWantThisDay.getStringFromList(),
                    totalDuty = (d.nonHolidayDuty + d.holidayDuty).ToString(),
                    holidayDuty = d.holidayDuty.ToString(),
                    nonHolidayDuty = d.nonHolidayDuty.ToString(),
                    mainWard = d.mainWard.ToString(),
                    capableOf = d.capableOf.getStringFromList(),
                    doctorType = d.doctorType.ToString(),
                    arrangedHolidayDuty = d.arrangedHolidayDuty.ToString(),
                    arrangedNonHolidayDuty = d.arrangedNonHolidayDuty.ToString(),
                    arrangedTotalHolidayDuty = (d.arrangedHolidayDuty + d.arrangedNonHolidayDuty).ToString()
                };
                toAdd.setAllBrush(whiteBrush);
                doctorList.Add(toAdd);
            }
            OnPropertyChanged(nameof(doctorList));
        }

        public class PropertyArray<T>
        {
            private T[] array;
            Brush[] brushArray;
            int daysInMonth;
            public PropertyArray(T[] array, Brush[] brushArray, int daysInMonth)
            {
                this.array = array;
                this.brushArray = brushArray;
                this.daysInMonth = daysInMonth;
            }
            public T this[int index]
            {
                get
                {
                    if(index>=0)
                        return array[index];
                    return array[0];
                }
                set
                {
                    array[index] = value;
                    if (index < daysInMonth) brushArray[index] = yellowBrush;
                }
            }
        }
        public class DateInformationView
        {
            public string ward { get; set; }
            string[] _date;
            public PropertyArray<string> dutyDoctorInDay { get; set; }
            public DateType[] dateType { get; set; }
            public Brush[] dutyDoctorInDay_Color { get; set; }
            public int daysInMonth;
            public DateInformationView(int daysInMonth)
            {
                this.daysInMonth = daysInMonth;
                _date = new string[31];
                dutyDoctorInDay_Color = new Brush[31];
                dateType = new DateType[31];
                dutyDoctorInDay = new PropertyArray<string>(_date, dutyDoctorInDay_Color, daysInMonth);
            }
            public void setAllBrush(Brush brush, string highLight)
            {
                for (int i = 0; i < 31; i++)
                {
                    dutyDoctorInDay_Color[i] = brush;
                    if (dateType[i] == DateType.Holiday)
                        dutyDoctorInDay_Color[i] = greenBrush;
                    if (dateType[i] == DateType.Weekend)
                        dutyDoctorInDay_Color[i] = lightGreenBrush;
                    if (i >= daysInMonth)
                        dutyDoctorInDay_Color[i] = blackBrush;
                    if (dutyDoctorInDay[i] == highLight)
                        dutyDoctorInDay_Color[i] = blueBrush;
                }
            }
            public void setAllBrush(Brush brush)
            {
                for (int i = 0; i < 31; i++)
                {
                    dutyDoctorInDay_Color[i] = brush;
                    if (dateType[i] == DateType.Holiday)
                        dutyDoctorInDay_Color[i] = greenBrush;
                    if (dateType[i] == DateType.Weekend)
                        dutyDoctorInDay_Color[i] = lightGreenBrush;
                    if (i >= daysInMonth)
                        dutyDoctorInDay_Color[i] = blackBrush;
                }
            }
        }
        public ObservableCollection<DateInformationView> dateList;
        public void refreshDateList()
        {
            dateList = new ObservableCollection<DateInformationView>();
            foreach (var date in mainLogic.dateList)
            {
                var toAdd = new DateInformationView(_daysInThisMonth);
                toAdd.ward = date.wardType.ToString();
                for (int i = 0; i < toAdd.daysInMonth; i++)
                {
                    toAdd.dateType = date.dateType;
                    if (date.dutyDoctor[i] == null)
                    {
                        toAdd.dutyDoctorInDay[i] = "";
                    }
                    else
                    {
                        toAdd.dutyDoctorInDay[i] = date.dutyDoctor[i];
                    }
                }
                toAdd.setAllBrush(whiteBrush);
                dateList.Add(toAdd);
            }
            OnPropertyChanged(nameof(dateList));
        }
        public void setHighLight(string v)
        {
            var newDateList = new ObservableCollection<DateInformationView>();
            foreach (var ward in dateList)
            {
                ward.setAllBrush(whiteBrush, v);
                newDateList.Add(ward);
            }
            dateList = newDateList;
            OnPropertyChanged(nameof(dateList));
        }

        int _daysInThisMonth;
        public string daysInThisMonth
        {
            get
            {
                return _daysInThisMonth.ToString();
            }
            set
            {
                bool fail = false;
                int v = value.getIntFromString(out fail);
                if (!fail && v >= 28 && v <= 31)
                {
                    _daysInThisMonth = v;
                }
                else
                {
                    _daysInThisMonth = 31;
                }
                OnPropertyChanged(nameof(daysInThisMonth));
                OnPropertyChanged(nameof(holidayCount));
                OnPropertyChanged(nameof(workdayCount));
                OnPropertyChanged(nameof(holidayDutyCount));
                OnPropertyChanged(nameof(workdayDutyCount));
            }
        }
        int _firstWeekDayOfThisMonth;
        public string firstWeekDayOfThisMonth
        {
            get
            {
                return _firstWeekDayOfThisMonth.ToString();
            }
            set
            {
                bool fail = false;
                int v = value.getIntFromString(out fail);
                if (!fail && v > 0 && v <= 7)
                {
                    _firstWeekDayOfThisMonth = v;
                }
                else
                {
                    _firstWeekDayOfThisMonth = 1;
                }
                OnPropertyChanged(nameof(firstWeekDayOfThisMonth));
                OnPropertyChanged(nameof(holidayCount));
                OnPropertyChanged(nameof(workdayCount));
                OnPropertyChanged(nameof(holidayDutyCount));
                OnPropertyChanged(nameof(workdayDutyCount));
            }
        }
        List<int> _additionalHolidays;
        public string additionalHolidays
        {
            get
            {
                return _additionalHolidays.getStringFromList();
            }
            set
            {
                bool fail = false;
                List<int> v = value.getIntListFromString(out fail);
                if (!fail)
                {
                    _additionalHolidays = v;
                }
                else
                {
                    _additionalHolidays = new List<int>();
                }
                OnPropertyChanged(nameof(additionalHolidays));
                OnPropertyChanged(nameof(holidayCount));
                OnPropertyChanged(nameof(workdayCount));
                OnPropertyChanged(nameof(holidayDutyCount));
                OnPropertyChanged(nameof(workdayDutyCount));
            }
        }
        public int holidayCount { get { return _additionalHolidays.Count; } }
        public int workdayCount { get { return _daysInThisMonth - _additionalHolidays.Count; } }
        public int holidayDutyCount { get { return holidayCount * WardSets.allWards.Count(); } }
        public int workdayDutyCount { get { return workdayCount * WardSets.allWards.Count(); } }

        public class WardShiftView
        {
            public string ward { get; set; }
            public string holidayShift { get; set; }
            public string nonHolidayShift { get; set; }
            public string availableHolidayDoctor { get; set; }
            public string availableWorkDayDoctor { get; set; }
        }
        public ObservableCollection<WardShiftView> wardShiftList;


        public class RankShiftSummaryView
        {
            public string doctorRank { get; set; }
            public string doctorCount { get; set; }
            public string nonHolidayShiftPerPerson { get; set; }
            public string holidayShiftPerPerson { get; set; }
            public string totalNonHolidayShift { get; set; }
            public string totalHolidayShift { get; set; }
        }
        public ObservableCollection<RankShiftSummaryView> RankShiftSummaryList;
        public void refreshRankShiftSummaryList()
        {
            RankShiftSummaryList = new ObservableCollection<RankShiftSummaryView>();
            RankShiftSummaryView R3 = new RankShiftSummaryView()
            {
                doctorRank = "R3",
                holidayShiftPerPerson = "1",
                nonHolidayShiftPerPerson = "2",
                doctorCount = "4"
            };
            RankShiftSummaryList.Add(R3);
            OnPropertyChanged(nameof(RankShiftSummaryList));
        }


    }

}
