using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using BusinessLogic.Interface;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Model;

namespace WPF_ManageCourt.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _loginService;

        public LoginViewModel(IUserService loginService)
        {
            _loginService = loginService;
            LoginCommand = new RelayCommand<object>((o)=>CanExecuteLogin(o), (o)=>ExecuteLogin(o));
            ForgotPasswordCommand = new RelayCommand<object>((o) => true, (o) => ShowForgotPasswordWindow());
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task ExecuteLogin(object parameter)
        {
            var user = await _loginService.AuthenticateUserAsync(Username, Password);

            if (user != null)
            {
                var login = (User) user;
                Application.Current.Properties["LoggedInUser"] = user;
                var userViewDashboard = new UserView();
                userViewDashboard.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Application.Current.Windows[0].Close();
        }

        private void ShowForgotPasswordWindow()
        {
            
            var forgotPasswordWindow = new ForgetPasswordWindow();
            forgotPasswordWindow.Show();
            Application.Current.Windows[0].Close();
        }
    }

}
