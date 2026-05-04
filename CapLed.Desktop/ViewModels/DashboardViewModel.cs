using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

/// <summary>
/// Dashboard ViewModel — aggregates stock KPIs from existing Equipment data.
/// No new API calls required: uses the same EquipmentService.GetAllAsync().
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    private readonly EquipmentService _equipmentService;

    // ─── KPI Properties ──────────────────────────────────────────────────────

    private int _totalArticles;
    public int TotalArticles { get => _totalArticles; set => SetProperty(ref _totalArticles, value); }

    private int _stockGlobal;
    public int StockGlobal { get => _stockGlobal; set => SetProperty(ref _stockGlobal, value); }

    private int _stockCasa;
    public int StockCasa { get => _stockCasa; set => SetProperty(ref _stockCasa, value); }

    private int _stockTanger;
    public int StockTanger { get => _stockTanger; set => SetProperty(ref _stockTanger, value); }

    private int _articlesCritiques;
    public int ArticlesCritiques { get => _articlesCritiques; set => SetProperty(ref _articlesCritiques, value); }

    private int _articlesRupture;
    public int ArticlesRupture { get => _articlesRupture; set => SetProperty(ref _articlesRupture, value); }

    private int _ruptureCasa;
    public int RuptureCasa { get => _ruptureCasa; set => SetProperty(ref _ruptureCasa, value); }

    private int _ruptureTanger;
    public int RuptureTanger { get => _ruptureTanger; set => SetProperty(ref _ruptureTanger, value); }

    private int _disponiblesCasaUniquement;
    public int DisponiblesCasaUniquement { get => _disponiblesCasaUniquement; set => SetProperty(ref _disponiblesCasaUniquement, value); }

    private int _disponiblesTangerUniquement;
    public int DisponiblesTangerUniquement { get => _disponiblesTangerUniquement; set => SetProperty(ref _disponiblesTangerUniquement, value); }

    // ─── Status indicators ────────────────────────────────────────────────────

    public string CasaStatus => RuptureCasa == 0 ? "OK" : "ALERTE";
    public string TangerStatus => RuptureTanger == 0 ? "OK" : "ALERTE";

    public string AlerteLocalMessage
    {
        get
        {
            int transferables = DisponiblesCasaUniquement + DisponiblesTangerUniquement;
            if (transferables == 0) return string.Empty;

            var parts = new List<string>();
            if (DisponiblesTangerUniquement > 0)
                parts.Add($"{DisponiblesTangerUniquement} article(s) en rupture à Casa mais disponible(s) à Tanger");
            if (DisponiblesCasaUniquement > 0)
                parts.Add($"{DisponiblesCasaUniquement} article(s) en rupture à Tanger mais disponible(s) à Casa");
            return string.Join(" · ", parts);
        }
    }

    public bool HasAlerteLocale => DisponiblesCasaUniquement > 0 || DisponiblesTangerUniquement > 0;

    // ─── Commands ────────────────────────────────────────────────────────────

    public ICommand RefreshCommand { get; }

    public DashboardViewModel(EquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
        RefreshCommand = new AsyncRelayCommand(LoadKpiAsync);
    }

    public async Task InitializeAsync() => await LoadKpiAsync();

    private async Task LoadKpiAsync()
    {
        BeginOperation();
        try
        {
            // Fetch all articles in one call (existing API, no new endpoint)
            var result = await _equipmentService.GetAllAsync(pageSize: 9999);
            var items = result.Items;

            TotalArticles = items.Count();
            StockGlobal = items.Sum(i => i.Quantity);
            StockCasa = items.Sum(i => i.QuantityCasa);
            StockTanger = items.Sum(i => i.QuantityTanger);

            ArticlesRupture = items.Count(i => i.Quantity == 0);
            ArticlesCritiques = items.Count(i =>
                i.AlertLevel == StockManager.Core.Domain.Enums.StockAlertLevel.CRITICAL ||
                i.AlertLevel == StockManager.Core.Domain.Enums.StockAlertLevel.OUT_OF_STOCK);

            RuptureCasa = items.Count(i => i.QuantityCasa == 0 && i.Quantity > 0);
            RuptureTanger = items.Count(i => i.QuantityTanger == 0 && i.Quantity > 0);

            // Articles en rupture locale mais disponibles dans l'autre dépôt
            DisponiblesCasaUniquement = items.Count(i => i.QuantityTanger == 0 && i.QuantityCasa > 0);
            DisponiblesTangerUniquement = items.Count(i => i.QuantityCasa == 0 && i.QuantityTanger > 0);

            OnPropertyChanged(nameof(CasaStatus));
            OnPropertyChanged(nameof(TangerStatus));
            OnPropertyChanged(nameof(AlerteLocalMessage));
            OnPropertyChanged(nameof(HasAlerteLocale));
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger les indicateurs : " + ex.Message;
        }
        finally
        {
            EndOperation();
        }
    }
}
