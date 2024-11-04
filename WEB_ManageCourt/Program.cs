using BusinessLogic.Interface;
using BusinessLogic.Service;
using DataAccess.DAO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Repositories.Interface;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using WEB_ManageCourt.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WEB_ManageCourt.VNPAY;
using WEB_ManageCourt.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddScoped<BadmintonCourtDAO>();
builder.Services.AddScoped<IBadmintonCourtService, BadmintonCourtService>();
builder.Services.AddScoped<IAccessoryRepository, AccessoryRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<ICourtScheduleRepository, CourtScheduleRepository>();
builder.Services.AddScoped<IBadmintonCourtRepository, BadmintonCourtRepository>();
builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
builder.Services.AddScoped<ICourtImageRepository, CourtImageRepository>();

builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<BookingDAO>();
builder.Services.AddScoped<UserOtpDAO>();
builder.Services.AddScoped<AccessoryDAO>();
builder.Services.AddScoped<CourtScheduleDAO>();
builder.Services.AddScoped<CourtImageDAO>();
builder.Services.AddScoped<IUserOtpService, UserOtpService>();
builder.Services.AddScoped<ICourtScheduleService, CourtScheduleService>();
builder.Services.AddScoped<IAccessoryService, AccessoryService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourtImageService, CourtImageService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<EmailService, EmailService>();
builder.Services.AddHttpContextAccessor();
var supportedCultures = new[] { new CultureInfo("vi-VN") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRequestLocalization();
Utils.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");


app.Run();
