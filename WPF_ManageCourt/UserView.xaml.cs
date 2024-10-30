using BusinessLogic.Interface;
using Microsoft.Extensions.DependencyInjection;
using Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_ManageCourt.ViewModel;

namespace WPF_ManageCourt
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        public UserView()
        {
            InitializeComponent();
            var serviceProvider = App.ServiceProvider;
            DataContext = serviceProvider.GetService<UserViewModel>();
        }


        private void PasswordBoxControl_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                var viewModel = this.DataContext as UserViewModel;
                viewModel.SetPassword(passwordBox.Password); 
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem != null)
            {
                var user = dataGrid.SelectedItem as User;
                var viewModel = DataContext as UserViewModel;
                viewModel.SelectedUser = user;
                viewModel.IsUpdateUserDialogOpen = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem != null)
            {
                var viewModel = DataContext as UserViewModel;
                var selectedValue = comboBox.SelectedItem.ToString();
                viewModel.SelectedUser.IsEnabled = selectedValue.Equals("Active", StringComparison.OrdinalIgnoreCase);
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
    }
}
