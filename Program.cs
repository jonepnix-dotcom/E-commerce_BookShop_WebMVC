using Microsoft.EntityFrameworkCore;
using TheLight_JoneBookShop_WebMVC.Data;
using TheLight_JoneBookShop_WebMVC.helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using TheLight_JoneBookShop_WebMVC.Service;

var builder = WebApplication.CreateBuilder(args);

var getConnectionStr = builder.Configuration.GetConnectionString("MyConnect");
builder.Services.AddDbContext<DbjonebookshopContext>(option => option.UseSqlServer(getConnectionStr));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

builder.Services.AddHttpClient<ShippingService>();

builder.Services.AddSingleton<IVnPayService, VnPayService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

//UseAuthorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options
    =>
    {
        options.LoginPath = "/dang-nhap";
        options.AccessDeniedPath = "/AccessDenied";
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["GoogleKeys:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
        //googleOptions.CallbackPath = "/dang-nhap-google"; // Đổi URL nếu cần
    });

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();


//Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.Cookie.Name = ".JoneBookShop.Session";
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
    option.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Setup "vi-VN"
var cultureInfo = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

ManagerTool.Initialize(configuration);

var app = builder.Build();

// Use localization
var supportedCultures = new[] { cultureInfo };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseMiddleware<SessionMiddleware>();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

// Cấu hình route trực tiếp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();