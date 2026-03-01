using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels;
using System.Net.Http;

namespace CapLed.Desktop;

public partial class App : Application
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // ── Services ────────────────────────────────────────────────────
        services.AddSingleton(new HttpClient { BaseAddress = new Uri("http://localhost:5000/") });
        services.AddSingleton<IConfirmationService, WpfConfirmationService>();
        
        services.AddSingleton<EquipmentService>();
        services.AddSingleton<CategoryService>();
        services.AddSingleton<StockService>();
        services.AddSingleton<AlertService>();
        services.AddSingleton<UserService>();
        services.AddSingleton<AuthService>();

        // ── ViewModels ──────────────────────────────────────────────────
        services.AddSingleton<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<EquipmentListViewModel>();
        services.AddTransient<EquipmentDetailViewModel>();
        services.AddTransient<StockMovementViewModel>();
        services.AddTransient<CategoryViewModel>();
        services.AddTransient<AlertsViewModel>();
        services.AddTransient<UserViewModel>();
        services.AddTransient<LoginViewModel>();

        // Factory to solve circular dependency
        services.AddSingleton<Func<MainViewModel, LoginViewModel>>(provider => 
            (mainVm) => new LoginViewModel(provider.GetRequiredService<AuthService>(), mainVm));

        // ── Views ───────────────────────────────────────────────────────
        services.AddSingleton<MainWindow>(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainViewModel>()
        });
    }
}
