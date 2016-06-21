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
        static Brush redBrush = new SolidColorBrush(Colors.Red);

        MainLogic mainLogic;
        public ViewModel(MainLogic mainLogic)
        {
            this.mainLogic = mainLogic;
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
                totalWorkHour_Color = brush;
            }
            private string _ID;
            public string ID
            {
                get { return _ID; }
                set
                {
                    _ID = value;
                    ID_Color = yellowBrush;
                    if(_ID.Length>1)
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

            private string _totalWorkHour;
            public string totalWorkHour
            {
                get { return _totalWorkHour; }
                set
                {
                    _totalWorkHour = value;
                    totalWorkHour_Color = yellowBrush;
                }
            }
            public Brush totalWorkHour_Color { get; set; }
            
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
                    mainWard = d.mainWard.ToString(),
                    capableOf = d.capableOf.getStringFromList(),
                    doctorType = d.doctorType.ToString(),
                };
                toAdd.setAllBrush(whiteBrush);
                doctorList.Add(toAdd);
            }
            OnPropertyChanged(nameof(doctorList));
        }

        public class classDateInformationView
        {
            public string ward { get; set; }
            public string[] date { get; set; }
            public classDateInformationView()
            {
                date = new string[31];
            }
        }
        public ObservableCollection<classDateInformationView> dateList;
        public void refreshDutyDay()
        {
            dateList = new ObservableCollection<classDateInformationView>();
            var toAdd = new classDateInformationView();
            toAdd.ward = "TEST";
            for (int i = 0; i < toAdd.date.Length; i++)
            {
                toAdd.date[i] = "T";
            }
            dateList.Add(toAdd);
            OnPropertyChanged(nameof(dateList));
        }

        public class WardShiftView
        {
            public string ward { get; set; }
            public string holidayShift { get; set; }
            public string nonHolidayShift { get; set; }
            public string availableDoctor { get; set; }
        }
        public ObservableCollection<WardShiftView> WardShiftList;
        public void refreshWardShiftList()
        {
            WardShiftList = new ObservableCollection<WardShiftView>();
            WardShiftView NICU = new WardShiftView()
            {
                ward = "NICU",
                holidayShift = "5",
                nonHolidayShift = "10",
                availableDoctor = "10"
            };
            WardShiftList.Add(NICU);
            OnPropertyChanged(nameof(WardShiftList));
        }

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
