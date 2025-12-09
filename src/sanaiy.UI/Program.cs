using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using sanaiy.Application.Services;
using sanaiy.BLL.Interfaces;
using sanaiy.BLL.Services;
using sanaiy.BLL.Services.Implementations;
using sanaiy.DAL.Context;
using sanaiy.DAL.Repositories;
using sanaiy.BLL.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Bind AdminAccount options from configuration
builder.Services.Configure<AdminAccountOptions>(
    builder.Configuration.GetSection("AdminAccount")
);

// 1. Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);

// Distributed cache + Session
builder.Services.AddDistributedMemoryCache();

// 2. Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ICraftsmanService, CraftsmanService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(sanaiy.BLL.Mappings.MappingProfile));

// 4. Authentication & Cookie Configuration
builder.Services.AddAuthentication("sanaiyCookie")
    .AddCookie("sanaiyCookie", options =>
    {
        // LoginPath pointed to non-existing controller ClientAuth; correct to UserAuth
        options.LoginPath = "/UserAuth/Login";
        options.AccessDeniedPath = "/UserAuth/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    // add policies if needed
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
