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
using BusinessLogic.Service;
using System.IO;
using Microsoft.Win32;
using WPF_ManageCourt;

public class CourtViewModel : BaseViewModel
{
    private readonly string  _webRootPath = "E:\\FPT\\FA24\\PRN221\\Assigment_v2\\ManageCourt_Group1\\WEB_ManageCourt\\wwwroot\\images\\";
    private readonly IBadmintonCourtService _courtService;
    private readonly ICourtImageService _courtImageService; // Service cho BadmintonCourt
    private ObservableCollection<BadmintonCourt> _courts; // Collection cho danh sách sân
    private BadmintonCourt _selectedCourt; // Sân được chọn
    private bool _isAddCourtDialogOpen; // Kiểm soát hiển thị dialog thêm mới
    private bool _isUpdateCourtDialogOpen; // Kiểm soát hiển thị dialog cập nhật
    private string _message;
    private bool _isShowMessageDialog;
    private bool _isCourtsEmpty;
    private List<CourtImage> _selectedCourtImages;
    private User _user;
    public List<CourtImage> SelectedCourtImages
    {
        get => _selectedCourtImages;
        set
        {
            _selectedCourtImages = value;
            OnPropertyChanged(nameof(SelectedCourtImages));
        }
    }

    public CourtViewModel(IBadmintonCourtService courtService, ICourtImageService  courtImageService)
    {
        _user = (User)Application.Current.Properties["LoggedInUser"];
        if (_user == null) { 
            LoginWindow login = new LoginWindow();
            login.Show();
            Application.Current.Windows[0].Close();
            return;
        }
        _courtImageService  = courtImageService;
        _courtService = courtService; // Injected service
        SelectedCourt = new BadmintonCourt();
        InitializeCommands(); // Khởi tạo các command
        LoadCourts(); // Tải danh sách sân khi khởi động
    }

    private async Task UpdateSelectedCourtImagesAsync()
    {
        var courtImages = await _courtImageService.GetImagesByCourtIdAsync(SelectedCourt.CourtId) ?? new List<CourtImage>();
        foreach (var image in courtImages)
        {

            if (!image.ImageUrl.StartsWith("http://"))
            {
                image.ImageUrl = $"http://localhost:5116/{image.ImageUrl}";
            }
        }
        SelectedCourtImages = courtImages;
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

    public  BadmintonCourt SelectedCourt
    {
        get => _selectedCourt;
        set
        {
            _selectedCourt = value;
            OnPropertyChanged(nameof(SelectedCourt));
            UpdateSelectedCourtImagesAsync();
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

    public ICommand ChooseImageCommand { get; private set; }
    public ICommand UploadImageCommand { get; private set; }

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
        ChooseImageCommand = new RelayCommand<object>(_ => true, _ => ChooseImage());
        UploadImageCommand = new RelayCommand<object>(_ => true, _ => UploadImageAsync());
    }

    public List<string> SelectedImagePaths { get; set; } = new List<string>();

    private void ChooseImage()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
            Multiselect = true 
        };

        if (openFileDialog.ShowDialog() == true)
        {
            SelectedImagePaths = openFileDialog.FileNames.ToList();
            OnPropertyChanged(nameof(SelectedImagePaths));
        }
    }

    private async Task UploadImageAsync()
    {
        if (SelectedCourt == null || SelectedImagePaths == null || SelectedImagePaths.Count == 0)
        {
            MessageBox.Show("Please select a court and images before uploading.");
            return;
        }

        Directory.CreateDirectory(_webRootPath);

        await _courtImageService.DeleteImagesByCourtIdAsync(SelectedCourt.CourtId);

        foreach (string imagePath in SelectedImagePaths)
        {
            string fileName = $"{SelectedCourt.CourtId}_{Path.GetFileName(imagePath)}";
            string destinationPath = Path.Combine(_webRootPath, fileName);

            File.Copy(imagePath, destinationPath, true);

            CourtImage courtImage = new CourtImage()
            {
                ImageUrl = $"/images/{fileName}",
                CourtId = SelectedCourt.CourtId
            };
            await _courtImageService.AddImageAsync(courtImage);
        }
        UpdateSelectedCourtImagesAsync();
        MessageBox.Show("Images uploaded successfully.");
    }


    private async void LoadCourts()
    {
        try
        {
            var courts = await _courtService.GetCourtsByOwnerIdAsync(_user.UserId);
            Courts = new ObservableCollection<BadmintonCourt>(courts);
            SelectedCourt = new BadmintonCourt(); 
        }
        catch (Exception ex)
        {
            ShowMessage($"Error loading courts: {ex.Message}", isError: true);
        }
    }
    public ObservableCollection<string> StatusOptions { get; set; } = new ObservableCollection<string> { "Active", "Inactive" };
    private void OpenAddCourtDialog()
    {
        SelectedCourt = new BadmintonCourt(); 
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
            SelectedCourt.OwnerId = _user.UserId;
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
    }

    #endregion
}