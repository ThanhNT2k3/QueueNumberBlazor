using QMS.Web.Components;
using QMS.Application;
using QMS.Infrastructure;
using QMS.Infrastructure.Persistence;
using QMS.Web.Services;
using QMS.Web.Hubs;
using QMS.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DetailedErrors = true);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add QMS Notification Service
builder.Services.AddScoped<IQmsNotificationService, QmsNotificationService>();
builder.Services.AddSingleton<QmsEventService>();

// Add Authentication State Service as Scoped
builder.Services.AddScoped<QMS.Web.Services.AuthenticationStateService>();
// builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, QMS.Web.Auth.CustomAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "QMS_Session";
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<QmsDbContext>();
        await DbInitializer.InitializeAsync(context);
        
        // Seed Service Types with icons and colors
        await QMS.Web.Data.ServiceTypeSeedData.SeedServicesAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Disable HTTPS redirection to avoid port issues
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();

// Map SignalR Hubs
app.MapHub<QmsHub>("/qmshub");
app.MapHub<PrinterHub>("/printerhub");
Console.WriteLine("=== QMS Application Started ===");
Console.WriteLine("SignalR Hub mapped at: /qmshub");
Console.WriteLine("Printer Hub mapped at: /printerhub");
Console.WriteLine("Application URL: http://localhost:5101");
Console.WriteLine("================================");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
