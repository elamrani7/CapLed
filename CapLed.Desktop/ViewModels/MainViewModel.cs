using CapLed.Desktop.ViewModels.Base;
using System.Windows.Input;

namespace CapLed.Desktop.ViewModels;

/// <summary>
/// Root ViewModel for the shell window.
/// Controls which view is currently displayed via CurrentViewModel.
/// Exposes navigation commands bound to the sidebar buttons.
/// </summary>
public class MainViewModel : BaseViewModel
{
    // ─── Injected child ViewModels ────────────────────────────────────────────
    private readonly DashboardViewModel _dashboardVm;
    private readonly EquipmentListViewModel _equipmentListVm;
    private readonly EquipmentDetailViewModel _equipmentDetailVm;
    private readonly CategoryViewModel _categoryVm;
    private readonly StockMovementViewModel _stockVm;
    private readonly AlertsViewModel _alertsVm;
    private readonly UserViewModel _userVm;

    // ─── Navigation state ─────────────────────────────────────────────────────
    private BaseViewModel _currentViewModel = null!;

    /// <summary>The view currently displayed in the ContentControl.</summary>
    public BaseViewModel CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    // ── Active nav item (to highlight the selected sidebar button) ────────────
    private string _activeSection = "Dashboard";
    public string ActiveSection
    {
        get => _activeSection;
        private set => SetProperty(ref _activeSection, value);
    }

    // ─── Navigation commands ──────────────────────────────────────────────────
    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowEquipmentCommand { get; }
    public ICommand ShowCategoriesCommand { get; }
    public ICommand ShowStockCommand { get; }
    public ICommand ShowAlertsCommand { get; }
    public ICommand ShowUsersCommand { get; }

    // ─── Constructor (all dependencies injected by DI) ────────────────────────
    public MainViewModel(
        DashboardViewModel dashboardVm,
        EquipmentListViewModel equipmentListVm,
        EquipmentDetailViewModel equipmentDetailVm,
        CategoryViewModel categoryVm,
        StockMovementViewModel stockVm,
        AlertsViewModel alertsVm,
        UserViewModel userVm)
    {
        _dashboardVm     = dashboardVm;
        _equipmentListVm = equipmentListVm;
        _equipmentDetailVm = equipmentDetailVm;
        _categoryVm      = categoryVm;
        _stockVm         = stockVm;
        _alertsVm        = alertsVm;
        _userVm          = userVm;

        // Break circular dependencies by wiring up delegates
        _equipmentListVm.NavigateToDetailRequested = (id) => NavigateToEquipmentDetail(id);
        _equipmentDetailVm.NavigateToListRequested = (refresh) => NavigateToEquipmentList(refresh);

        // Wire navigation commands
        ShowDashboardCommand  = new RelayCommand(() => Navigate("Dashboard",  _dashboardVm));
        ShowEquipmentCommand  = new RelayCommand(() => Navigate("Equipment",  _equipmentListVm));
        ShowCategoriesCommand = new RelayCommand(() => Navigate("Categories", _categoryVm));
        ShowStockCommand      = new RelayCommand(() => Navigate("Stock",      _stockVm));
        ShowAlertsCommand     = new RelayCommand(() => Navigate("Alerts",     _alertsVm));
        ShowUsersCommand      = new RelayCommand(() => Navigate("Users",      _userVm));

        // Default view: Dashboard
        Navigate("Dashboard", _dashboardVm);
    }

    // ─── Navigation helper ────────────────────────────────────────────────────
    private async void Navigate(string section, BaseViewModel viewModel)
    {
        ActiveSection = section;
        CurrentViewModel = viewModel;

        if (viewModel is EquipmentListViewModel eqVm)
        {
            await eqVm.InitializeAsync();
        }
        else if (viewModel is StockMovementViewModel stockVm)
        {
            await stockVm.InitializeAsync();
        }
        else if (viewModel is CategoryViewModel catVm)
        {
            await catVm.LoadCategoriesAsync();
        }
    }

    /// <summary>
    /// Navigates to the Equipment Detail screen.
    /// If id is provided, it's Edit mode; otherwise, it's Create mode.
    /// </summary>
    public async Task NavigateToEquipmentDetail(int? id = null)
    {
        ActiveSection = "Equipment";
        await _equipmentDetailVm.LoadAsync(id);
        CurrentViewModel = _equipmentDetailVm;
    }

    /// <summary>
    /// Navigates back to the Equipment List screen.
    /// </summary>
    public async Task NavigateToEquipmentList(bool refresh = false)
    {
        ActiveSection = "Equipment";
        if (refresh) await _equipmentListVm.InitializeAsync();
        CurrentViewModel = _equipmentListVm;
    }
}
