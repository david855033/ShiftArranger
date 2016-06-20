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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainLogic.arrange();
            viewModel.refreshDoctorList();
            DoctorListView.ItemsSource = viewModel.doctorList;
        }
        private void DoctorView_SaveChange(object sender, RoutedEventArgs e)
        {
            if (viewModel.doctorList == null)
                return;
            var newList = new List<DoctorInformation>();
            bool error = false;
            foreach (var row in viewModel.doctorList)
            {
                var newDoctorInformation = new DoctorInformation();
                if (row.ID == "")
                {
                    error = true;
                    row.ID = "####";
                    DoctorListView.Items.Refresh();
                }

                newDoctorInformation.ID = row.ID;
                newDoctorInformation.name = row.name;

            }
        }

        private void Button_Test(object sender, RoutedEventArgs e)
        {
            viewModel.doctorList[0].ID = "1232131";
            DoctorListView.Items.Refresh();
            ChangeDataGridColor(DoctorListView, modifiedCells);
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


        string contentBeforeEdit;
        int editingRow, editingCol;
        List<Tuple<int, int>> modifiedCells = new List<Tuple<int, int>>();
        private void ListView_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var thisDataGrid = sender as DataGrid;
            var cellInfo = thisDataGrid.CurrentCell;
            editingCol = cellInfo.Column.DisplayIndex;
            editingRow = thisDataGrid.SelectedIndex;
            if (cellInfo != null)
            {
                var column = cellInfo.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var element = new FrameworkElement() { DataContext = cellInfo.Item };
                    BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
                    contentBeforeEdit = element.Tag?.ToString();
                }
            }
        }
        private void ListView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var thisDataGrid = sender as DataGrid;
            var cellInfo = thisDataGrid.CurrentCell;
            var newContent = ((TextBox)e.EditingElement).Text.ToString();
            if (contentBeforeEdit != newContent && contentBeforeEdit != null)
            {
                modifiedCells.Add(new Tuple<int, int>(editingRow, editingCol));
            }
            ChangeDataGridColor(thisDataGrid, modifiedCells);
        }

        private void ChangeDataGridColor(DataGrid datagrid, List<Tuple<int, int>> cells)
        {
            foreach (var c in cells)
            {
                int row = c.Item1;
                int col = c.Item2;
                DataGridRow thisRow =
                    datagrid.ItemContainerGenerator.ContainerFromItem(datagrid.Items[row]) as DataGridRow;
                DataGridCell thisColumnInthisRow =
                    datagrid.Columns[col].GetCellContent(thisRow) as DataGridCell;

                thisColumnInthisRow.Background = new SolidColorBrush(Colors.Yellow);
            }
        }






    }
}
