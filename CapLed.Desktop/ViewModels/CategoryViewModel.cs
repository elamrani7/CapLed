using System.Collections.ObjectModel;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class CategoryViewModel : BaseViewModel
{
    private readonly CategoryService _categoryService;
    private readonly IConfirmationService _confirmationService;

    // ─── Collections ─────────────────────────────────────────────────────────
    public ObservableCollection<CategoryModel> Categories { get; } = new();

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

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand RefreshCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFormCommand { get; }

    public CategoryViewModel(CategoryService categoryService, IConfirmationService confirmationService)
    {
        _categoryService = categoryService;
        _confirmationService = confirmationService;

        RefreshCommand = new AsyncRelayCommand(LoadCategoriesAsync);
        AddCommand = new RelayCommand(PrepareForAdd);
        EditCommand = new RelayCommand(PrepareForEdit, () => SelectedCategory != null);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsSaving);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedCategory != null);
        ClearFormCommand = new RelayCommand(ClearForm);
    }

    public async Task LoadCategoriesAsync()
    {
        BeginOperation();

        try
        {
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
                Description = Description
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
                await LoadCategoriesAsync();
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

        if (!_confirmationService.Confirm("Suppression", $"Voulez-vous vraiment supprimer la catégorie '{SelectedCategory.Label}' ?"))
            return;
        
        BeginSave();

        try
        {
            bool success = await _categoryService.DeleteAsync(SelectedCategory.Id);
            if (success)
            {
                SuccessMessage = "Catégorie supprimée.";
                ClearForm();
                await LoadCategoriesAsync();
            }
            else
            {
                ErrorMessage = "Impossible de supprimer la catégorie (elle est probablement liée à des équipements).";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors de la suppression : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }

    private void ClearForm()
    {
        Label = string.Empty;
        Description = null;
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
