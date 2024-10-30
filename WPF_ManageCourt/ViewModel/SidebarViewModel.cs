using System.Windows;
using System.Windows.Input;

namespace WPF_ManageCourt.ViewModel
{
    public class SidebarViewModel : BaseViewModel
    {
        public ICommand OpenMainManagementCommand { get; }
        public ICommand OpenUserManagementCommand { get; }

        public SidebarViewModel()
        {
            OpenMainManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new MainWindow()));
            OpenUserManagementCommand = new RelayCommand<object>((o) => true, _ => OpenWindow(new UserView()));
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
