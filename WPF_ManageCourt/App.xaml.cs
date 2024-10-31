using BusinessLogic.Interface;
using BusinessLogic.Service;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Interface;
using Repositories;
using System;
using System.Windows;
using WPF_ManageCourt.ViewModel;
using DataAccess.DAO;
using WPF_ManageCourt.Services;

namespace WPF_ManageCourt
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Thiết lập Dependency Injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register services
            services.AddTransient<IBadmintonCourtService, BadmintonCourtService>();
            services.AddTransient<ICourtImageService, CourtImageService>();
            services.AddTransient<ICourtScheduleService, CourtScheduleService>();
            services.AddTransient<IAccessoryService, AccessoryService>();
            services.AddTransient<IUserOtpService, UserOtpService>();
            services.AddTransient<EmailService, EmailService>();
            services.AddTransient<IAccessoryService, AccessoryService>();

            services.AddTransient<UserDAO, UserDAO>();
            services.AddTransient<UserOtpDAO, UserOtpDAO>();
            services.AddTransient<BookingDAO, BookingDAO>();
            services.AddTransient<AccessoryDAO, AccessoryDAO>();

            // Register UserRepository and UserService
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserOtpRepository, UserOtpRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IBookingRepository, BookingRepository>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<IAccessoryRepository, AccessoryRepository>();
            services.AddTransient<IAccessoryService, AccessoryService>();

            // Register ViewModels
            services.AddTransient<UserViewModel>();
            services.AddTransient<SidebarViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<BookingManagerViewModel>();
            services.AddTransient<AccessoryViewModel>();

            // Register MainWindow
            services.AddTransient<MainWindow>();
            services.AddTransient<UserView>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<ForgetPasswordWindow>();
            services.AddTransient<DashboardWindow>();
            services.AddTransient<BookingManageWindow>();
            services.AddTransient<CourtManageWindow>();
            services.AddTransient<ScheduleWindow>();
            services.AddTransient<AccessoryWindow>();
            services.AddTransient<ProfileWindow>();
            services.AddTransient<AccessoryWindow>();
        }

    }
}
