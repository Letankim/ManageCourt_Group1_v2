using BusinessLogic.Interface;
using Model;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_ManageCourt.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {
        private User _user;
        private IUserService _userService;

        public string Username => _user.Username;

        public string FullName
        {
            get => _user.FullName;
            set
            {
                _user.FullName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _user.Email;
            set
            {
                _user.Email = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => _user.Phone;
            set
            {
                _user.Phone = value;
                OnPropertyChanged();
            }
        }

        public string Role => _user.Role;

        public ICommand SaveProfileCommand { get; }
        public ICommand CancelProfileCommand { get; }

        public string NewPassword { get; set; }

        public ProfileViewModel(IUserService userService)
        {
            _userService = userService;
            _user = (User) Application.Current.Properties["LoggedInUser"];
            if (_user == null)
            {
                Application.Current.MainWindow?.Close();
                return;
            }

            SaveProfileCommand = new RelayCommand<object>((_) => true, _ => SaveProfile());
            CancelProfileCommand = new RelayCommand<object>((_) => true, _ => CancelProfile());
        }

        private async Task SaveProfile()
        {
            if (_user != null)
            {
                _user.FullName = FullName;
                _user.Email = Email;
                _user.Phone = Phone;

                if (NewPassword != null && !string.IsNullOrWhiteSpace(NewPassword))
                {
                    _user.Password = EncryptPassword(NewPassword);
                }
                await _userService.UpdateUserAsync(_user);
                MessageBox.Show("Thông tin hồ sơ đã được cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelProfile();
            }
        }

        private void CancelProfile()
        {
            var userWindow = new UserView();
            userWindow.Show();
            Application.Current.Windows[0].Close();
        }
        private string EncryptPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
