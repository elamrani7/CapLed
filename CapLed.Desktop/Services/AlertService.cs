using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

/// <summary>
/// Wraps Alerts API calls.
/// Endpoint: GET api/v1/Alerts/low-stock
/// </summary>
public class AlertService : ApiClientBase
{
    public AlertService(HttpClient httpClient) : base(httpClient) { }

    /// <summary>GET api/v1/Alerts/low-stock — retrieve all low-stock alerts.</summary>
    public async Task<List<AlertModel>> GetLowStockAlertsAsync()
    {
        var result = await GetAsync<List<AlertModel>>("api/v1/Alerts/low-stock");
        return result ?? new List<AlertModel>();
    }
}
