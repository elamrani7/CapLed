using System.Collections.ObjectModel;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class StockMovementViewModel : BaseViewModel
{
    private readonly StockService _stockService;
    private readonly EquipmentService _equipmentService;

    // ─── Collections ─────────────────────────────────────────────────────────
    public ObservableCollection<StockMovementModel> Movements { get; } = new();
    public ObservableCollection<EquipmentListItemModel> EquipmentChoices { get; } = new();

    // ─── Filters ─────────────────────────────────────────────────────────────
    private EquipmentListItemModel? _selectedFilterEquipment;
    public EquipmentListItemModel? SelectedFilterEquipment
    {
        get => _selectedFilterEquipment;
        set => SetProperty(ref _selectedFilterEquipment, value);
    }

    private string _selectedFilterType = "Touts"; // "Touts", "ENTRY", "EXIT"
    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    private DateTime? _filterDateFrom;
    public DateTime? FilterDateFrom
    {
        get => _filterDateFrom;
        set => SetProperty(ref _filterDateFrom, value);
    }

    private DateTime? _filterDateTo;
    public DateTime? FilterDateTo
    {
        get => _filterDateTo;
        set => SetProperty(ref _filterDateTo, value);
    }

    // ─── New Movement Form ────────────────────────────────────────────────────
    private EquipmentListItemModel? _selectedEquipmentForNew;
    public EquipmentListItemModel? SelectedEquipmentForNew
    {
        get => _selectedEquipmentForNew;
        set => SetProperty(ref _selectedEquipmentForNew, value);
    }

    private string _newMovementType = "ENTRY";
    public string NewMovementType
    {
        get => _newMovementType;
        set => SetProperty(ref _newMovementType, value);
    }

    private int _movementQuantity = 1;
    public int MovementQuantity
    {
        get => _movementQuantity;
        set => SetProperty(ref _movementQuantity, value);
    }

    private DateTime _movementDate = DateTime.Now;
    public DateTime MovementDate
    {
        get => _movementDate;
        set => SetProperty(ref _movementDate, value);
    }

    private string? _movementComment;
    public string? MovementComment
    {
        get => _movementComment;
        set => SetProperty(ref _movementComment, value);
    }

    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set => SetProperty(ref _isSaving, value);
    }

    // ─── Paging ──────────────────────────────────────────────────────────────
    private int _page = 1;
    public int Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }

    private int _pageSize = 15;
    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        private set 
        { 
            SetProperty(ref _totalCount, value);
            OnPropertyChanged(nameof(TotalPages));
        }
    }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand RefreshCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand RegisterMovementCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    public StockMovementViewModel(StockService stockService, EquipmentService equipmentService)
    {
        _stockService = stockService;
        _equipmentService = equipmentService;

        RefreshCommand = new AsyncRelayCommand(async () => { Page = 1; await LoadHistoryAsync(); });
        ClearFiltersCommand = new AsyncRelayCommand(ClearFiltersAsync);
        RegisterMovementCommand = new AsyncRelayCommand(RegisterMovementAsync, () => !IsSaving);
        
        PreviousPageCommand = new AsyncRelayCommand(async () => { if (Page > 1) { Page--; await LoadHistoryAsync(); } });
        NextPageCommand = new AsyncRelayCommand(async () => { if (Page < TotalPages) { Page++; await LoadHistoryAsync(); } });
    }

    public async Task InitializeAsync()
    {
        await LoadEquipmentChoicesAsync();
        await LoadHistoryAsync();
    }

    private async Task LoadEquipmentChoicesAsync()
    {
        try
        {
            var equipments = await _equipmentService.GetAllAsync(pageSize: 100);
            EquipmentChoices.Clear();
            foreach (var e in equipments.Items) EquipmentChoices.Add(e);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur chargement équipements: " + ex.Message;
        }
    }

    private async Task LoadHistoryAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var filter = new StockMovementFilter
            {
                EquipmentId = SelectedFilterEquipment?.Id,
                Type = SelectedFilterType == "Touts" ? null : SelectedFilterType,
                DateFrom = FilterDateFrom,
                DateTo = FilterDateTo,
                Page = Page,
                PageSize = PageSize
            };

            var result = await _stockService.GetFilteredHistoryAsync(filter);
            Movements.Clear();
            foreach (var m in result.Items) Movements.Add(m);
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du chargement de l'historique : " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ClearFiltersAsync()
    {
        SelectedFilterEquipment = null;
        SelectedFilterType = "Touts";
        FilterDateFrom = null;
        FilterDateTo = null;
        Page = 1;
        await LoadHistoryAsync();
    }

    private async Task RegisterMovementAsync()
    {
        if (SelectedEquipmentForNew == null)
        {
            ErrorMessage = "Veuillez sélectionner un équipement.";
            return;
        }

        if (MovementQuantity <= 0)
        {
            ErrorMessage = "La quantité doit être supérieure à 0.";
            return;
        }

        IsSaving = true;
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var model = new StockMovementCreateModel
            {
                EquipmentId = SelectedEquipmentForNew.Id,
                Quantity = MovementQuantity,
                Type = NewMovementType,
                Date = MovementDate,
                Comment = MovementComment
            };

            StockMovementModel? result;
            if (NewMovementType == "ENTRY")
                result = await _stockService.RecordEntryAsync(model);
            else
                result = await _stockService.RecordExitAsync(model);

            if (result != null)
            {
                SuccessMessage = "Mouvement enregistré avec succès.";
                // Reset form
                MovementComment = string.Empty;
                MovementQuantity = 1;
                // Refresh list
                await LoadHistoryAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors de l'enregistrement : " + ex.Message;
        }
        finally
        {
            IsSaving = false;
        }
    }
}
