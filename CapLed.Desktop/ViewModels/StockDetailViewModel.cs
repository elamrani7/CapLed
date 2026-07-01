using System.Collections.ObjectModel;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class StockDetailViewModel : BaseViewModel
{
    private readonly StockDetailService _detailService;

    // ─── Article Info ────────────────────────────────────────────────
    private string _articleName = string.Empty;
    public string ArticleName { get => _articleName; set => SetProperty(ref _articleName, value); }

    private string _typeGestion = "QUANTITE";
    public string TypeGestion { get => _typeGestion; set => SetProperty(ref _typeGestion, value); }

    private int _totalQuantite;
    public int TotalQuantite { get => _totalQuantite; set => SetProperty(ref _totalQuantite, value); }

    // ─── Mode flags ──────────────────────────────────────────────────
    public bool IsLotMode      => TypeGestion == "LOT";
    public bool IsSerieMode    => TypeGestion == "SERIALISE";
    public bool IsQuantiteMode => TypeGestion == "QUANTITE" || (!IsLotMode && !IsSerieMode);

    // ─── Data collections ────────────────────────────────────────────
    public ObservableCollection<LotDetailModel>   Lots   { get; } = new();
    public ObservableCollection<SerieDetailModel> Series { get; } = new();

    // ─── State ───────────────────────────────────────────────────────

    private bool _isVisible;
    public bool IsVisible { get => _isVisible; set => SetProperty(ref _isVisible, value); }

    // ─── Commands ────────────────────────────────────────────────────
    public ICommand CloseCommand { get; }

    public StockDetailViewModel(StockDetailService detailService)
    {
        _detailService = detailService;
        CloseCommand   = new RelayCommand(_ => IsVisible = false);
    }

    /// <summary>Charge le détail du stock pour l'article sélectionné.</summary>
    public async Task LoadAsync(int articleId)
    {
        IsLoading = true;
        IsVisible = true;
        Lots.Clear();
        Series.Clear();

        try
        {
            var detail = await _detailService.GetDetailAsync(articleId);
            if (detail == null) return;

            ArticleName   = detail.ArticleName;
            TypeGestion   = detail.TypeGestion;
            TotalQuantite = detail.TotalQuantite;

            // Notify mode flags
            OnPropertyChanged(nameof(IsLotMode));
            OnPropertyChanged(nameof(IsSerieMode));
            OnPropertyChanged(nameof(IsQuantiteMode));

            if (detail.Lots != null)
                foreach (var lot in detail.Lots)
                    Lots.Add(lot);

            if (detail.Series != null)
                foreach (var s in detail.Series)
                    Series.Add(s);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur chargement détail : " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
