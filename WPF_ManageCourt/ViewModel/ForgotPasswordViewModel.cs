using BusinessLogic.Interface;
using Model;
using Repositories.Interface;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_ManageCourt.Services;
using Timer = System.Timers.Timer;


namespace WPF_ManageCourt.ViewModel
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly EmailService _emailService;
        private readonly IUserService _userService;
        private readonly IUserOtpService _otpService;

        private bool _isOtpScreenVisible;
        private bool _isSendOtpEnabled = true;
        private bool _isResendOtpEnabled = false;
        private string _sendOtpButtonText = "";
        private Timer _otpTimer;

        public bool IsOtpScreenVisible
        {
            get => _isOtpScreenVisible;
            set
            {
                _isOtpScreenVisible = value;
                OnPropertyChanged(nameof(IsOtpScreenVisible));
                OnPropertyChanged(nameof(IsSendOtpScreenVisible));
            }
        }

        public bool IsSendOtpScreenVisible => !_isOtpScreenVisible;
        public bool IsSendOtpEnabled
        {
            get => _isSendOtpEnabled;
            set
            {
                _isSendOtpEnabled = value;
                OnPropertyChanged(nameof(IsSendOtpEnabled));
            }
        }

        public bool IsResendOtpEnabled
        {
            get => _isResendOtpEnabled;
            set
            {
                _isResendOtpEnabled = value;
                OnPropertyChanged(nameof(IsResendOtpEnabled));
            }
        }

        public string SendOtpButtonText
        {
            get => _sendOtpButtonText;
            set
            {
                _sendOtpButtonText = value;
                OnPropertyChanged(nameof(SendOtpButtonText));
            }
        }

        public string Username { get; set; }
        public string EnteredOtp { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand SendOtpCommand { get; }
        public ICommand VerifyOtpCommand { get; }
        public ICommand SubmitNewPasswordCommand { get; }
        public ICommand ResendOtpCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public ForgotPasswordViewModel(EmailService emailService, IUserService userService, IUserOtpService otpService)
        {
            _emailService = emailService;
            _userService = userService;
            _otpService = otpService;
            IsOtpScreenVisible = false;

            SendOtpCommand = new RelayCommand<object>((o) => IsSendOtpEnabled, async (o) => await SendOtpAsync());
            VerifyOtpCommand = new RelayCommand<object>((o) => true, async (o) => await VerifyOtpAsync());
            SubmitNewPasswordCommand = new RelayCommand<object>((o) => true, async (o) => await SubmitNewPasswordAsync());
            BackToLoginCommand = new RelayCommand<object>((o) => true, (o) => BackToLogin());
            ResendOtpCommand = new RelayCommand<object>((o) => IsResendOtpEnabled, async (o) => await ResendOtpAsync());
        }

        private async Task ResendOtpAsync()
        {
            await SendOtpAsync();
            MessageBox.Show("OTP has been resent to your email.");
        }

        private async Task SendOtpAsync()
        {
            var user = await _userService.GetUserByUsernameAsync(Username);
            if (user == null)
            {
                MessageBox.Show("User not found: " + Username);
                return;
            }
            Application.Current.Properties["ForgetUser"] = user;
            var otpCode = new Random().Next(100000, 999999).ToString();
            await _otpService.SaveOtpAsync(user.UserId, otpCode);
            await _emailService.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code is: {otpCode}");
            MessageBox.Show("OTP has been sent to your email.");

            IsOtpScreenVisible = true;
            StartOtpCountdown();
        }

        private void StartOtpCountdown()
        {
            IsSendOtpEnabled = false;
            IsResendOtpEnabled = false;
            int countdown = 60;
            SendOtpButtonText = $"Retry in {countdown}s";

            _otpTimer = new Timer(1000);
            _otpTimer.Elapsed += (sender, e) =>
            {
                countdown--;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SendOtpButtonText = $"Retry in {countdown}s";
                });

                if (countdown <= 0)
                {
                    _otpTimer.Stop();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SendOtpButtonText = "OTP has expired";
                        IsSendOtpEnabled = true;
                        IsResendOtpEnabled = true;
                    });
                }
            };
            _otpTimer.Start();
        }

        private async Task VerifyOtpAsync()
        {
            var user = await _userService.GetUserByUsernameAsync(Username);
            if (user == null)
            {
                MessageBox.Show("User not found");
                return;
            }

            var isValidOtp = await _otpService.ValidateOtpAsync(user.UserId, EnteredOtp);
            if (isValidOtp)
            {
                MessageBox.Show("OTP verified! You can now reset your password.");
                IsOtpScreenVisible = true;
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.");
                IsOtpScreenVisible = false;
            }
        }

        private async Task SubmitNewPasswordAsync()
        {
            var user = (User)Application.Current.Properties["ForgetUser"];
            if (user == null)
            {
                MessageBox.Show("No user found.");
                return;
            }
            var isValidOtp = await _otpService.ValidateOtpAsync(user.UserId, EnteredOtp);
            if (isValidOtp)
            {
                if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    MessageBox.Show("Please fill in both password fields.");
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    MessageBox.Show("Passwords do not match. Please try again.");
                    return;
                }

                await _userService.UpdatePasswordAsync(user.UserId, NewPassword);
                MessageBox.Show("Password reset successfully.");

                BackToLogin();
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.");
                IsOtpScreenVisible = true;
            }
        }

        private void BackToLogin()
        {
            var loginWindow = new LoginWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
            foreach (Window window in Application.Current.Windows)
            {
                if (window is ForgetPasswordWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

    }
}
