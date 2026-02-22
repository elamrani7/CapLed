using System.Collections.ObjectModel;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class EquipmentDetailViewModel : BaseViewModel
{
    private readonly EquipmentService _equipmentService;
    private readonly CategoryService _categoryService;

    // ─── Mode & ID ───────────────────────────────────────────────────────────
    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        private set => SetProperty(ref _isEditMode, value);
    }

    private int? _id;
    public int? Id
    {
        get => _id;
        private set => SetProperty(ref _id, value);
    }

    // ─── Editable Fields ──────────────────────────────────────────────────────
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _reference = string.Empty;
    public string Reference
    {
        get => _reference;
        set => SetProperty(ref _reference, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _selectedCondition = "NEW";
    public string SelectedCondition
    {
        get => _selectedCondition;
        set => SetProperty(ref _selectedCondition, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    private CategoryModel? _selectedCategory;
    public CategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public ObservableCollection<CategoryModel> Categories { get; } = new();
    public List<string> ConditionOptions { get; } = new() { "NEW", "USED", "DAMAGED", "REPAIRING" };

    // ─── State ───────────────────────────────────────────────────────────────
    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        private set => SetProperty(ref _isSaving, value);
    }

    public string Title => IsEditMode ? "Modifier l'Équipement" : "Nouvel Équipement";

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    // ─── Navigation Support ───────────────────────────────────────────────────
    public Func<bool, Task>? NavigateToListRequested { get; set; }

    public EquipmentDetailViewModel(
        EquipmentService equipmentService, 
        CategoryService categoryService)
    {
        _equipmentService = equipmentService;
        _categoryService = categoryService;

        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsSaving);
        CancelCommand = new AsyncRelayCommand(CancelAsync);
    }

    /// <summary>
    /// Initializes the ViewModel for Create or Edit mode.
    /// </summary>
    public async Task LoadAsync(int? id = null)
    {
        IsLoading = true;
        ErrorMessage = null;
        SuccessMessage = null;
        Id = id;
        IsEditMode = id.HasValue;

        try
        {
            // Always reload categories to ensure dropdown is fresh
            await LoadCategoriesAsync();

            if (IsEditMode && id.HasValue)
            {
                var equipment = await _equipmentService.GetByIdAsync(id.Value);
                if (equipment != null)
                {
                    Name = equipment.Name;
                    Reference = equipment.Reference;
                    Description = equipment.Description ?? string.Empty;
                    SelectedCondition = equipment.Condition;
                    Quantity = equipment.Quantity;
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == equipment.CategoryId);
                }
                else
                {
                    ErrorMessage = "Équipement introuvable.";
                }
            }
            else
            {
                // Reset fields for Create mode
                Name = string.Empty;
                Reference = string.Empty;
                Description = string.Empty;
                SelectedCondition = "NEW";
                Quantity = 0;
                SelectedCategory = null;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur de chargement : " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var cats = await _categoryService.GetAllAsync();
        Categories.Clear();
        foreach (var c in cats) Categories.Add(c);
    }

    private async Task SaveAsync()
    {
        if (!Validate()) return;

        IsSaving = true;
        ErrorMessage = null;

        try
        {
            var editModel = new EquipmentEditModel
            {
                Name = Name,
                Reference = Reference,
                Description = Description,
                Condition = SelectedCondition,
                CategoryId = SelectedCategory!.Id
            };

            bool success;
            if (IsEditMode && Id.HasValue)
            {
                success = await _equipmentService.UpdateAsync(Id.Value, editModel);
            }
            else
            {
                success = await _equipmentService.CreateAsync(editModel);
            }

            if (success)
            {
                SuccessMessage = "Enregistré avec succès ! Retour à la liste...";
                await Task.Delay(1000); // Give user a moment to see the message
                if (NavigateToListRequested != null) await NavigateToListRequested.Invoke(true);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur d'enregistrement : " + ex.Message;
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Le nom est obligatoire.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Reference))
        {
            ErrorMessage = "La référence (SKU) est obligatoire.";
            return false;
        }
        if (SelectedCategory == null)
        {
            ErrorMessage = "Veuillez sélectionner une catégorie.";
            return false;
        }
        if (Quantity < 0)
        {
            ErrorMessage = "La quantité ne peut pas être négative.";
            return false;
        }
        return true;
    }

    private async Task CancelAsync()
    {
        if (NavigateToListRequested != null) await NavigateToListRequested.Invoke(false);
    }
}
