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
    
    private bool _visibleSite;
    public bool VisibleSite
    {
        get => _visibleSite;
        set => SetProperty(ref _visibleSite, value);
    }

    private bool _isPublished;
    public bool IsPublished
    {
        get => _isPublished;
        set => SetProperty(ref _isPublished, value);
    }

    private decimal? _prixVente;
    public decimal? PrixVente
    {
        get => _prixVente;
        set => SetProperty(ref _prixVente, value);
    }

    private ArticleEtatDetailModel _etatDetail = new();
    public ArticleEtatDetailModel EtatDetail
    {
        get => _etatDetail;
        set => SetProperty(ref _etatDetail, value);
    }

    public ObservableCollection<ArticleChampValeurModel> ChampsSpecifiques { get; } = new();
    public ObservableCollection<PhotoModel> Photos { get; } = new();

    private CategoryModel? _selectedCategory;
    public CategoryModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public ObservableCollection<CategoryModel> Categories { get; } = new();
    public List<string> ConditionOptions { get; } = new() { "NEUF", "OCCASION", "RECONDITIONNE" };

    public string Title => IsEditMode ? "Modifier l'Équipement" : "Nouvel Équipement";

    // ─── Commands ────────────────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ICommand UploadImagesCommand { get; }
    public ICommand DeleteImageCommand { get; }
    public ICommand SetPrimaryImageCommand { get; }

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

        UploadImagesCommand = new AsyncRelayCommand(UploadImagesActionAsync, () => IsEditMode && !IsSaving);
        DeleteImageCommand = new AsyncRelayCommand(DeleteImageActionAsync, (_) => !IsSaving);
        SetPrimaryImageCommand = new AsyncRelayCommand(SetPrimaryImageActionAsync, (_) => !IsSaving);
    }

    /// <summary>
    /// Initializes the ViewModel for Create or Edit mode.
    /// </summary>
    public async Task LoadAsync(int? id = null)
    {
        BeginOperation();
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
                    
                    // Case-insensitive match for the condition string from API
                    var match = ConditionOptions.FirstOrDefault(o => o.Equals(equipment.Condition, StringComparison.OrdinalIgnoreCase));
                    SelectedCondition = match ?? "NEUF";

                    Quantity = equipment.Quantity;
                    VisibleSite = equipment.VisibleSite;
                    IsPublished = equipment.IsPublished;
                    PrixVente = equipment.PrixVente;
                    EtatDetail = equipment.EtatDetail ?? new ArticleEtatDetailModel();
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == equipment.CategoryId);

                    ChampsSpecifiques.Clear();
                    if (equipment.ChampsSpecifiques != null)
                    {
                        foreach(var champ in equipment.ChampsSpecifiques)
                            ChampsSpecifiques.Add(champ);
                    }

                    Photos.Clear();
                    if (equipment.Photos != null)
                    {
                        foreach(var p in equipment.Photos) Photos.Add(p);
                    }
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
                SelectedCondition = "NEUF";
                Quantity = 0;
                VisibleSite = false;
                IsPublished = false;
                PrixVente = null;
                EtatDetail = new ArticleEtatDetailModel();
                ChampsSpecifiques.Clear();
                SelectedCategory = null;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur de chargement : " + ex.Message;
        }
        finally
        {
            EndOperation();
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

        BeginSave();

        try
        {
            var editModel = new EquipmentEditModel
            {
                Name = Name,
                Reference = Reference,
                Description = Description,
                Condition = SelectedCondition,
                CategoryId = SelectedCategory!.Id,
                VisibleSite = VisibleSite,
                IsPublished = IsPublished,
                PrixVente = PrixVente,
                MinThreshold = 2, // Default or add a field
                EtatDetail = EtatDetail,
                ChampsSpecifiques = ChampsSpecifiques.ToList()
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
            EndSave();
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

    // ─── Image Management ───────────────────────────────────────────────────

    private async Task UploadImagesActionAsync()
    {
        if (!Id.HasValue) return;

        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Multiselect = true,
            Filter = "Images (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp",
            Title = "Sélectionner des images"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            BeginSave();
            try
            {
                var updatedEquipment = await _equipmentService.UploadImagesAsync(Id.Value, openFileDialog.FileNames);
                RefreshPhotos(updatedEquipment.Photos);
                SuccessMessage = "Images téléversées avec succès.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de l'upload : " + ex.Message;
            }
            finally
            {
                EndSave();
            }
        }
    }

    private async Task DeleteImageActionAsync(object? parameter)
    {
        if (!Id.HasValue || parameter is not int photoId) return;
        
        BeginSave();
        try
        {
            var success = await _equipmentService.DeleteImageAsync(Id.Value, photoId);
            if (success)
            {
                var photoToRemove = Photos.FirstOrDefault(p => p.Id == photoId);
                if (photoToRemove != null) Photos.Remove(photoToRemove);
                SuccessMessage = "Image supprimée.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur de suppression : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }

    private async Task SetPrimaryImageActionAsync(object? parameter)
    {
        if (!Id.HasValue || parameter is not int photoId) return;
        
        BeginSave();
        try
        {
            var success = await _equipmentService.SetPrimaryImageAsync(Id.Value, photoId);
            if (success)
            {
                foreach(var p in Photos) p.IsPrimary = (p.Id == photoId);
                SuccessMessage = "Image principale mise à jour.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }

    private void RefreshPhotos(IEnumerable<PhotoModel>? updatedPhotos)
    {
        Photos.Clear();
        if (updatedPhotos != null)
        {
            foreach (var p in updatedPhotos) Photos.Add(p);
        }
    }
}
