using CapLed.Desktop.ViewModels.Base;
using CapLed.Desktop.Core;
using CapLed.Desktop.Services;
using System.Windows.Input;

namespace CapLed.Desktop.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly AppSession _session = AppSession.Current;
    private readonly AuthService _authService;

    // ─── Injected child ViewModels ────────────────────────────────────────────
    private readonly DashboardViewModel _dashboardVm;
    private readonly EquipmentListViewModel _equipmentListVm;
    private readonly EquipmentDetailViewModel _equipmentDetailVm;
    private readonly CategoryViewModel _categoryVm;
    private readonly StockMovementViewModel _stockVm;
    private readonly AlertsViewModel _alertsVm;
    private readonly UserViewModel _userVm;
    private readonly LoginViewModel _loginVm;
    private readonly IConfirmationService _confirmationService;

    // ─── Authentication State ─────────────────────────────────────────────────
    public bool IsAuthenticated => _session.IsAuthenticated;
    public bool IsAdmin => _session.IsAdmin;
    public string UserFullName => _session.FullName;
    public string UserRoleDisplay => _session.Role == "ADMIN" ? "Administrateur" : "Gestionnaire Stock";

    // ─── Navigation state ─────────────────────────────────────────────────────
    private BaseViewModel _currentViewModel = null!;
    public BaseViewModel CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

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
    public ICommand LogoutCommand { get; }

    public MainViewModel(
        AuthService authService,
        DashboardViewModel dashboardVm,
        EquipmentListViewModel equipmentListVm,
        EquipmentDetailViewModel equipmentDetailVm,
        CategoryViewModel categoryVm,
        StockMovementViewModel stockVm,
        AlertsViewModel alertsVm,
        UserViewModel userVm,
        IConfirmationService confirmationService,
        Func<MainViewModel, LoginViewModel> loginVmFactory)
    {
        _authService = authService;
        _dashboardVm = dashboardVm;
        _equipmentListVm = equipmentListVm;
        _equipmentDetailVm = equipmentDetailVm;
        _categoryVm = categoryVm;
        _stockVm = stockVm;
        _alertsVm = alertsVm;
        _userVm = userVm;
        _confirmationService = confirmationService;
        
        // Use factory to avoid circular dependency
        _loginVm = loginVmFactory(this);

        _equipmentListVm.NavigateToDetailRequested = (id) => NavigateToEquipmentDetail(id);
        _equipmentDetailVm.NavigateToListRequested = (refresh) => NavigateToEquipmentList(refresh);

        ShowDashboardCommand  = new RelayCommand(() => Navigate("Dashboard",  _dashboardVm));
        ShowEquipmentCommand  = new RelayCommand(() => Navigate("Equipment",  _equipmentListVm));
        ShowCategoriesCommand = new RelayCommand(() => Navigate("Categories", _categoryVm));
        ShowStockCommand      = new RelayCommand(() => Navigate("Stock",      _stockVm));
        ShowAlertsCommand     = new RelayCommand(() => Navigate("Alerts",     _alertsVm));
        ShowUsersCommand      = new RelayCommand(() => Navigate("Users",      _userVm));
        LogoutCommand         = new RelayCommand(ExecuteLogout);

        // Initial view
        if (IsAuthenticated)
        {
            if (IsAdmin)
                Navigate("Dashboard", _dashboardVm);
            else
                Navigate("Equipment", _equipmentListVm);
        }
        else
        {
            ShowLogin();
        }
    }

    public void ShowLogin()
    {
        ActiveSection = "Login";
        CurrentViewModel = _loginVm;
        OnPropertyChanged(nameof(IsAuthenticated));
        OnPropertyChanged(nameof(IsAdmin));
        OnPropertyChanged(nameof(UserFullName));
    }

    private void ExecuteLogout()
    {
        if (_confirmationService.Confirm("Déconnexion", "Voulez-vous vraiment vous déconnecter ?"))
        {
            _authService.Logout();
            ShowLogin();
        }
    }

    private async void Navigate(string section, BaseViewModel viewModel)
    {
        if (!IsAuthenticated && section != "Login")
        {
            ShowLogin();
            return;
        }

        ActiveSection = section;
        CurrentViewModel = viewModel;
        
        // Notify UI about session changes in case we just logged in
        OnPropertyChanged(nameof(IsAuthenticated));
        OnPropertyChanged(nameof(IsAdmin));
        OnPropertyChanged(nameof(UserFullName));
        OnPropertyChanged(nameof(UserRoleDisplay));

        if (viewModel is EquipmentListViewModel eqVm) await eqVm.InitializeAsync();
        else if (viewModel is StockMovementViewModel svm) await svm.InitializeAsync();
        else if (viewModel is CategoryViewModel cvm) await cvm.LoadCategoriesAsync();
        else if (viewModel is AlertsViewModel avm) await avm.InitializeAsync();
        else if (viewModel is UserViewModel uvm) await uvm.InitializeAsync();
    }

    public async Task NavigateToEquipmentDetail(int? id = null)
    {
        if (!IsAuthenticated) { ShowLogin(); return; }
        ActiveSection = "Equipment";
        await _equipmentDetailVm.LoadAsync(id);
        CurrentViewModel = _equipmentDetailVm;
    }

    public async Task NavigateToEquipmentList(bool refresh = false)
    {
        if (!IsAuthenticated) { ShowLogin(); return; }
        ActiveSection = "Equipment";
        if (refresh) await _equipmentListVm.InitializeAsync();
        CurrentViewModel = _equipmentListVm;
    }
}
