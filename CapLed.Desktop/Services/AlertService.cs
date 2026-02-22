using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

public class AlertService : ApiClientBase
{
    public AlertService(HttpClient http) : base(http)
    {
    }

    /// <summary>
    /// Fetches all equipments currently under their minimum threshold.
    /// </summary>
    public async Task<List<AlertModel>> GetLowStockAlertsAsync()
    {
        var response = await Http.GetAsync("api/v1/alerts/low-stock");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<AlertModel>>(JsonOptions) ?? new();
        }
        
        var error = await HandleErrorResponse(response, "GET alerts/low-stock");
        throw new Exception(error);
    }

    /// <summary>
    /// Placeholder for future functionality to acknowledge alerts.
    /// Backend support needed for persistence.
    /// </summary>
    public async Task<bool> UpdateAlertStatusAsync(int id, string status)
    {
        // For now, this is a placeholder as the backend currently computes alerts on-the-fly
        // without a dedicated 'Alert' entity in the database.
        return await Task.FromResult(true); 
    }
}
