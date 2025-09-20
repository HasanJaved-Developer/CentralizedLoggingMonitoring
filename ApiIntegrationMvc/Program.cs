using Microsoft.AspNetCore.Authentication.Cookies;
using UserManagement.Sdk.Extensions;
using CentralizedLogging.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add MemoryCache globally
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor(); // required for the above

builder.Services.AddUserManagementSdk();
builder.Services.AddCentralizedLoggingSdk();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Cookie auth for the web app (UI)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.LogoutPath = "/Account/Logout";
        o.AccessDeniedPath = "/Account/Denied";
        // optional:
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "root_to_login",
    pattern: "",
    defaults: new { area = "Account", controller = "Login", action = "Index" });

app.Run();
