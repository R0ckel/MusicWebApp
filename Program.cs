using Microsoft.AspNetCore.Authentication.Cookies;
using MusicWebApp.DataAccess.Accessors;
using MusicWebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UserDAO>();
builder.Services.AddSingleton<AuthorDAO>();
builder.Services.AddSingleton<GenreDAO>();
builder.Services.AddSingleton<AlbumDAO>();
builder.Services.AddSingleton<TrackDAO>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
