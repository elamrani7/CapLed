using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CapLed.Desktop.Models;
using CapLed.Desktop.Services;
using CapLed.Desktop.ViewModels.Base;

namespace CapLed.Desktop.ViewModels;

public class AlertsViewModel : BaseViewModel
{
    private readonly AlertService _alertService;

    public ObservableCollection<AlertModel> Alerts { get; } = new();

    private AlertModel? _selectedAlert;
    public AlertModel? SelectedAlert
    {
        get => _selectedAlert;
        set => SetProperty(ref _selectedAlert, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand AcknowledgeCommand { get; }

    public AlertsViewModel(AlertService alertService)
    {
        _alertService = alertService;

        RefreshCommand = new AsyncRelayCommand(LoadAlertsAsync);
        AcknowledgeCommand = new AsyncRelayCommand(AcknowledgeAlertAsync, () => SelectedAlert != null && !IsSaving);
    }

    public async Task InitializeAsync()
    {
        await LoadAlertsAsync();
    }

    private async Task LoadAlertsAsync()
    {
        BeginOperation();
        try
        {
            var result = await _alertService.GetLowStockAlertsAsync();
            Alerts.Clear();
            foreach (var alert in result)
            {
                Alerts.Add(alert);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du chargement des alertes : " + ex.Message;
        }
        finally
        {
            EndOperation();
        }
    }

    private async Task AcknowledgeAlertAsync()
    {
        if (SelectedAlert == null) return;

        BeginSave();

        try
        {
            // Note: Backend currently doesn't persist 'Acknowledge' status for fly-calculated alerts
            bool success = await _alertService.UpdateAlertStatusAsync(SelectedAlert.Id, "Acknowledged");
            if (success)
            {
                SuccessMessage = $"Alerte pour {SelectedAlert.EquipmentName} traitée (Simulation).";
                await LoadAlertsAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du traitement de l'alerte : " + ex.Message;
        }
        finally
        {
            EndSave();
        }
    }
}
