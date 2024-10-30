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
    using WPF_ManageCourt.ViewModel;

    namespace WPF_ManageCourt
    {
        /// <summary>
        /// Interaction logic for BookingManageWindow.xaml
        /// </summary>
        public partial class BookingManageWindow : Window
        {
            public BookingManageWindow()
            {
                InitializeComponent();
                var serviceProvider = App.ServiceProvider;
                DataContext = serviceProvider.GetService<BookingManagerViewModel>();
            }

            private void Header_Loaded(object sender, RoutedEventArgs e)
            {

            }


            private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (sender is ComboBox comboBox && comboBox.SelectedItem != null)
                {
                    var viewModel = DataContext as BookingManagerViewModel;
                    var selectedValue = comboBox.SelectedItem.ToString();
                }
            }

            private void ImportButton_Click(object sender, RoutedEventArgs e)
            {
                Button button = sender as Button;
                if (button?.ContextMenu != null)
                {
                    button.ContextMenu.PlacementTarget = button;
                    button.ContextMenu.IsOpen = true;
                }
            }

            private void ExportButton_Click(object sender, RoutedEventArgs e)
            {
                Button button = sender as Button;
                if (button?.ContextMenu != null)
                {
                    button.ContextMenu.PlacementTarget = button;
                    button.ContextMenu.IsOpen = true;
                }
            }
            private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                var dataGrid = sender as DataGrid;
                if (dataGrid.SelectedItem != null)
                {
                    var booking = dataGrid.SelectedItem as Booking;
                    var viewModel = DataContext as BookingManagerViewModel;
                    viewModel.SelectedBookingManager = booking;
                    viewModel.IsUpdateUserDialogOpen = true;
                }
            }
        }

    }