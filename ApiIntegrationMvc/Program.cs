using UserManagement.Sdk;
using UserManagement.Sdk.Abstractions;
using UserManagement.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add MemoryCache globally
builder.Services.AddMemoryCache();

builder.Services.AddUserManagementSdk();

// Register the token provider (memory-based)
builder.Services.AddScoped<IAccessTokenProvider, MemoryCacheAccessTokenProvider>();


// Add services to the container.
builder.Services.AddControllersWithViews();

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
