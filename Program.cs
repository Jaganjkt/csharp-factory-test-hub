using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using WebApplication.Components;
using WebApplication.Data;
using WebApplication.Hubs;
using WebApplication.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=ericsson_testhub.db"));

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ITestSessionService, TestSessionService>();
builder.Services.AddScoped<IFaultAnalysisService, FaultAnalysisService>();
builder.Services.AddScoped<ITestDeviceService, MockRadioDeviceService>();

builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<TestMonitorHub>("/testhub");

app.Run();
