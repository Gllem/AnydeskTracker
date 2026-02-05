using System.Net;
using AnydeskTracker.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using AnydeskTracker.Services;
using AnydeskTracker.Services.MetrikaServices;
using DotNetEnv;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

#region ServiceRegistration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders().AddDefaultUI();

builder.Services.AddHttpClient("Beget")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var cookies = new CookieContainer();

        cookies.Add(new Cookie("beget", "begetok", "/", "moshelovka.onf.ru"));
        
        return new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookies
        };
    });

builder.Services.AddScoped<UserWorkService>();
builder.Services.AddScoped<TelegramService>();
builder.Services.AddScoped<PcService>();
builder.Services.AddScoped<ParserService>();
builder.Services.AddScoped<AgentActionsService>();
builder.Services.AddScoped<AgentCommandsService>();
builder.Services.AddScoped<AgentGamesUpdater>();
builder.Services.AddScoped<YandexMetrikaService>();
builder.Services.AddScoped<SheetsService>((x) => new SheetsService(new BaseClientService.Initializer()
{
    ApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY")
}));

builder.Services.AddHostedService<PcStatusUpdater>();
builder.Services.AddHostedService<ActionCleanupService>();
builder.Services.AddHostedService<TelegramNotifier>();
builder.Services.AddHostedService<DailyParserService>();

builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<UserActionService>();
builder.Services.AddHttpContextAccessor();
#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
}

#region Configuration
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<AgentHub>("/hubs/agent");

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

SeedAdminUser(app);

app.Run();

#endregion

async void SeedAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@anydesk.local";
    string adminPassword = "Admin123!";

    // Создаем роль, если её нет
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Проверяем, есть ли уже такой пользователь
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine("✅ Админ создан: " + adminEmail);
        }
        else
        {
            Console.WriteLine("❌ Ошибка при создании админа:");
            foreach (var error in result.Errors)
                Console.WriteLine($"  - {error.Description}");
        }
    }
    else
    {
        Console.WriteLine("ℹ️ Админ уже существует: " + adminEmail);
    }
}
