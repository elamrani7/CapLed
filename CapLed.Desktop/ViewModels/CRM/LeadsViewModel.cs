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
        "DEVIS_ENVOYE",
        "ACCEPTE",
        "REFUSE"
    };

    public ICommand SearchCommand { get; }
    public ICommand UpdateStatusCommand { get; }

    public LeadsViewModel(CrmApiClient crmApiClient, AuthService authService, IConfirmationService confirmation)
    {
        _crmApiClient = crmApiClient;
        _authService = authService;
        _confirmation = confirmation;

        SelectedStatus = "Tous";

        SearchCommand = new AsyncRelayCommand(LoadLeadsAsync);
        UpdateStatusCommand = new AsyncRelayCommand(async param => 
            await UpdateStatusAsync((LeadModel?)param));
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

    private async Task UpdateStatusAsync(LeadModel? lead)
    {
        if (lead == null) return;

        // Simplify for demonstration: just switch status in a cycle or to EN_COURS
        var nextStatus = lead.Statut == "NOUVEAU" ? "EN_COURS" :
                         lead.Statut == "EN_COURS" ? "DEVIS_ENVOYE" :
                         lead.Statut == "DEVIS_ENVOYE" ? "ACCEPTE" : lead.Statut;

        if (nextStatus == lead.Statut) return;

        try
        {
            await _crmApiClient.UpdateLeadStatusAsync(lead.Id, nextStatus);
            lead.Statut = nextStatus;
            
            // Force UI update
            var index = Leads.IndexOf(lead);
            if (index >= 0)
            {
                Leads[index] = lead;
            }
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur réseau", ex.Message);
        }
    }
}
