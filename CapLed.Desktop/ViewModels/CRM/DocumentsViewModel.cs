using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CapLed.Desktop.Core;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels.CRM;

public class DocumentsViewModel : BaseViewModel
{
    private readonly DocumentApiClient _documentApiClient;
    private readonly IConfirmationService _confirmation;

    public ObservableCollection<BonCommandeModel> BonsCommande { get; } = new();
    public ObservableCollection<BonLivraisonModel> BonsLivraison { get; } = new();



    public ICommand LoadDocumentsCommand { get; }
    public ICommand DownloadBcPdfCommand { get; }
    public ICommand DownloadBlPdfCommand { get; }
    public ICommand DeleteBcCommand { get; }

    public DocumentsViewModel(DocumentApiClient documentApiClient, IConfirmationService confirmation)
    {
        _documentApiClient = documentApiClient;
        _confirmation = confirmation;

        LoadDocumentsCommand = new AsyncRelayCommand(LoadDataAsync);
        DownloadBcPdfCommand = new AsyncRelayCommand(async param => await DownloadBcAsync((BonCommandeModel?)param));
        DownloadBlPdfCommand = new AsyncRelayCommand(async param => await DownloadBlAsync((BonLivraisonModel?)param));
        DeleteBcCommand = new AsyncRelayCommand(async param => await DeleteBcAsync((BonCommandeModel?)param));
    }

    private async Task DeleteBcAsync(BonCommandeModel? bc)
    {
        if (bc == null) return;
        
        if (bc.Statut != "EN_ATTENTE" && bc.Statut != "CREE")
        {
            _confirmation.ShowError("Action impossible", "Seuls les Bons de Commande en attente peuvent être supprimés.");
            return;
        }

        if (!_confirmation.Confirm("Confirmation de suppression", 
            $"Voulez-vous vraiment supprimer le Bon de Commande {bc.Numero} ?\n\nLe devis associé redeviendra libre."))
            return;

        try
        {
            await _documentApiClient.DeleteBonCommandeAsync(bc.Id);
            _confirmation.ShowInfo("Succès", "Le Bon de Commande a été supprimé avec succès.");
            await LoadDataAsync();
        }
        catch (ApiException ex)
        {
            _confirmation.ShowError("Erreur métier", ex.Message);
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur réseau", ex.Message);
        }
    }

    public async Task LoadDataAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            BonsCommande.Clear();
            var bcResult = await _documentApiClient.GetBonsCommandeAsync(1, 100);
            if (bcResult?.Items != null)
            {
                foreach (var bc in bcResult.Items) BonsCommande.Add(bc);
            }

            BonsLivraison.Clear();
            var blResult = await _documentApiClient.GetBonsLivraisonAsync(1, 100);
            if (blResult?.Items != null)
            {
                foreach (var bl in blResult.Items) BonsLivraison.Add(bl);
            }
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DownloadBcAsync(BonCommandeModel? bc)
    {
        if (bc == null) return;
        try
        {
            var bytes = await _documentApiClient.DownloadBcPdfAsync(bc.Id);
            SavePdfAndOpen($"BC_{bc.Numero}.pdf", bytes);
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur de téléchargement", ex.Message);
        }
    }

    private async Task DownloadBlAsync(BonLivraisonModel? bl)
    {
        if (bl == null) return;
        try
        {
            var bytes = await _documentApiClient.DownloadBlPdfAsync(bl.Id);
            SavePdfAndOpen($"BL_{bl.Numero}.pdf", bytes);
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur de téléchargement", ex.Message);
        }
    }

    private void SavePdfAndOpen(string filename, byte[] data)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), filename);
            File.WriteAllBytes(tempPath, data);
            
            // Open the generated PDF (windows)
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _confirmation.ShowError("Erreur d'ouverture", "Impossible d'ouvrir le fichier PDF : " + ex.Message);
        }
    }
}
