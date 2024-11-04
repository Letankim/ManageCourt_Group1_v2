using BusinessLogic.Interface; // Ensure you have this interface for user operations
using Model; // This should be your user model from the library
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WPF_ManageCourt.ViewModel;

public class UserViewModel : BaseViewModel
{
    private readonly IUserService _userService; // Service for user operations
    private ObservableCollection<User> _users; // Collection of users
    private ObservableCollection<string> _userRoles;
    private ObservableCollection<string> _statusOptions;// Collection of user roles
    private User _selectedUser; // Currently selected user
    private bool _isHideId; // Flag for hiding user ID
    private bool _isAddUserDialogOpen; // Flag for controlling dialog visibility
    private bool _isUpdateUserDialogOpen; // Flag for controlling update dialog visibility
    private string message;
    private bool _isShowMessageDialog;

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

    public ObservableCollection<string> StatusOptions { get; set; } = new ObservableCollection<string> { "Active", "Inactive" };

    public UserViewModel(IUserService userService)
    {
        _userService = userService; // Injected service
        SelectedUser = new User(); // Initialize selected user
        InitializeCommands(); // Initialize commands
        Load(); // Load users on startup
    }

    #region Properties
    public void SetPassword(string password)
    {
        SelectedUser.Password = password;
    }
    public ObservableCollection<User> Users
    {
        get => _users;
        set
        {
            _users = value;
            OnPropertyChanged(nameof(Users));
            IsUsersEmpty = _users == null || _users.Count == 0;
        }
    }

    public ObservableCollection<string> UserRoles
    {
        get => _userRoles;
        set
        {
            _userRoles = value;
            OnPropertyChanged(nameof(UserRoles));
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

    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            OnPropertyChanged(nameof(SelectedUser));
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
        AddUserCommand = new RelayCommand<object>(_ => CanExecuteAddUser(), _ => AddUser());
        EditCommand = new RelayCommand<object>(_ => SelectedUser != null, _ => EditUser());
        DeleteCommand = new RelayCommand<object>(_ => SelectedUser != null, _ => DeleteUser());
        RowSelectedCommand = new RelayCommand<object>(_ => true, _ => ShowDetailUser());
        SearchCommand = new RelayCommand<object>(_ => true, _ => SearchUsers());
        ImportCommand = new RelayCommand<string>(_ => true,format => ImportUsers(format));
        ExportCommand = new RelayCommand<string>(_ => true,format => ExportUsers(format));
        ShowExportCommand = new RelayCommand<string>(_ => true, format => ShowExportOptions());
    }

    private void ShowExportOptions()
    {
        IsExportMenuOpen = true;
    }

    private async void ImportUsers(string format)
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
                    var users = await _userService.ImportFromJsonAsync(filePath);
                    Load();
                }
                else if (format == "excel")
                {
                    var users = await _userService.ImportFromExcelAsync(filePath);
                    Load();
                }
                ShowSuccessMessage("Users imported successfully.");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error importing users: {ex.Message}");
        }
    }

    private async void ExportUsers(string format)
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
                    await _userService.ExportToJsonAsync(Users.ToList(), filePath);
                }
                else if (format == "excel")
                {
                    await _userService.ExportToExcelAsync(Users.ToList(), filePath);
                }
                ShowSuccessMessage("Users exported successfully.");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error exporting users: {ex.Message}");
        }
    }


    private void SearchUsers()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            Load();
            return;
        }

        var filteredUsers = _users.Where(user =>
            user.Username.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            user.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            user.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        Users = new ObservableCollection<User>(filteredUsers);
    }


    private void ShowDetailUser()
    {
        IsAddUserDialogOpen = true;
    }

    private void OpenAddUserDialog()
    {
        IsAddUserDialogOpen = true; 
        SelectedUser = new User(); 
    }

    private bool CanExecuteAddUser()
    {
        return !string.IsNullOrWhiteSpace(SelectedUser?.Username) && !string.IsNullOrWhiteSpace(SelectedUser?.Password);
    }

    private async void Load()
    {
        IsHideId = false;

        try
        {
            var users = await _userService.GetListAllUsersAsync();
            Users = new ObservableCollection<User>(users);
            UserRoles = new ObservableCollection<string> { "Admin", "User", "CourtOwner" };
            SelectedUser = new User();
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error loading users: {ex.Message}");
        }
    }

    private async void AddUser()
    {
        if (!CanExecuteAddUser())
        {
            ShowWarningMessage("Please fill all required fields.");
            return;
        }

        try
        {
            User newUser = new User
            {
                Username = SelectedUser.Username,
                Password = SelectedUser.Password,
                Role = SelectedUser.Role,
                FullName = SelectedUser.FullName,
                Email = SelectedUser.Email,
                Phone = SelectedUser.Phone,
                IsEnabled = SelectedUser.IsEnabled
            };
            await _userService.AddUserAsync(newUser);
            Load();
            IsAddUserDialogOpen = false;
            ShowSuccessMessage("User added successfully.");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error adding user: {ex.Message}");
        }
    }

    // Edit the selected user
    private async void EditUser()
    {
        if (SelectedUser == null || string.IsNullOrEmpty(SelectedUser.UserId + ""))
        {
            ShowWarningMessage("Please select a user to edit.");
            return;
        }

        try
        {
            await _userService.UpdateUserAsync(SelectedUser);
            Load();
            ShowSuccessMessage("User updated successfully.");
            IsUpdateUserDialogOpen = false;
            IsAddUserDialogOpen = false;
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error editing user: {ex.Message}");
        }
    }

    private async void DeleteUser()
    {
        if (SelectedUser == null)
        {
            ShowWarningMessage("Please select a user to delete.");
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                if (SelectedUser.UserId == 0) {
                    return;
                }
                var isExist =  await _userService.GetUserByIdAsync(SelectedUser.UserId);
                if (isExist != null)
                {
                    await _userService.DeleteUserAsync(SelectedUser.UserId);
                    Load();
                    ShowSuccessMessage("User deleted successfully.");
                    IsUpdateUserDialogOpen = false;
                    IsAddUserDialogOpen = false;
                }             }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error deleting user: {ex.Message}" + SelectedUser.UserId);
            }
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
