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

namespace ShiftArranger
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        MainLogic mainLogic;
        ViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            mainLogic = new MainLogic();
            viewModel = new ViewModel(mainLogic);
        }

        private void Initialize_Doctor_List(object sender, RoutedEventArgs e)
        {
            mainLogic.arrange();
            viewModel.refreshDoctorList();
            DoctorListView.ItemsSource = viewModel.doctorList;
        }

        private void DoctorView_SaveChange(object sender, RoutedEventArgs e)
        {
            if (viewModel.doctorList == null) return;
            bool canSave = true;

            if (canSave)
            {
                var newDoctorList = new List<DoctorInformation>();
                bool fail = false;
                foreach (var doctorInViewModel in viewModel.doctorList)
                {
                    var toAdd = new DoctorInformation();


                    toAdd.ID = doctorInViewModel.ID;
                    toAdd.name = doctorInViewModel.name;
                    toAdd.mainWard = doctorInViewModel.mainWard.getWardFromString(out fail);
                    toAdd.capableOf = doctorInViewModel.capableOf.getWardListFromString(out fail);

                    newDoctorList.Add(toAdd);
                }

            }
        }

        private void Button_Test(object sender, RoutedEventArgs e)
        {
            viewModel.doctorList[0].ID = "1232131";
            DoctorListView.Items.Refresh();
        }


        private void Button_AssignDuty(object sender, RoutedEventArgs e)
        {
            viewModel.refreshDutyDay();
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
    }
}
