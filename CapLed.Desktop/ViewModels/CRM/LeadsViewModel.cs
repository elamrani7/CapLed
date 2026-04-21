using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CapLed.Desktop.Core;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels.CRM;

public class LeadsViewModel : BaseViewModel
{
    private readonly CrmApiClient _crmApiClient;
    private readonly AuthService _authService;
    private readonly IConfirmationService _confirmation;

    public ObservableCollection<LeadModel> Leads { get; } = new();


    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set { _searchQuery = value; OnPropertyChanged(); }
    }

    private string _selectedStatus = string.Empty;
    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
            _ = LoadLeadsAsync();
        }
    }

    public ObservableCollection<string> StatusList { get; } = new()
    {
        "Tous",
        "NOUVEAU",
        "EN_COURS",
        "ACCEPTE",
        "REFUSE"
    };

    public ICommand SearchCommand { get; }
    public ICommand PrendreEnChargeCommand { get; }
    public ICommand AccepterDevisCommand { get; }
    public ICommand RefuserDevisCommand { get; }
    public ICommand CreerCommandeCommand { get; }

    public LeadsViewModel(CrmApiClient crmApiClient, AuthService authService, IConfirmationService confirmation)
    {
        _crmApiClient = crmApiClient;
        _authService = authService;
        _confirmation = confirmation;

        SelectedStatus = "Tous";

        SearchCommand = new AsyncRelayCommand(LoadLeadsAsync);
        PrendreEnChargeCommand = new AsyncRelayCommand(async param => await BaseUpdateStatusAsync((LeadModel?)param, "EN_COURS"));
        AccepterDevisCommand = new AsyncRelayCommand(async param => await AskAndSetStatusAsync((LeadModel?)param, "ACCEPTE", "Voulez-vous accepter ce devis ? (Action irréversible)"));
        RefuserDevisCommand = new AsyncRelayCommand(async param => await AskAndSetStatusAsync((LeadModel?)param, "REFUSE", "Voulez-vous refuser ce devis ? (Action irréversible)"));
        
        CreerCommandeCommand = new RelayCommand(param => 
            _confirmation.ShowError("Simulation ERP", "Le composant 'Créer Bon de Commande' n'est pas encore implémenté (Phase Documents ERP).")
        );
    }

    public async Task LoadLeadsAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            Leads.Clear();
            var statutFilter = SelectedStatus == "Tous" ? null : SelectedStatus;
            var result = await _crmApiClient.GetLeadsAsync(1, 100, SearchQuery, statutFilter);
            
            if (result?.Items != null)
            {
                foreach (var lead in result.Items)
                {
                    Leads.Add(lead);
                }
            }
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur de chargement", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AskAndSetStatusAsync(LeadModel? lead, string targetStatus, string question)
    {
        if (lead == null) return;
        
        if (_confirmation.Confirm("Confirmation", question))
        {
            await BaseUpdateStatusAsync(lead, targetStatus);
        }
    }

    private async Task BaseUpdateStatusAsync(LeadModel? lead, string targetStatus)
    {
        if (lead == null || targetStatus == lead.Statut) return;

        try
        {
            await _crmApiClient.UpdateLeadStatusAsync(lead.Id, targetStatus);
            lead.Statut = targetStatus;
            
            // Force UI update
            var index = Leads.IndexOf(lead);
            if (index >= 0)
            {
                // Assign a completely new instance or re-assign to trigger notification on ObservableCollection indexer
                Leads[index] = lead;
            }
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur réseau", ex.Message);
        }
    }
}
