using AspNetCoreHero.ToastNotification;
using DoAn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// Configuration

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<DlctContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookingCatToc")));


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Add this line to enable session
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});
// Add services to the container.
builder.Services.AddControllersWithViews();
// Configure authorization policies
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireRole("Admin"));
//});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set your desired session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//      name: "Admin",
//      pattern: "{area:Admin}/{controller=Home}/{action=Index}/{id?}"
//    );
//});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Client}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Client}/{action=Index}/{id?}"
    );
});
app.Run();
