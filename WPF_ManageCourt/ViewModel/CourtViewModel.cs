using BusinessLogic.Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WPF_ManageCourt.ViewModel;

public class CourtViewModel : BaseViewModel
{
    private readonly IBadmintonCourtService _courtService; // Service cho BadmintonCourt
    private ObservableCollection<BadmintonCourt> _courts; // Collection cho danh sách sân
    private BadmintonCourt _selectedCourt; // Sân được chọn
    private bool _isAddCourtDialogOpen; // Kiểm soát hiển thị dialog thêm mới
    private bool _isUpdateCourtDialogOpen; // Kiểm soát hiển thị dialog cập nhật
    private string _message;
    private bool _isShowMessageDialog;
    private bool _isCourtsEmpty;

    public CourtViewModel(IBadmintonCourtService courtService)
    {
        _courtService = courtService; // Injected service
        SelectedCourt = new BadmintonCourt();
        InitializeCommands(); // Khởi tạo các command
        LoadCourts(); // Tải danh sách sân khi khởi động
    }

    #region Properties

    public ObservableCollection<BadmintonCourt> Courts
    {
        get => _courts;
        set
        {
            _courts = value;
            OnPropertyChanged(nameof(Courts));
            IsCourtsEmpty = _courts == null || _courts.Count == 0;
        }
    }

    public BadmintonCourt SelectedCourt
    {
        get => _selectedCourt;
        set
        {
            _selectedCourt = value;
            OnPropertyChanged(nameof(SelectedCourt));
        }
    }

    public bool IsAddCourtDialogOpen
    {
        get => _isAddCourtDialogOpen;
        set
        {
            _isAddCourtDialogOpen = value;
            OnPropertyChanged(nameof(IsAddCourtDialogOpen));
        }
    }

    public bool IsUpdateCourtDialogOpen
    {
        get => _isUpdateCourtDialogOpen;
        set
        {
            _isUpdateCourtDialogOpen = value;
            OnPropertyChanged(nameof(IsUpdateCourtDialogOpen));
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged(nameof(Message));
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

    public bool IsCourtsEmpty
    {
        get => _isCourtsEmpty;
        set
        {
            _isCourtsEmpty = value;
            OnPropertyChanged(nameof(IsCourtsEmpty));
        }
    }

    #endregion

    #region Commands

    public ICommand LoadedWindowCommand { get; private set; }
    public ICommand OpenAddCourtDialogCommand { get; private set; }
    public ICommand AddCourtCommand { get; private set; }
    public ICommand EditCourtCommand { get; private set; }
    public ICommand DeleteCourtCommand { get; private set; }

    #endregion

    #region Methods

    // Khởi tạo các command
    private void InitializeCommands()
    {
        LoadedWindowCommand = new RelayCommand<object>(_ => true, _ => LoadCourts());
        OpenAddCourtDialogCommand = new RelayCommand<object>(_ => true, _ => OpenAddCourtDialog());
        AddCourtCommand = new RelayCommand<object>(_ => CanExecuteAddCourt(), _ => AddCourt());
        EditCourtCommand = new RelayCommand<object>(_ => SelectedCourt != null, _ => EditCourt());
        DeleteCourtCommand = new RelayCommand<object>(_ => SelectedCourt != null, _ => DeleteCourt());
    }

    private async void LoadCourts()
    {
        try
        {
            var courts = await _courtService.GetListAllCourtsAsync();
            Courts = new ObservableCollection<BadmintonCourt>(courts);
            SelectedCourt = new BadmintonCourt(); // Đặt lại sân đã chọn
        }
        catch (Exception ex)
        {
            ShowMessage($"Error loading courts: {ex.Message}", isError: true);
        }
    }
    public ObservableCollection<string> StatusOptions { get; set; } = new ObservableCollection<string> { "Active", "Inactive" };
    private void OpenAddCourtDialog()
    {
        SelectedCourt = new BadmintonCourt(); // Khởi tạo sân mới
        IsAddCourtDialogOpen = true;
    }

    private bool CanExecuteAddCourt()
    {
        return !string.IsNullOrWhiteSpace(SelectedCourt?.CourtName) && SelectedCourt.PricePerHour > 0;
    }

    private async void AddCourt()
    {
        try
        {
            await _courtService.AddCourtAsync(SelectedCourt);
            LoadCourts();
            IsAddCourtDialogOpen = false;
            ShowMessage("Court added successfully.", isError: false);
        }
        catch (Exception ex)
        {
            ShowMessage($"Error adding court: {ex.Message}", isError: true);
        }
    }

    private async void EditCourt()
    {
        if (SelectedCourt == null || SelectedCourt.CourtId == 0)
        {
            ShowMessage("Please select a court to edit.", isError: true);
            return;
        }

        try
        {
            await _courtService.UpdateCourtAsync(SelectedCourt);
            LoadCourts();
            ShowMessage("Court updated successfully.", isError: false);
            IsUpdateCourtDialogOpen = false;
            IsAddCourtDialogOpen = false;
        }
        catch (Exception ex)
        {
            ShowMessage($"Error editing court: {ex.Message}", isError: true);
        }
    }

    private async void DeleteCourt()
    {
        if (SelectedCourt == null)
        {
            ShowMessage("Please select a court to delete.", isError: true);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this court?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _courtService.DeleteCourtAsync(SelectedCourt.CourtId);
                LoadCourts();
                ShowMessage("Court deleted successfully.", isError: false);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting court: {ex.Message}", isError: true);
            }
        }
    }

    private void ShowMessage(string message, bool isError)
    {
        Message = message;
        IsShowMessageDialog = true;
        // Có thể thêm logic để phân biệt hiển thị thông báo lỗi hoặc thành công
    }

    #endregion
}