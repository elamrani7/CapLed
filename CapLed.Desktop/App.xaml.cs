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
        services.AddSingleton(new HttpClient { BaseAddress = new Uri("https://capled-api.onrender.com/") });
        services.AddSingleton<IConfirmationService, WpfConfirmationService>();
        
        services.AddSingleton<EquipmentService>();
        services.AddSingleton<CategoryService>();
        services.AddSingleton<FamilleService>();
        services.AddSingleton<StockService>();
        services.AddSingleton<AlertService>();
        services.AddSingleton<UserService>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<StockDetailService>();
        
        services.AddSingleton<CrmApiClient>();
        services.AddSingleton<DocumentApiClient>();

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
        
        services.AddTransient<CapLed.Desktop.ViewModels.CRM.LeadsViewModel>();
        services.AddTransient<CapLed.Desktop.ViewModels.CRM.DocumentsViewModel>();

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
