using Model;
using System.IO;
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
        public ICommand OpenProfileCommand { get; }
        public ICommand LogoutCommand { get; }


        private string _loggedInUserName;
        public string LoggedInUserName
        {
            get => _loggedInUserName;
            set
            {
                _loggedInUserName = value;
                OnPropertyChanged();
            }
        }

        private string _userProfileImage;
        public string UserProfileImage
        {
            get => _userProfileImage;
            set
            {
                _userProfileImage = value;
                OnPropertyChanged();
            }
        }

        public SidebarViewModel()
        {
            if (Application.Current.Properties["LoggedInUser"] is User loggedInUser)
            {
                LoggedInUserName = loggedInUser.FullName;
                UserProfileImage = "https://coffective.com/wp-content/uploads/2018/06/default-featured-image.png.jpg";
            }

            OpenMainManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new MainWindow()));
            OpenUserManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new UserView()));
            OpenBookingManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new BookingManageWindow()));
            OpenAccessoryManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new AccessoryWindow()));
            OpenDashboardManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new DashboardWindow()));
            OpenCourtManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new CourtManageWindow()));
            OpenScheduleManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new ScheduleWindow()));
            OpenProfileCommand = new RelayCommand<object>((_) => true, _ => OpenProfile());
            LogoutCommand = new RelayCommand<object>((o) => true, _ => Logout());
        }

        private void OpenProfile()
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Show();
            Application.Current.Windows[0].Close();
        }


        private void OpenWindow(Window window)
        {
            window.Show();
            Application.Current.Windows[0].Close();
        }

        private void Logout()
        {
            Application.Current.Properties["LoggedInUser"] = null;
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Application.Current.Windows[0].Close();
        }
    }
}
