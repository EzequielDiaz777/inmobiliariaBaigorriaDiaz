using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
/*
Inmobiliaria:
- Corregir renovar contrato
- Mejorar el crear inmueble (LISTO)
- Mejorar el buscar inmueble (PREGUNTAR AL PROFE)
- Corregir actualizar contrato
- Corregir el 'Volver' en Renovar contrato (LISTO)
- Corregir el buscador para que no sea por JS
- Verificar los páginados de los index
- Corregir details de propietario
*/
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.AccessDeniedPath = "/Home";
    options.LoginPath = "/Home/Login";
    options.LogoutPath = "/Home/Logout";
    options.Events.OnRedirectToLogin = context =>
    {
        // En lugar de pasar ReturnUrl, simplemente redirige al usuario a la página de inicio de sesión
        context.Response.Redirect("/Home/Login");
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // Redirige a la página de inicio sin agregar ReturnUrl
        context.Response.Redirect("/Home");
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
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