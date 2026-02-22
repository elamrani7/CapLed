using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CapLed.Desktop;

public partial class App : Application
{
    // ─── Service Provider ────────────────────────────────────────────────────
    public static IServiceProvider Services { get; private set; } = null!;

    // ─── API base address — change this to match your running API ────────────
    private const string ApiBaseAddress = "http://localhost:5115/";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Resolve and display MainWindow
        var mainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ── HttpClient ────────────────────────────────────────────────────────
        // All API services share one HttpClient configured with the base address.
        services.AddHttpClient<EquipmentService>(c => c.BaseAddress = new Uri(ApiBaseAddress));
        services.AddHttpClient<CategoryService> (c => c.BaseAddress = new Uri(ApiBaseAddress));
        services.AddHttpClient<StockService>    (c => c.BaseAddress = new Uri(ApiBaseAddress));
        services.AddHttpClient<AlertService>    (c => c.BaseAddress = new Uri(ApiBaseAddress));
        services.AddHttpClient<UserService>     (c => c.BaseAddress = new Uri(ApiBaseAddress));

        // ── ViewModels ────────────────────────────────────────────────────────
        // Child ViewModels are Transient (new instance each navigation if desired)
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<EquipmentListViewModel>();
        services.AddTransient<EquipmentDetailViewModel>();
        services.AddTransient<CategoryViewModel>();
        services.AddTransient<StockMovementViewModel>();
        services.AddTransient<AlertsViewModel>();
        services.AddTransient<UserViewModel>();

        // MainViewModel is Singleton so the shell persists
        services.AddSingleton<MainViewModel>();
    }
}
