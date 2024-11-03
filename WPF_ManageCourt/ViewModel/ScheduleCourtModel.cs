using BusinessLogic.Interface;
using BusinessLogic.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPF_ManageCourt.ViewModel
{
    public class ScheduleCourtModel : BaseViewModel
    {
        private readonly ICourtScheduleService _scheduleService;
        private readonly IBadmintonCourtService _courtService;
        private ObservableCollection<CourtSchedule> _schedules;
        private ObservableCollection<CourtSchedule> _schedulesAllCourtName;
        private CourtSchedule _selectedSchedule;
        private bool _isHideId;
        private bool _isAddScheduleDialogOpen;
        private bool _isUpdateScheduleDialogOpen;
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


    public ScheduleCourtModel(IBadmintonCourtService courtService, ICourtScheduleService scheduleService)
        {
            _courtService = courtService;
            _scheduleService = scheduleService;
            Load();
            InitializeCommands();
        }

        #region Properties

        public ObservableCollection<CourtSchedule> Schedules
        {
            get => _schedules;
            set
            {
                _schedules = value;
                OnPropertyChanged(nameof(Schedules));
                IsSchedulesEmpty = _schedules == null || _schedules.Count == 0;
            }
        }
        
        public ObservableCollection<CourtSchedule> SchedulesAllCourtName
        {
            get => _schedulesAllCourtName;
            set
            {
                _schedulesAllCourtName = value;
                OnPropertyChanged(nameof(SchedulesAllCourtName));
            }
        }

        

        public CourtSchedule SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                _selectedSchedule = value;
                OnPropertyChanged(nameof(SelectedSchedule));
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

        public bool IsAddScheduleDialogOpen
        {
            get => _isAddScheduleDialogOpen;
            set
            {
                _isAddScheduleDialogOpen = value;
                OnPropertyChanged(nameof(IsAddScheduleDialogOpen));
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

        public bool IsUpdateScheduleDialogOpen
        {
            get => _isUpdateScheduleDialogOpen;
            set
            {
                _isUpdateScheduleDialogOpen = value;
                OnPropertyChanged(nameof(IsUpdateScheduleDialogOpen));
            }
        }

        private bool _isSchedulesEmpty;

        public bool IsSchedulesEmpty
        {
            get => _isSchedulesEmpty;
            set
            {
                _isSchedulesEmpty = value;
                OnPropertyChanged(nameof(IsSchedulesEmpty));
            }
        }

        #endregion

        #region Commands

        public ICommand LoadedWindowCommand { get; private set; }
        public ICommand OpenAddScheduleDialogCommand { get; private set; }
        public ICommand AddScheduleCommand { get; private set; }
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
            OpenAddScheduleDialogCommand = new RelayCommand<object>(_ => true, _ => OpenAddScheduleDialog());
            AddScheduleCommand = new RelayCommand<object>(_ => CanExecuteAddSchedule(), _ => AddSchedule());
            EditCommand = new RelayCommand<object>(_ => SelectedSchedule != null, _ => EditSchedule());
            DeleteCommand = new RelayCommand<object>(_ => SelectedSchedule != null, _ => DeleteSchedule());
            RowSelectedCommand = new RelayCommand<object>(_ => true, _ => ShowDetailSchedule());
            SearchCommand = new RelayCommand<object>(_ => true, _ => SearchSchedules());
        }

        private void SearchSchedules()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Load();
                return;
            }

            var filteredSchedules = _schedules.Where(schedule =>
                schedule.Date.ToString().Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                schedule.Court.CourtName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            Schedules = new ObservableCollection<CourtSchedule>(filteredSchedules);
        }

        private void ShowDetailSchedule()
        {
            IsAddScheduleDialogOpen = true;
        }

        private void OpenAddScheduleDialog()
        {
            IsAddScheduleDialogOpen = true;
            SelectedSchedule = new CourtSchedule();
        }

        private bool CanExecuteAddSchedule()
        {
            return !string.IsNullOrWhiteSpace(SelectedSchedule?.CourtId.ToString()) && !string.IsNullOrWhiteSpace(SelectedSchedule?.TimeSlot) && SelectedSchedule?.Date != DateOnly.MinValue && SelectedSchedule?.IsAvailable != null;
        }

        private async void Load()
        {
            IsHideId = false;
            try
            {
                var courtSchedules = await _scheduleService.GetListAllSchedulesAsync();
                var schedulesAllGroupName = await _scheduleService.GetAllSchedulesAllCourtNameAsync();
                Schedules = new ObservableCollection<CourtSchedule>(courtSchedules);
                SchedulesAllCourtName = new ObservableCollection<CourtSchedule>(schedulesAllGroupName);
                SelectedSchedule = new CourtSchedule();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading Schedules: {ex.Message}");
            }
        }

        private async void AddSchedule()
        {
            if (!CanExecuteAddSchedule())
            {
                ShowWarningMessage("Please fill all required fields.");
                return;
            }

            try
            {
                CourtSchedule newSchedule = new CourtSchedule
                {
                    CourtId = SelectedSchedule.CourtId,
                    TimeSlot = SelectedSchedule.TimeSlot,
                    Date = SelectedSchedule.Date,
                    IsAvailable = SelectedSchedule.IsAvailable
                };
                await _scheduleService.AddScheduleAsync(newSchedule);
                Load();
                IsAddScheduleDialogOpen = false;
                ShowSuccessMessage("Schedule added successfully.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error adding schedule: {ex.Message}");
            }
        }

        // Edit the selected schedule
        private async void EditSchedule()
        {
            if (SelectedSchedule == null || string.IsNullOrEmpty(SelectedSchedule.ScheduleId + ""))
            {
                ShowWarningMessage("Please select a schedule to edit.");
                return;
            }

            try
            {
                await _scheduleService.UpdateScheduleAsync(SelectedSchedule);
                Load();
                ShowSuccessMessage("Schedule updated successfully.");
                IsUpdateScheduleDialogOpen = false;
                IsAddScheduleDialogOpen = false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error editing schedule: {ex.Message}");
            }
        }

        private async void DeleteSchedule()
        {
            if (SelectedSchedule == null)
            {
                ShowWarningMessage("Please select a schedule to delete.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this schedule?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (SelectedSchedule.ScheduleId == 0)
                    {
                        return;
                    }
                    var isExist = await _scheduleService.GetScheduleByIdAsync(SelectedSchedule.ScheduleId);
                    if (isExist != null)
                    {
                        await _scheduleService.DeleteScheduleAsync(SelectedSchedule.ScheduleId);
                        Load();
                        ShowSuccessMessage("Schedule deleted successfully.");
                        IsUpdateScheduleDialogOpen = false;
                        IsAddScheduleDialogOpen = false;
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error deleting user: {ex.Message}" + SelectedSchedule.ScheduleId);
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
}
