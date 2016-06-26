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
            try
            {
                LoadDoctorListFrom(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\default.sdc");
            }
            catch
            {
                GenerateDefaultDoctorList();
            }
            try
            {
                LoadDateListFrom(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\default.sdt");
            }
            catch
            {
                Initialize_WorkDay(this, new RoutedEventArgs());
            }
        }

        private void GenerateDefaultDoctorList()
        {
            var newDoctorList = new List<DoctorInformation>();

            newDoctorList.Add(new DoctorInformation()
            {
            });

            mainLogic.doctorList = newDoctorList;
            viewModel.refreshDoctorList();
            DoctorListView.ItemsSource = viewModel.doctorList;
            DoctorListView.Items.Refresh();
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
                newDoctorList.Sort();
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
            Doctor_SaveDialog.Filter = "sdc files (*.sdc)|*.sdc";
            Doctor_SaveDialog.FilterIndex = 1;
            Doctor_SaveDialog.RestoreDirectory = true;
            if (Doctor_SaveDialog.ShowDialog() == true)
            {
                SaveDoctorListTo(Doctor_SaveDialog.FileName);
            }
        }
        private void SaveDoctorListTo(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.Default))
            {
                foreach (var doctor in mainLogic.doctorList)
                {
                    sw.WriteLine(doctor.ToString());
                }
            }
        }
        Microsoft.Win32.OpenFileDialog Doctor_LoadDialog = new Microsoft.Win32.OpenFileDialog();
        private void LoadDoctorList(object sender, RoutedEventArgs e)
        {
            Doctor_LoadDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Doctor_LoadDialog.Filter = "sdc files (*.sdc)|*.sdc";
            Doctor_LoadDialog.FilterIndex = 1;
            Doctor_LoadDialog.RestoreDirectory = true;
            if (Doctor_LoadDialog.ShowDialog() == true)
            {
                LoadDoctorListFrom(Doctor_LoadDialog.FileName);
            }
        }
        private void LoadDoctorListFrom(string path)
        {
            var newDoctorList = new List<DoctorInformation>();
            using (var sr = new StreamReader(path, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    newDoctorList.Add(DoctorInformation.loadFromString(line));
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
                    if (dateInViewModel.dutyDoctorInDay[i] != "")
                    {
                        if (toAdd == null)
                        {
                            allPassed = false;
                        }
                        else
                        {
                            toAdd.dutyDoctor[i] = mainLogic.doctorList.FirstOrDefault(x => x.ID == dateInViewModel.dutyDoctorInDay[i])?.ID;
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
        private void dateListView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            if (dateListView.SelectedCells.Count > 0)
            {
                var c = dateListView.SelectedCells.First();
                string ID = (c.Item as ViewModel.DateInformationView).dutyDoctorInDay[c.Column.DisplayIndex - 1];

                viewModel.setHighLight(ID);
            }

            dateListView.ItemsSource = viewModel.dateList;
            dateListView.Items.Refresh();
        }
        Microsoft.Win32.SaveFileDialog Date_SaveDialog = new Microsoft.Win32.SaveFileDialog();
        private void SaveDateList(object sender, RoutedEventArgs e)
        {
            Date_SaveDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Date_SaveDialog.Filter = "sdt files (*.sdt)|*.sdt";
            Date_SaveDialog.FilterIndex = 1;
            Date_SaveDialog.RestoreDirectory = true;
            if (Date_SaveDialog.ShowDialog() == true)
            {
                SaveDateListTo(Date_SaveDialog.FileName);
            }
        }
        private void SaveDateListTo(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.Default))
            {
                foreach (var date in mainLogic.dateList)
                {
                    sw.WriteLine(date.ToString());
                }
                sw.WriteLine($"[daysIntheMonth]={viewModel.daysInThisMonth}");
                sw.WriteLine($"[firstWeekDay]={viewModel.firstWeekDayOfThisMonth}");
                sw.WriteLine($"[holiday]={viewModel.additionalHolidays}");
            }
        }
        Microsoft.Win32.OpenFileDialog Date_LoadDialog = new Microsoft.Win32.OpenFileDialog();
        private void LoadDateList(object sender, RoutedEventArgs e)
        {
            Date_LoadDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Date_LoadDialog.Filter = "sdt files (*.sdt)|*.sdt";
            Date_LoadDialog.FilterIndex = 1;
            Date_LoadDialog.RestoreDirectory = true;
            if (Date_LoadDialog.ShowDialog() == true)
            {
                LoadDateListFrom(Date_LoadDialog.FileName);
            }
        }
        private void LoadDateListFrom(string path)
        {
            var newDateList = new List<DateInformation>();
            using (var sr = new StreamReader(path, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.Split('=')[0] == "[daysIntheMonth]")
                    {
                        viewModel.daysInThisMonth = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "[firstWeekDay]")
                    {
                        viewModel.firstWeekDayOfThisMonth = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "[holiday]")
                    {
                        viewModel.additionalHolidays = line.Split('=')[1];
                    }
                    else
                    {
                        newDateList.Add(DateInformation.loadFromString(line));
                    }
                }
            }
            mainLogic.dateList = newDateList;
            viewModel.refreshDateList();
            dateListView.ItemsSource = viewModel.dateList;
            dateListView.Items.Refresh();
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
            for (int i = 0; i < viewModel.daysInThisMonth.getIntFromString(out fail); i++)
            {
                if (i % 7 == (7 - firstWeekDay - 1) || i % 7 == (7 - firstWeekDay))
                {
                    Holidays.Add(i + 1);
                }
            }
            viewModel.additionalHolidays = Holidays.getStringFromList();
        }
        private void DoctorView_AddDoctor(object sender, RoutedEventArgs e)
        {
            viewModel.doctorList.Add(new ViewModel.DoctorInformationView());
        }
        private void DoctorView_DeleteDoctor(object sender, RoutedEventArgs e)
        {
            int index = new List<ViewModel.DoctorInformationView>(viewModel.doctorList).
                FindIndex(x => x.ID == DeleteID.Text);
            if (index < 0)
                index = new List<ViewModel.DoctorInformationView>(viewModel.doctorList).
                FindIndex(x => x.ID == "" || x.ID == null);
            if (index >= 0)
                viewModel.doctorList.RemoveAt(index);
        }



        private void Arrange(object sender, RoutedEventArgs e)
        {
            bool fail;
            mainLogic.daysInThisMonths = viewModel.daysInThisMonth.getIntFromString(out fail);
            mainLogic.weekDayOfTheFirstDay = viewModel.firstWeekDayOfThisMonth.getIntFromString(out fail);
            mainLogic.Holidays = viewModel.additionalHolidays.getIntListFromString(out fail);
            mainLogic.arrange();
            viewModel.refreshDateList();
            viewModel.refreshDoctorList();

            dateListView.ItemsSource = viewModel.dateList;
            DoctorListView.ItemsSource = viewModel.doctorList;
        }

        private void Window_Unloaded(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveDoctorListTo(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\default.sdc");
            SaveDateListTo(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\default.sdt");
        }
        private void CalculateWardShiftList(object sender, RoutedEventArgs e)
        {
            var NewWardShiftList = new ObservableCollection<ViewModel.WardShiftView>();
            foreach (var ward in WardSets.allWards)
            {
                bool fail;
                var newWardShift = new ViewModel.WardShiftView();
                newWardShift.ward = ward.ToString();
                newWardShift.holidayShift = holidayCountView.Text;
                newWardShift.nonHolidayShift = workdayCountView.Text;
                newWardShift.availableHolidayDoctor =
                  (from q in mainLogic.doctorList
                   where q.mainWard == ward
                   select q.holidayDuty).Sum().ToString();
                newWardShift.availableWorkDayDoctor =
                  (from q in mainLogic.doctorList
                   where q.mainWard == ward
                   select q.nonHolidayDuty).Sum().ToString();
                NewWardShiftList.Add(newWardShift);
            }
            viewModel.wardShiftList = NewWardShiftList;
            WardShiftListView.ItemsSource = viewModel.wardShiftList;
            WardShiftListView.Items.Refresh();
        }




    }
}
