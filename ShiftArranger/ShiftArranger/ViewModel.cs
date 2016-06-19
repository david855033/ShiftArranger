﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        MainLogic mainLogic;
        public ViewModel(MainLogic mainLogic)
        {
            this.mainLogic = mainLogic;
        }

        public ObservableCollection<DoctorInformationView> doctorList;
        public void refreshDoctorList()
        {
            doctorList = new ObservableCollection<DoctorInformationView>();
            foreach (var d in mainLogic.doctorList)
            {
                doctorList.Add(
                   new DoctorInformationView()
                   {
                       ID = d.ID,
                       absoluteAvoidThisDay = d.absoluteAvoidThisDay.getStringFromList(),
                       absoluteWantThisDay = d.absoluteWantThisDay.getStringFromList(),
                       relativeAvoidThisDay = d.relativeAvoidThisDay.getStringFromList(),
                       relativeWantThisDay = d.relativeWantThisDay.getStringFromList(),
                       mainWard = d.mainWard.ToString(),
                       capableOf = d.capableOf.getStringFromList(),
                       doctorType = d.doctorType.ToString()
                   }
                   );
            }
            OnPropertyChanged(nameof(doctorList));
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

    public class RankShiftSummaryView
    {
        public string doctorRank { get; set; }
        public string doctorCount { get; set; }
        public string nonHolidayShiftPerPerson { get; set; }
        public string holidayShiftPerPerson { get; set; }
        public string totalNonHolidayShift { get; set; }
        public string totalHolidayShift { get; set; }
    }
    public class WardShiftView
    {
        public string ward { get; set; }
        public string holidayShift { get; set; }
        public string nonHolidayShift { get; set; }
        public string availableDoctor { get; set; }
    }
    public class DoctorInformationView
    {
        public string ID { get; set; }
        public string absoluteAvoidThisDay { get; set; }
        public string absoluteWantThisDay { get; set; }
        public string relativeAvoidThisDay { get; set; }
        public string relativeWantThisDay { get; set; }
        public string mainWard { get; set; }
        public string capableOf { get; set; }
        public string doctorType { get; set; }
        public string totalDuty { get; set; }
        public string holidayDuty { get; set; }
        public string nonHolidayDuty { get; set; }
        public string totalWorkHour { get; set; }
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
}