using Microsoft.Extensions.DependencyInjection;
using Model;
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
using System.Windows.Shapes;

namespace WPF_ManageCourt
{
    /// <summary>
    /// Interaction logic for CourtManageWindow.xaml
    /// </summary>
    public partial class CourtManageWindow : Window
    {
        public CourtManageWindow()
        {
            InitializeComponent();
            var serviceProvider = App.ServiceProvider;
            DataContext = serviceProvider.GetService<CourtViewModel>();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem != null)
            {
                var court = dataGrid.SelectedItem as BadmintonCourt;
                var viewModel = DataContext as CourtViewModel;
                viewModel.SelectedCourt = court;
                viewModel.IsUpdateCourtDialogOpen = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
