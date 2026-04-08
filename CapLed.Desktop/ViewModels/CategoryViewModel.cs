using System.Collections.ObjectModel;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class CategoryViewModel : BaseViewModel
{
    private readonly CategoryService _categoryService;
    private readonly FamilleService _familleService;
    private readonly IConfirmationService _confirmationService;

    // ─── Collections ─────────────────────────────────────────────────────────
    public ObservableCollection<CategoryModel> Categories { get; } = new();
    public ObservableCollection<FamilleModel> Familles { get; } = new();
    public ObservableCollection<string> GestionStockTypes { get; } = new() { "QUANTITE", "LOT", "SERIALISE" };

    // ─── Form Selection & Edit State ──────────────────────────────────────────
    private CategoryModel? _selectedCategory;
    public CategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    private string _label = string.Empty;
    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private int? _selectedFamilleId;
    public int? SelectedFamilleId
    {
        get => _selectedFamilleId;
        set => SetProperty(ref _selectedFamilleId, value);
    }

    private string _typeGestionStock = "QUANTITE";
    public string TypeGestionStock
    {
        get => _typeGestionStock;
        set => SetProperty(ref _typeGestionStock, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetProperty(ref _isEditMode, value))
            {
                OnPropertyChanged(nameof(FormTitle));
            }
        }
    }

    private int? _editingCategoryId;

    public string FormTitle => IsEditMode ? "Modifier la Catégorie" : "Nouvelle Catégorie";

    // ─── Quick-Add Famille panel ──────────────────────────────────────────────
    private bool _showAddFamillePanel;
    public bool ShowAddFamillePanel
    {
        get => _showAddFamillePanel;
        set => SetProperty(ref _showAddFamillePanel, value);
    }

    private string _nouvelleFamilleLibelle = string.Empty;
    public string NouvelleFamilleLibelle
    {
        get => _nouvelleFamilleLibelle;
        set => SetProperty(ref _nouvelleFamilleLibelle, value);
    }

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand RefreshCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand DeleteCategoryCommand { get; }   // called from DataGrid row
    public ICommand ClearFormCommand { get; }
    public ICommand ToggleAddFamilleCommand { get; }
    public ICommand SaveNewFamilleCommand { get; }

    public CategoryViewModel(CategoryService categoryService, FamilleService familleService, IConfirmationService confirmationService)
    {
        _categoryService = categoryService;
        _familleService = familleService;
        _confirmationService = confirmationService;

        RefreshCommand       = new AsyncRelayCommand(LoadDataAsync);
        AddCommand           = new RelayCommand(PrepareForAdd);
        EditCommand          = new RelayCommand(PrepareForEdit, () => SelectedCategory != null);
        SaveCommand          = new AsyncRelayCommand(SaveAsync, () => !IsSaving);
        DeleteCommand        = new AsyncRelayCommand(DeleteAsync, () => SelectedCategory != null);
        DeleteCategoryCommand = new AsyncRelayCommand(async p => await DeleteCategoryByRowAsync(p as CategoryModel));
        ClearFormCommand     = new RelayCommand(ClearForm);
        ToggleAddFamilleCommand = new RelayCommand(() => { ShowAddFamillePanel = !ShowAddFamillePanel; NouvelleFamilleLibelle = string.Empty; });
        SaveNewFamilleCommand   = new AsyncRelayCommand(SaveNewFamilleAsync, () => !IsSaving);
    }

    public async Task LoadDataAsync()
    {
        BeginOperation();

        try
        {
            var familles = await _familleService.GetAllAsync();
            Familles.Clear();
            foreach (var fam in familles) Familles.Add(fam);

            var result = await _categoryService.GetAllAsync();
            Categories.Clear();
            foreach (var cat in result)
            {
                Categories.Add(cat);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du chargement des catégories : " + ex.Message;
        }
        finally
        {
            EndOperation();
        }
    }

    private void PrepareForAdd()
    {
        ClearForm();
        IsEditMode = false;
        _editingCategoryId = null;
    }

    private void PrepareForEdit()
    {
        if (SelectedCategory == null) return;

        Label = SelectedCategory.Label;
        Description = SelectedCategory.Description;
        SelectedFamilleId = SelectedCategory.FamilleId;
        TypeGestionStock = !string.IsNullOrEmpty(SelectedCategory.TypeGestionStock) ? SelectedCategory.TypeGestionStock : "QUANTITE";
        
        IsEditMode = true;
        _editingCategoryId = SelectedCategory.Id;
        SuccessMessage = null;
        ErrorMessage = null;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Label))
        {
            ErrorMessage = "Le libellé est obligatoire.";
            return;
        }

        BeginSave();

        try
        {
            var model = new CategoryModel
            {
                Label = Label,
                Description = Description,
                FamilleId = SelectedFamilleId,
                TypeGestionStock = TypeGestionStock
            };

            bool success;
            if (IsEditMode && _editingCategoryId.HasValue)
            {
                model.Id = _editingCategoryId.Value;
                success = await _categoryService.UpdateAsync(_editingCategoryId.Value, model);
            }
            else
            {
                success = await _categoryService.CreateAsync(model);
            }

            if (success)
            {
                SuccessMessage = IsEditMode ? "Catégorie mise à jour." : "Catégorie créée.";
                if (!IsEditMode) ClearForm();
                await LoadDataAsync();
            }
            else
            {
                ErrorMessage = "L'opération a échoué. Veuillez vérifier les logs de l'API.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors de l'enregistrement : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedCategory == null) return;
        await DeleteCategoryByRowAsync(SelectedCategory);
    }

    private async Task DeleteCategoryByRowAsync(CategoryModel? cat)
    {
        if (cat == null) return;

        if (!_confirmationService.Confirm("Suppression", $"Supprimer la catégorie '{cat.Label}' ?"))
            return;

        BeginSave();
        try
        {
            bool success = await _categoryService.DeleteAsync(cat.Id);
            if (success)
            {
                SuccessMessage = "Catégorie supprimée.";
                if (SelectedCategory?.Id == cat.Id) ClearForm();
                await LoadDataAsync();
            }
            else
            {
                ErrorMessage = "Impossible de supprimer (catégorie liée à des équipements).";
            }
        }
        catch (Exception ex) { ErrorMessage = "Erreur suppression : " + ex.Message; }
        finally { EndSave(); }
    }

    private async Task SaveNewFamilleAsync()
    {
        if (string.IsNullOrWhiteSpace(NouvelleFamilleLibelle))
        {
            ErrorMessage = "Le libellé de la famille est obligatoire.";
            return;
        }

        BeginSave();
        try
        {
            var newFamille = new FamilleModel
            {
                Code    = NouvelleFamilleLibelle.ToUpperInvariant().Replace(" ", "-").Substring(0, Math.Min(20, NouvelleFamilleLibelle.Length)),
                Libelle = NouvelleFamilleLibelle
            };
            bool ok = await _familleService.CreateAsync(newFamille);
            if (ok)
            {
                SuccessMessage = $"Famille '{NouvelleFamilleLibelle}' créée.";
                ShowAddFamillePanel = false;
                NouvelleFamilleLibelle = string.Empty;
                // Reload familles and auto-select the new one
                var familles = await _familleService.GetAllAsync();
                Familles.Clear();
                foreach (var f in familles) Familles.Add(f);
                // Select the newly created family
                var created = Familles.LastOrDefault(f => f.Libelle == newFamille.Libelle);
                if (created != null) SelectedFamilleId = created.Id;
            }
            else { ErrorMessage = "Impossible de créer la famille."; }
        }
        catch (Exception ex) { ErrorMessage = "Erreur : " + ex.Message; }
        finally { EndSave(); }
    }

    private void ClearForm()
    {
        Label = string.Empty;
        Description = null;
        SelectedFamilleId = null;
        TypeGestionStock = "QUANTITE";
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
