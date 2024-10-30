using BusinessLogic.Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;

namespace WPF_ManageCourt.ViewModel
{
    public class BookingManagerViewModel : BaseViewModel
    {
        private readonly IBookingService _bookingService;
        private ObservableCollection<Booking> _booking; // Collection of users
        private ObservableCollection<string> _statusPaymentOptions;
        private ObservableCollection<string> _statusBookingOptions;
        private ObservableCollection<string> _statusPaymentMethodOptions;
        private Booking _selectedBooking; // Currently selected user
        private bool _isHideId; // Flag for hiding user ID
        private bool _isAddUserDialogOpen; // Flag for controlling dialog visibility
        private bool _isUpdateUserDialogOpen; // Flag for controlling update dialog visibility
        private string message;
        private bool _isShowMessageDialog;
        private readonly User _currentUser;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public ObservableCollection<string> StatusBookingOptions { get; set; } = new ObservableCollection<string> { "NoShow", "Confirmed", "Cancelled" };
        public ObservableCollection<string> StatusPaymentOptions { get; set; } = new ObservableCollection<string> { "Completed", "Pending" };
        public ObservableCollection<string> StatusPaymentMethodOptions { get; set; } = new ObservableCollection<string> { "Online", "AftefPlay" };

        public BookingManagerViewModel(IBookingService bookingService)

        {
            _bookingService = bookingService;
            _currentUser = (User)Application.Current.Properties["LoggedInUser"];
            ShowSuccessMessage("Users imported successfully." + _currentUser.UserId);
            SelectedBookingManager = new Booking();
            InitializeCommands();
            Load();
        }

        #region Properties
        public ObservableCollection<Booking> Bookings
        {
            get => _booking;
            set
            {
                _booking = value;
                OnPropertyChanged(nameof(Bookings));
                IsUsersEmpty = _booking == null || _booking.Count == 0;
            }
        }



        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsHideId
        {
            get => _isHideId;
            set
            {
                _isHideId = value;
                OnPropertyChanged(nameof(IsHideId));
            }
        }

        public bool IsAddUserDialogOpen
        {
            get => _isAddUserDialogOpen;
            set
            {
                _isAddUserDialogOpen = value;
                OnPropertyChanged(nameof(IsAddUserDialogOpen));
            }
        }

        public bool IsShowMessageDialog
        {
            get => _isShowMessageDialog;
            set
            {
                _isShowMessageDialog = value;
                OnPropertyChanged(nameof(IsShowMessageDialog));
            }
        }

        public bool IsUpdateUserDialogOpen
        {
            get => _isUpdateUserDialogOpen;
            set
            {
                _isUpdateUserDialogOpen = value;
                OnPropertyChanged(nameof(IsUpdateUserDialogOpen));
            }
        }

        public Booking SelectedBookingManager
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged(nameof(SelectedBookingManager));
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }
        private bool _isUsersEmpty;

        public bool IsUsersEmpty
        {
            get => _isUsersEmpty;
            set
            {
                _isUsersEmpty = value;
                OnPropertyChanged(nameof(IsUsersEmpty));
            }
        }

        private bool _isExportMenuOpen;
        public bool IsExportMenuOpen
        {
            get => _isExportMenuOpen;
            set
            {
                _isExportMenuOpen = value;
                OnPropertyChanged(nameof(IsExportMenuOpen));
            }
        }


        #endregion

        #region Commands

        public ICommand LoadedWindowCommand { get; private set; }
        public ICommand OpenAddUserDialogCommand { get; private set; }
        public ICommand AddUserCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand RowSelectedCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }

        public ICommand ShowExportCommand { get; private set; }
        #endregion

        #region Methods

        // Initialize all commands
        private void InitializeCommands()
        {
            LoadedWindowCommand = new RelayCommand<object>(_ => true, _ => Load());
            OpenAddUserDialogCommand = new RelayCommand<object>(_ => true, _ => OpenAddUserDialog());
            EditCommand = new RelayCommand<object>(_ => SelectedBookingManager != null, _ => EditBooking());
            RowSelectedCommand = new RelayCommand<object>(_ => true, _ => ShowDetailBooking());
            SearchCommand = new RelayCommand<object>(_ => true, _ => SearchBookings());
            ImportCommand = new RelayCommand<string>(_ => true, format => ImportBooking(format));
            ExportCommand = new RelayCommand<string>(_ => true, format => ExportBooking(format));
            ShowExportCommand = new RelayCommand<string>(_ => true, format => ShowExportOptions());
        }

        private void ShowExportOptions()
        {
            IsExportMenuOpen = true;
        }

        private async void ImportBooking(string format)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = format == "json" ? "JSON Files (*.json)|*.json" : "Excel Files (*.xlsx)|*.xlsx";
                if (dialog.ShowDialog() == true)
                {
                    string filePath = dialog.FileName;
                    if (format == "json")
                    {
                        var users = await _bookingService.ImportFromJsonAsync(filePath);
                        Load();
                    }
                    else if (format == "excel")
                    {
                        var users = await _bookingService.ImportFromExcelAsync(filePath);
                        Load();
                    }
                    ShowSuccessMessage("Users imported successfully.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error importing booking: {ex.Message}");
            }
        }

        private async void ExportBooking(string format)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.Filter = format == "json" ? "JSON Files (*.json)|*.json" : "Excel Files (*.xlsx)|*.xlsx";
                if (dialog.ShowDialog() == true)
                {
                    string filePath = dialog.FileName;
                    if (format == "json")
                    {
                        await _bookingService.ExportToJsonAsync(Bookings.ToList(), filePath);
                    }
                    else if (format == "excel")
                    {
                        await _bookingService.ExportToExcelAsync(Bookings.ToList(), filePath);
                    }
                    ShowSuccessMessage("Users exported successfully.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error exporting users: {ex.Message}");
            }
        }


        private void SearchBookings()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Load();
                return;
            }


            var filteredBookings = _booking.Where(booking =>
                booking.User.Username.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                booking.ContactPhone.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                booking.ContactEmail.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            Bookings = new ObservableCollection<Booking>(filteredBookings);
        }


        private void ShowDetailBooking()
        {
            IsAddUserDialogOpen = true;
        }

        private void OpenAddUserDialog()
        {
            IsAddUserDialogOpen = true;
            SelectedBookingManager = new Booking();
        }

        private bool CanExecuteAddUser()
        {
            return !string.IsNullOrWhiteSpace(SelectedBookingManager?.ContactName);
        }

        private async void Load()
        {
            IsHideId = false;

            try
            {
                var books = await _bookingService.GetAllBookingByOwnersAsync(_currentUser.UserId);
                Bookings = new ObservableCollection<Booking>(books);
                SelectedBookingManager = new Booking();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading bookings: {ex.Message}");
            }
        }


        // Edit the selected user
        private async void EditBooking()
        {
            if (SelectedBookingManager == null || string.IsNullOrEmpty(SelectedBookingManager.BookingId + ""))
            {
                ShowWarningMessage("Please select a book to edit.");
                return;
            }

            try
            {
                await _bookingService.UpdateBookingPartialAsync(SelectedBookingManager);
                Load();
                ShowSuccessMessage("Booking updated successfully.");
                IsUpdateUserDialogOpen = true;
                IsAddUserDialogOpen = false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error editing Booking: {ex.Message}");
            }
        }

        private void ShowSuccessMessage(string message)
        {
            Message = message;
            IsShowMessageDialog = true;
        }

        private void ShowWarningMessage(string message)
        {
            Message = message;
            IsShowMessageDialog = true;
        }

        private void ShowErrorMessage(string message)
        {
            Message = message;
            IsShowMessageDialog = true;
        }

        private void MouseLeftButtonDownDragFunc()
        {

        }

        private void ShowToastMessage(string message)
        {
            Message = message;
            IsShowMessageDialog = true;

            var toastContainer = Application.Current.MainWindow.FindName("AlertDialogHost") as FrameworkElement;
            if (toastContainer != null)
            {
                var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1))
                {
                    BeginTime = TimeSpan.FromSeconds(3)
                };

                toastContainer.Visibility = Visibility.Visible;

                toastContainer.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                fadeOutAnimation.Completed += (s, a) =>
                {
                    toastContainer.Visibility = Visibility.Collapsed;
                };
                toastContainer.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            }
        }

        #endregion
    }
}