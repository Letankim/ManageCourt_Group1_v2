using Microsoft.Extensions.DependencyInjection;
using Model;
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
using System.Windows.Shapes;
using WPF_ManageCourt.ViewModel;

namespace WPF_ManageCourt
{
    /// <summary>
    /// Interaction logic for ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private readonly ScheduleCourtModel _scheduleCourtModel;

        public ScheduleWindow()
        {
            InitializeComponent();
            var serviceProvider = App.ServiceProvider;
            DataContext = serviceProvider.GetService<ScheduleCourtModel>();
            LoadTimeSlots();
        }

        public ObservableCollection<string> GetTimeSlots()
        {
            return new ObservableCollection<string>
            {
                "0:00 - 1:00",
                "4:00 - 5:00",
                "5:00 - 6:00",
                "6:00 - 7:00",
                "7:00 - 8:00",
                "8:00 - 9:00",
                "9:00 - 10:00",
                "10:00 - 11:00",
                "11:00 - 12:00",
                "12:00 - 13:00",
                "13:00 - 14:00",
                "14:00 - 15:00",
                "15:00 - 16:00",
                "16:00 - 17:00",
                "17:00 - 18:00",
                "18:00 - 19:00",
                "19:00 - 20:00",
                "20:00 - 21:00",
                "21:00 - 22:00",
                "22:00 - 23:00",
                "23:00 - 24:00"
            };
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem != null)
            {
                var schedule = dataGrid.SelectedItem as CourtSchedule;
                var viewModel = DataContext as ScheduleCourtModel;
                viewModel.SelectedSchedule = schedule;
                viewModel.IsUpdateScheduleDialogOpen = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem != null)
            {
                var viewModel = DataContext as ScheduleCourtModel;
                var selectedValue = comboBox.SelectedItem.ToString();
            }
        }

        private void LoadTimeSlots()
        {
            var viewModel = DataContext as ScheduleCourtModel;
            TimeSlotAdd.ItemsSource = GetTimeSlots();
            TimeSlotUpdate.ItemsSource = GetTimeSlots();
        }



        //private async void AddButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var newSchedule = new CourtSchedule
        //    {
        //        CourtId = int.Parse(CourtIdTextBox.Text),
        //        Date = DateOnly.FromDateTime(DateDatePicker.SelectedDate ?? DateTime.Now),
        //        TimeSlot = TimeSlotComboBox.Text.ToString(),
        //        IsAvailable = IsAvailableCheckBox.IsChecked ?? false
        //    };

        //    if (await _scheduleCourtModel.AddSchedule(newSchedule))
        //    {
        //        MessageBox.Show("Lịch đặt sân đã được thêm.");
        //        ClearForm();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Thêm lịch đặt sân không thành công");
        //    }
        //}

        //private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(CourtIdTextBox.Text, out int courtId))
        //    {
        //        var success = await _scheduleCourtModel.EditSchedule(
        //            new CourtSchedule
        //            {
        //                CourtId = courtId,
        //                Date = DateOnly.FromDateTime(DateDatePicker.SelectedDate ?? DateTime.Now),
        //                TimeSlot = TimeSlotComboBox.Text,
        //                IsAvailable = IsAvailableCheckBox.IsChecked ?? false
        //            }
        //        );

        //        if (success)
        //        {
        //            MessageBox.Show("Lịch đặt sân đã được cập nhật.");
        //            ClearForm();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Lịch đặt sân không tồn tại để cập nhật.");
        //        }
        //    }
        //}

        //private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(CourtIdTextBox.Text, out int courtId))
        //    {
        //        var success = await _scheduleCourtModel.DeleteSchedule(
        //            courtId
        //        );

        //        if (success)
        //        {
        //            MessageBox.Show("Lịch đặt sân đã được xóa.");
        //            ClearForm();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Không tìm thấy lịch đặt sân để xóa.");
        //        }
        //    }
        //}

        //private void ClearForm()
        //{
        //    CourtIdTextBox.Clear();
        //    DateDatePicker.SelectedDate = null;
        //    TimeSlotComboBox.SelectedIndex = -1;
        //    IsAvailableCheckBox.IsChecked = false;
        //}
    }
}
