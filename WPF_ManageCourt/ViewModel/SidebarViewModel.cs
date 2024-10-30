using System.Windows;
using System.Windows.Input;

namespace WPF_ManageCourt.ViewModel
{
    public class SidebarViewModel : BaseViewModel
    {
        public ICommand OpenMainManagementCommand { get; }
        public ICommand OpenUserManagementCommand { get; }
        public ICommand OpenBookingManagementCommand { get; }
        public ICommand OpenAccessoryManagementCommand { get; }
        public ICommand OpenDashboardManagementCommand { get; }
        public ICommand OpenCourtManagementCommand { get; }

        public ICommand OpenScheduleManagementCommand { get; }
        public SidebarViewModel()
        {
            OpenMainManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new MainWindow()));
            OpenUserManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new UserView()));
            OpenBookingManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new BookingManageWindow()));
            OpenAccessoryManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new AccessoryWindow()));
            OpenDashboardManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new DashboardWindow()));
            OpenCourtManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new CourtManageWindow()));
            OpenScheduleManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new ScheduleWindow()));
        }

        private void OpenWindow(Window window)
        {
            window.Show();
            foreach (Window openWindow in Application.Current.Windows)
            {
                if (openWindow != window && openWindow != Application.Current.MainWindow)
                {
                    openWindow.Close();
                }
            }
        }
    }
}
