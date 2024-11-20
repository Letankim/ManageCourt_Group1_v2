using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;
using Model;

namespace WPF_ManageCourt.ViewModel
{
    public class AccessoryViewModel : BaseViewModel
    {
        private readonly IAccessoryService _accessoryService;
        private ObservableCollection<Accessory> _accessories;

        private Accessory _selectedAccessory;
        private bool _isHideId;
        private bool _isAddAccessoryDialogOpen;
        private bool _isUpdateAccessoryDialogOpen;
        private string message;
        private bool _isShowMessageDialog;
        private int _quantity;

        public AccessoryViewModel(IAccessoryService accessoryService)
        {
            _accessoryService = accessoryService;
            SelectedAccessory = new Accessory();
            InitializeCommands(); // Initialize commands
            Load();
        }
        #region Properties
        public ObservableCollection<Accessory> Accessories
        {
            get => _accessories;
            set
            {
                _accessories = value;
                OnPropertyChanged(nameof(Accessories));
                IsAccessoryEmpty = _accessories == null || _accessories.Count == 0;
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
        public Accessory SelectedAccessory
        {
            get => _selectedAccessory;
            set
            {
                _selectedAccessory = value;
                OnPropertyChanged(nameof(SelectedAccessory));
            }
        }
        public bool IsAddAccessoryDialogOpen
        {
            get => _isAddAccessoryDialogOpen;
            set
            {
                _isAddAccessoryDialogOpen = value;
                OnPropertyChanged(nameof(IsAddAccessoryDialogOpen));
            }
        }
        public bool IsUpdateAccessoryDialogOpen
        {
            get => _isUpdateAccessoryDialogOpen;
            set
            {
                _isUpdateAccessoryDialogOpen = value;
                OnPropertyChanged(nameof(IsUpdateAccessoryDialogOpen));
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
        private bool _isAccessoryEmpty;

        public bool IsAccessoryEmpty
        {
            get => _isAccessoryEmpty;
            set
            {
                _isAccessoryEmpty = value;
                OnPropertyChanged(nameof(IsAccessoryEmpty));
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
        public ICommand OpenAddAccessoryDialogCommand { get; private set; }
        public ICommand AddAccessoryCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand RowSelectedCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        #endregion

        #region Methods
        // Initialize all commands
        private void InitializeCommands()
        {
            LoadedWindowCommand = new RelayCommand<object>(_ => true, _ => Load());
            OpenAddAccessoryDialogCommand = new RelayCommand<object>(_ => true, _ => OpenAddAccessoryDialog());
            AddAccessoryCommand = new RelayCommand<object>(_ => CanExecuteAddAccessory(), _ => AddAccessoty());
            EditCommand = new RelayCommand<object>(_ => SelectedAccessory != null, _ => EditAccessory());
            DeleteCommand = new RelayCommand<object>(_ => SelectedAccessory != null, _ => DeleteAccessory());
            RowSelectedCommand = new RelayCommand<object>(_ => true, _ => ShowDetailAccessory());
            SearchCommand = new RelayCommand<object>(_ => true, _ => SearchAccessory());

        }



        private async void Load()
        {
            IsHideId = false;
            try
            {
                var accessories = await _accessoryService.GetListAllAccessoriesAsync();
                Accessories = new ObservableCollection<Accessory>(accessories);



                SelectedAccessory = new Accessory();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading users: {ex.Message}");
            }
        }

        private async void AddAccessoty()
        {
            if (!CanExecuteAddAccessory())
            {
                ShowWarningMessage("Please fill valid data.");
                return;
            }
            try
            {
                Accessory newAccessory = new Accessory
                {
                    Name = SelectedAccessory.Name,
                    Price = SelectedAccessory.Price,
                };
                await _accessoryService.AddAccessoryAsync(newAccessory);
                Load();
                IsAddAccessoryDialogOpen = false;
                ShowSuccessMessage("Add accessory successful");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error adding accessory: {ex.Message}");
            }
        }

        private async void EditAccessory()
        {
            if (SelectedAccessory == null || string.IsNullOrEmpty(SelectedAccessory.AccessoryId + ""))
            {
                ShowWarningMessage("Please select a accessory to edit.");
                return;
            }
            try
            {
                await _accessoryService.UpdateAccessoryAsync(SelectedAccessory);
                Load();
                ShowSuccessMessage("Update accessory successful");
                IsUpdateAccessoryDialogOpen = true;
                IsAddAccessoryDialogOpen = false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error editing accessory: {ex.Message}");
            }
        }

        private async void DeleteAccessory()
        {
            if (SelectedAccessory == null)
            {
                ShowWarningMessage("Please select a accessory to delete.");
            }
            var result = MessageBox.Show("Are you sure you want to delete this accessory?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _accessoryService.DeleteAccessoryAsync(SelectedAccessory.AccessoryId);
                    Load();
                    ShowSuccessMessage("Remove accessory successful");
                    IsUpdateAccessoryDialogOpen = true;
                    IsAddAccessoryDialogOpen = false;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error deleting accessory: {ex.Message}");
                }
            }
        }
        private void OpenAddAccessoryDialog()
        {
            IsAddAccessoryDialogOpen = true;
            SelectedAccessory = new Accessory();
        }

        private void ShowErrorMessage(string message)
        {
            Message = message;
            IsShowMessageDialog = true;
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

        private void MouseLeftButtonDownDragFunc()
        {

        }
        private bool CanExecuteAddAccessory()
        {
         
            if (string.IsNullOrWhiteSpace(SelectedAccessory?.Name))
            {
                return false; 
            }

            if (SelectedAccessory.Price != 0) 
            {
                if (!decimal.TryParse(SelectedAccessory.Price.ToString(), out var price) || price <= 0)
                {
                    ShowWarningMessage("Price must be a valid positive number."); 
                    return false; 
                }
            }

            return true; 
        }
        private void ShowDetailAccessory()
        {
            IsAddAccessoryDialogOpen = true;
        }

        private void SearchAccessory()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Load();
                return;
            }
            var filterAccessory = _accessories.Where(accessory =>
            accessory.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            Accessories = new ObservableCollection<Accessory>(filterAccessory);
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
