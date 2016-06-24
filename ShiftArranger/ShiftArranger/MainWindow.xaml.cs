using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ShiftArranger
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        static Brush whiteBrush = new SolidColorBrush(Colors.White);
        static Brush yellowBrush = new SolidColorBrush(Colors.Yellow);
        static Brush redBrush = new SolidColorBrush(Colors.Red);

        MainLogic mainLogic;
        ViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            mainLogic = new MainLogic();
            viewModel = new ViewModel(mainLogic);
            this.DataContext = viewModel;
        }

        private void Initialize_Doctor_List(object sender, RoutedEventArgs e)
        {
            mainLogic.initializedDoctors();
            viewModel.refreshDoctorList();
            DoctorListView.ItemsSource = viewModel.doctorList;
        }

        private void DoctorView_SaveChange(object sender, RoutedEventArgs e)
        {
            if (viewModel.doctorList == null) return;

            var newDoctorList = new List<DoctorInformation>();
            bool allPassed = true;
            foreach (var doctorInViewModel in viewModel.doctorList)
            {
                bool fail;
                var toAdd = new DoctorInformation();

                toAdd.ID = doctorInViewModel.ID;
                toAdd.name = doctorInViewModel.name;

                toAdd.mainWard = doctorInViewModel.mainWard.getWardFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.mainWard_Color = redBrush;
                    allPassed = false;
                }

                toAdd.capableOf = doctorInViewModel.capableOf.getWardListFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.capableOf_Color = redBrush;
                    allPassed = false;
                }

                toAdd.doctorType = doctorInViewModel.doctorType.getDoctorTypeFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.doctorType_Color = redBrush;
                    allPassed = false;
                }

                toAdd.holidayDuty = doctorInViewModel.holidayDuty.getIntFromString(out fail);
                if (fail || toAdd.holidayDuty > 3)
                {
                    doctorInViewModel.holidayDuty_Color = redBrush;
                    allPassed = false;
                }

                toAdd.nonHolidayDuty = doctorInViewModel.nonHolidayDuty.getIntFromString(out fail);
                if (fail || toAdd.nonHolidayDuty > 10)
                {
                    doctorInViewModel.nonHolidayDuty_Color = redBrush;
                    allPassed = false;
                }

                toAdd.absoluteAvoidThisDay = doctorInViewModel.absoluteAvoidThisDay.getIntListFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.absoluteAvoidThisDay_Color = redBrush;
                    allPassed = false;
                }

                toAdd.absoluteWantThisDay = doctorInViewModel.absoluteWantThisDay.getIntListFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.absoluteWantThisDay_Color = redBrush;
                    allPassed = false;
                }

                toAdd.relativeAvoidThisDay = doctorInViewModel.relativeAvoidThisDay.getIntListFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.relativeAvoidThisDay_Color = redBrush;
                    allPassed = false;
                }

                toAdd.relativeWantThisDay = doctorInViewModel.relativeWantThisDay.getIntListFromString(out fail);
                if (fail)
                {
                    doctorInViewModel.relativeWantThisDay_Color = redBrush;
                    allPassed = false;
                }

                if (allPassed)
                {
                    newDoctorList.Add(toAdd);
                }
            }
            if (allPassed)
            {
                mainLogic.doctorList = newDoctorList;
                viewModel.refreshDoctorList();
            }
            DoctorListView.ItemsSource = viewModel.doctorList;
            DoctorListView.Items.Refresh();

        }

        Microsoft.Win32.SaveFileDialog Doctor_SaveDialog = new Microsoft.Win32.SaveFileDialog();
        private void SaveDoctorList(object sender, RoutedEventArgs e)
        {
            Doctor_SaveDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Doctor_SaveDialog.Filter = "sav files (*.sav)|*.sav";
            Doctor_SaveDialog.FilterIndex = 1;
            Doctor_SaveDialog.RestoreDirectory = true;
            if (Doctor_SaveDialog.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(Doctor_SaveDialog.FileName, false, Encoding.Default))
                {
                    foreach (var doctor in mainLogic.doctorList)
                    {
                        sw.WriteLine(doctor.ToString());
                    }
                }
            }
        }
        Microsoft.Win32.OpenFileDialog Doctor_LoadDialog = new Microsoft.Win32.OpenFileDialog();
        private void LoadDoctorList(object sender, RoutedEventArgs e)
        {
            Doctor_LoadDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Doctor_LoadDialog.Filter = "sav files (*.sav)|*.sav";
            Doctor_LoadDialog.FilterIndex = 1;
            Doctor_LoadDialog.RestoreDirectory = true;
            var newDoctorList = new List<DoctorInformation>();
            if (Doctor_LoadDialog.ShowDialog() == true)
            {
                using (var sr = new StreamReader(Doctor_LoadDialog.FileName, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        newDoctorList.Add(DoctorInformation.loadFromString(line));
                    }
                }
            }
            mainLogic.doctorList = newDoctorList;
            viewModel.refreshDoctorList();
            DoctorListView.ItemsSource = viewModel.doctorList;
            DoctorListView.Items.Refresh();
        }

        private void Initialize_WorkDay(object sender, RoutedEventArgs e)
        {
            bool fail;
            mainLogic.daysInThisMonths = viewModel.daysInThisMonth.getIntFromString(out fail);
            mainLogic.weekDayOfTheFirstDay = viewModel.firstWeekDayOfThisMonth.getIntFromString(out fail);
            mainLogic.Holidays = viewModel.additionalHolidays.getIntListFromString(out fail);
            mainLogic.initializeDate();
            viewModel.refreshDateList();
            dateListView.ItemsSource = viewModel.dateList;
        }


        private void DateList_SaveChange(object sender, RoutedEventArgs e)
        {
            if (viewModel.dateList == null) return;
            var newDateList = new List<DateInformation>();
            bool allPassed = true;
            foreach (var dateInViewModel in viewModel.dateList)
            {
                bool fail;
                var toAdd = new DateInformation();

                toAdd.wardType = dateInViewModel.ward.getWardFromString(out fail);
                if (fail)
                {
                    allPassed = false;
                }

                for (int i = 0; i < mainLogic.daysInThisMonths; i++)
                {
                    if (dateInViewModel.date[i] != "")
                    {
                        if (toAdd == null)
                        {
                            allPassed = false;
                        }
                        else
                        {
                            toAdd.dutyDoctor[i] = mainLogic.doctorList.FirstOrDefault(x => x.ID == dateInViewModel.date[i]);
                        }
                    }
                    toAdd.dateType = dateInViewModel.dateType;
                }

                if (allPassed)
                {
                    newDateList.Add(toAdd);
                }
            }
            if (allPassed)
            {
                mainLogic.dateList = newDateList;
                viewModel.refreshDateList();
            }
            dateListView.ItemsSource = viewModel.dateList;
            dateListView.Items.Refresh();

        }
        private void SaveDateList(object sender, RoutedEventArgs e)
        {

        }

        private void LoadDateList(object sender, RoutedEventArgs e)
        {

        }




        private void Button_AssignDuty(object sender, RoutedEventArgs e)
        {
            viewModel.refreshDateList();
            dateListView.ItemsSource = viewModel.dateList;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            viewModel.refreshWardShiftList();
            WardShiftListView.ItemsSource = viewModel.WardShiftList;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            viewModel.refreshRankShiftSummaryList();
            RankShiftSummaryViewList.ItemsSource = viewModel.RankShiftSummaryList;
        }


        bool edited = false;
        private void ListView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (edited)
            {
                try
                {
                    (sender as DataGrid).Items.Refresh();
                    edited = false;
                }
                catch
                {
                    edited = true;
                }
            }

        }
        private void ListView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            edited = true;
        }

        private void GenerateWeekends(object sender, RoutedEventArgs e)
        {
            bool fail;
            int firstWeekDay = viewModel.firstWeekDayOfThisMonth.getIntFromString(out fail);
            List<int> Holidays = new List<int>();
            for (int i = 0; i < 31; i++)
            {
                if (i % 7 == (7 - firstWeekDay - 1) || i % 7 == (7 - firstWeekDay))
                {
                    Holidays.Add(i + 1);
                }
            }
            viewModel.additionalHolidays = Holidays.getStringFromList();
        }


    }
}
