using System;
using System.Collections.Generic;
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
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var SA = new MainLogic();
            SA.arrange();
            DoctorListView.View.
        }
    }
}
