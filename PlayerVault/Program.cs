using Microsoft.EntityFrameworkCore;
using PlayerVault.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔗 Connect to SQL Server using your context
builder.Services.AddDbContext<PlayerVaultContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PlayerVaultContext")));

// 📦 Add MVC support
builder.Services.AddControllersWithViews();

// 🧠 Add Memory Cache (required for session)
builder.Services.AddDistributedMemoryCache();

// 🔐 Add Session Service with options
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session lasts 30 mins idle
    options.Cookie.HttpOnly = true;                  // Secure cookie (JS can't touch)
    options.Cookie.IsEssential = true;               // Always stored, even without consent
});

var app = builder.Build();

// 🔧 Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🧩 Use session before authorization
app.UseSession();

app.UseAuthorization();

// 🚦 Set up route defaults
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
