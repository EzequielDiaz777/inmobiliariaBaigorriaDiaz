using Microsoft.EntityFrameworkCore;
using inmobiliariaBaigorriaDiaz.controllers;
using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.AccessDeniedPath = "/Home";
    })

builder.Services.AddAuthorization(options => {
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
