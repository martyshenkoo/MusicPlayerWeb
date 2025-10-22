using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using MusicPlayerWeb.Data;
using MusicPlayerWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// MySQL через EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Auth (cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddControllersWithViews();

// DI: замість InMemory — реальні Db-сервіси
builder.Services.AddScoped<IUserService, DbUserService>();
builder.Services.AddScoped<ITrackService, DbTrackService>();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// статичні файли + коректні MIME для аудіо
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".m4a"] = "audio/mp4";
provider.Mappings[".flac"] = "audio/flac";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// створюємо папку для аплоадів
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

app.Run();
