using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication1;
using WebApplication1.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<CleanupHostedService>();
builder.Services.AddSingleton<OnlineUsersService>();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Account/Login"; // Куда перенаправлять, если не авторизован
        options.AccessDeniedPath = "/Account/AccessDenied"; // Опционально
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Срок жизни куки
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ServiceRequests}/{action=Index}/{id?}");
app.MapControllerRoute(
        name: "account",
        pattern: "{controller=Account}/{action=Register}");
app.MapControllerRoute(
        name: "account",
        pattern: "{controller=Account}/{action=Login}");
app.Run();
