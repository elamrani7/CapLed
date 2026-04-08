using System.Collections.ObjectModel;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class EquipmentListViewModel : BaseViewModel
{
    private readonly EquipmentService _equipmentService;
    private readonly CategoryService _categoryService;
    private readonly FamilleService _familleService;
    private readonly IConfirmationService _confirmationService;

    // ─── Collections ─────────────────────────────────────────────────────────
    public ObservableCollection<EquipmentListItemModel> EquipmentItems { get; } = new();
    public ObservableCollection<CategoryModel> Categories { get; } = new();
    public ObservableCollection<FamilleModel> Familles { get; } = new();

    // ─── Selection & Filters ──────────────────────────────────────────────────
    private EquipmentListItemModel? _selectedEquipment;
    public EquipmentListItemModel? SelectedEquipment
    {
        get => _selectedEquipment;
        set => SetProperty(ref _selectedEquipment, value);
    }

    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private FamilleModel? _selectedFamille;
    public FamilleModel? SelectedFamille
    {
        get => _selectedFamille;
        set
        {
            if (SetProperty(ref _selectedFamille, value))
            {
                // Optionally clear category when famille changes
                SelectedCategory = null;
                // Auto-refresh could be triggering here, but we bind 'RefreshCommand' to the combo box usually, or just let users click button
            }
        }
    }

    private CategoryModel? _selectedCategory;
    public CategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    private string? _selectedCondition;
    public string? SelectedCondition
    {
        get => _selectedCondition;
        set => SetProperty(ref _selectedCondition, value);
    }

    public List<string> ConditionOptions { get; } = new() { "Tous", "NEW", "USED", "DAMAGED", "REPAIRING" };

    // ─── Pagination ──────────────────────────────────────────────────────────
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
        set => SetProperty(ref _totalCount, value);
    }

    private int _totalPages;
    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand AddEquipmentCommand { get; }
    public ICommand EditEquipmentCommand { get; }
    public ICommand DeleteEquipmentCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    // ─── Navigation Support ───────────────────────────────────────────────────
    public Func<int?, Task>? NavigateToDetailRequested { get; set; }

    public EquipmentListViewModel(EquipmentService equipmentService, CategoryService categoryService, FamilleService familleService, IConfirmationService confirmationService)
    {
        _equipmentService = equipmentService;
        _categoryService = categoryService;
        _familleService = familleService;
        _confirmationService = confirmationService;

        _selectedCondition = ConditionOptions[0]; // "Tous"

        RefreshCommand = new AsyncRelayCommand(async () => { Page = 1; await LoadEquipmentAsync(); });
        SearchCommand = new AsyncRelayCommand(async () => { Page = 1; await LoadEquipmentAsync(); });
        ClearFiltersCommand = new AsyncRelayCommand(ClearFiltersAsync);
        
        AddEquipmentCommand = new RelayCommand(AddEquipment);
        EditEquipmentCommand = new RelayCommand(EditEquipment, () => SelectedEquipment != null);
        DeleteEquipmentCommand = new AsyncRelayCommand(DeleteEquipmentAsync, () => SelectedEquipment != null);

        PreviousPageCommand = new AsyncRelayCommand(async () => { if (Page > 1) { Page--; await LoadEquipmentAsync(); } }, () => Page > 1);
        NextPageCommand = new AsyncRelayCommand(async () => { if (Page < TotalPages) { Page++; await LoadEquipmentAsync(); } }, () => Page < TotalPages);
    }

    public async Task InitializeAsync()
    {
        await LoadFamillesAsync();
        await LoadCategoriesAsync();
        await LoadEquipmentAsync();
    }

    private async Task LoadFamillesAsync()
    {
        try
        {
            var fams = await _familleService.GetAllAsync();
            Familles.Clear();
            if (fams != null)
            {
                foreach (var f in fams) Familles.Add(f);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger les familles : " + ex.Message;
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var cats = await _categoryService.GetAllAsync();
            Categories.Clear();
            foreach (var c in cats) Categories.Add(c);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger les catégories : " + ex.Message;
        }
    }

    private async Task LoadEquipmentAsync()
    {
        BeginOperation();

        try
        {
            string? conditionParam = SelectedCondition == "Tous" ? null : SelectedCondition;
            
            var result = await _equipmentService.GetAllAsync(
                familleId: SelectedFamille?.Id,
                categoryId: SelectedCategory?.Id,
                condition: conditionParam,
                search: SearchText,
                page: Page,
                pageSize: PageSize
            );

            EquipmentItems.Clear();
            foreach (var item in result.Items)
            {
                EquipmentItems.Add(item);
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du chargement des équipements : " + ex.Message;
        }
        finally
        {
            EndOperation();
        }
    }

    private async Task ClearFiltersAsync()
    {
        SearchText = null;
        SelectedFamille = null;
        SelectedCategory = null;
        SelectedCondition = ConditionOptions[0];
        Page = 1;
        await LoadEquipmentAsync();
    }

    private async void AddEquipment()
    {
        if (NavigateToDetailRequested != null) await NavigateToDetailRequested.Invoke(null);
    }

    private async void EditEquipment()
    {
        if (SelectedEquipment == null) return;
        if (NavigateToDetailRequested != null) await NavigateToDetailRequested.Invoke(SelectedEquipment.Id);
    }

    private async Task DeleteEquipmentAsync()
    {
        if (SelectedEquipment == null) return;

        if (!_confirmationService.Confirm("Suppression", $"Voulez-vous vraiment supprimer l'équipement '{SelectedEquipment.Name}' ?"))
            return;

        BeginSave();
        try
        {
            bool success = await _equipmentService.DeleteAsync(SelectedEquipment.Id);
            if (success)
            {
                await LoadEquipmentAsync();
                SuccessMessage = "Équipement supprimé avec succès.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Échec de la suppression : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }
}
