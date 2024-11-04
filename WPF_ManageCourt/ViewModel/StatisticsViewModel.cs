using BusinessLogic.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using LiveCharts.Wpf;
using LiveCharts;
using Model;
using System.Windows;

namespace WPF_ManageCourt.ViewModel
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly IBookingService _bookingService;
        private readonly User _user;
        private readonly ICourtScheduleService _courtScheduleService;
        private readonly IBookingAccessoryService _bookingAccessoryService;

        // Properties for revenue statistics
        public ObservableCollection<string> RevenueLabels { get; set; }
        public ChartValues<decimal> RevenueValues { get; set; }
        public Func<double, string> CurrencyFormatter { get; set; }

        // Properties for court status statistics
        public SeriesCollection CourtStatusValues { get; set; }
        public SeriesCollection PaymentStatusValues { get; set; }
        public ChartValues<int> EmptySlotValues { get; set; }
        public ObservableCollection<string> TimeLabels { get; set; }
        public ChartValues<int> BookedSlotValues { get; set; }
        public ObservableCollection<string> AccessoryLabels { get; set; }
        public ChartValues<int> AccessoryValues { get; set; }
        public Func<double, string> QuantityFormatter { get; set; }
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateChartCommand { get; }

        public StatisticsViewModel(IBookingService bookingService, ICourtScheduleService courtScheduleService, IBookingAccessoryService bookingAccessoryService)
        {
            _user = (User)Application.Current.Properties["LoggedInUser"];
            _bookingService = bookingService;
            _courtScheduleService = courtScheduleService ?? throw new ArgumentNullException(nameof(courtScheduleService));
            BookedSlotValues = new ChartValues<int>();
            _bookingAccessoryService = bookingAccessoryService;

            EmptySlotValues = new ChartValues<int>();
            TimeLabels = new ObservableCollection<string>();
            // Initialize collections
            RevenueLabels = new ObservableCollection<string>();
            RevenueValues = new ChartValues<decimal>();
            CourtStatusValues = new SeriesCollection();
            PaymentStatusValues = new SeriesCollection();
            CurrencyFormatter = value => value.ToString("C0", CultureInfo.CurrentCulture);
            AccessoryLabels = new ObservableCollection<string>();
            AccessoryValues = new ChartValues<int>();
            QuantityFormatter = value => value.ToString("N0");
            // Initialize command
            UpdateChartCommand = new RelayCommand<object>(_ => true, _ => LoadDataAsync());
        }
        private async Task LoadAccessoryDataAsync(DateOnly startDate, DateOnly endDate)
        {
            // Lấy danh sách thống kê doanh số phụ kiện từ dịch vụ
            var accessorySalesDataList = await _bookingAccessoryService.GetAccessorySalesReport(startDate, endDate);

            // Xóa dữ liệu cũ trước khi thêm dữ liệu mới
            AccessoryLabels.Clear();
            AccessoryValues.Clear();

            foreach (var accessoryData in accessorySalesDataList)
            {
                // Thêm tên phụ kiện và số lượng bán ra vào biểu đồ
                AccessoryLabels.Add($"{accessoryData.AccessoryName} (With Bookings)");
                AccessoryValues.Add(accessoryData.TotalQuantitySoldWithBookings);

                AccessoryLabels.Add($"{accessoryData.AccessoryName} (Without Bookings)");
                AccessoryValues.Add(accessoryData.TotalQuantitySoldWithoutBookings);
            }
        }
        private async Task LoadDataAsync(int? courtId = null)
        {
            if (StartDate == null || EndDate == null || StartDate > EndDate)
                return;

            // Clear previous data
            RevenueLabels.Clear();
            RevenueValues.Clear();
            CourtStatusValues.Clear();
            PaymentStatusValues.Clear();
            EmptySlotValues.Clear();
            TimeLabels.Clear();
            BookedSlotValues.Clear();
            AccessoryLabels.Clear();
            AccessoryValues.Clear();
            var startDay = DateOnly.FromDateTime(StartDate.Value);
            var endDay = DateOnly.FromDateTime(EndDate.Value);

            // Load revenue data
            var revenueData = await _bookingService.StatisticsAsync(startDay, endDay, _user.UserId);
            var filteredRevenueData = revenueData.Where(entry => entry.Key >= startDay && entry.Key <= endDay);
            TimeLabels.Add("Slot 1");
            foreach (var entry in filteredRevenueData)
            {
                RevenueLabels.Add(entry.Key.ToString("dd/MM/yyyy"));
                RevenueValues.Add(entry.Value);
            }
            await LoadAccessoryDataAsync(startDay, endDay);

            for (var date = startDay; date <= endDay; date = date.AddDays(1))
            {
                // Lấy thống kê số slot trống và đã đặt cho từng ngày
                var availabilityStats = await _courtScheduleService.GetAvailabilityStatisticsAsync(date, date);

                // Thêm thống kê hàng ngày vào biểu đồ
                EmptySlotValues.Add(availabilityStats.availableCount);  // Số slot trống
                BookedSlotValues.Add(availabilityStats.bookedCount);     // Số slot đã đặt
                TimeLabels.Add(date.ToString("dd/MM/yyyy"));            // Nhãn là ngày

                // Nếu cần, bạn cũng có thể thêm bookedCount vào một series riêng
            }
            // Load court status data
            var statusData = await _bookingService.StatisticStatus(startDay, endDay, _user.UserId);

            // Aggregate court status data
            int totalConfirmed = statusData.Sum(entry => entry.Value.Confirmed);
            int totalNoShow = statusData.Sum(entry => entry.Value.NoShow);
            int totalCancelled = statusData.Sum(entry => entry.Value.Cancelled);

            // Update CourtStatusValues for the PieChart
            CourtStatusValues.Add(new PieSeries
            {
                Title = "Confirmed",
                Values = new ChartValues<int> { totalConfirmed },
                Fill = new SolidColorBrush(Colors.Green)
            });
            CourtStatusValues.Add(new PieSeries
            {
                Title = "NoShow",
                Values = new ChartValues<int> { totalNoShow },
                Fill = new SolidColorBrush(Colors.Yellow)
            });
            CourtStatusValues.Add(new PieSeries
            {
                Title = "Cancelled",
                Values = new ChartValues<int> { totalCancelled },
                Fill = new SolidColorBrush(Colors.Red)
            }); Console.WriteLine($"Confirmed: {totalConfirmed}, NoShow: {totalNoShow}, Cancelled: {totalCancelled}");

            //Payment
            var paymentData = await _bookingService.StatisticPayment(startDay, endDay, _user.UserId);

            // Aggregate payment status data
            int totalAfterPlay = paymentData.Sum(entry => entry.Value.AfterPlay);
            int totalOnline = paymentData.Sum(entry => entry.Value.Online);

            // Update PaymentStatusValues for the PieChart
            PaymentStatusValues.Add(new PieSeries
            {
                Title = "AfterPlay",
                Values = new ChartValues<int> { totalAfterPlay },
                Fill = new SolidColorBrush(Colors.Green)
            });
            PaymentStatusValues.Add(new PieSeries // Fixed: Change this to PaymentStatusValues
            {
                Title = "Online",
                Values = new ChartValues<int> { totalOnline },
                Fill = new SolidColorBrush(Colors.Yellow)
            });

        }

    }
}


