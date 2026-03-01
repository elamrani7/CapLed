using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

/// <summary>
/// Wraps Stock movement API calls.
/// Endpoints:
///   POST api/v1/Stock/entry
///   POST api/v1/Stock/exit
///   GET  api/v1/Stock/level/{equipmentId}
///   GET  api/v1/Stock/history/{equipmentId}
///   GET  api/v1/Stock/alerts/low-stock
/// </summary>
public class StockService : ApiClientBase
{
    public StockService(HttpClient httpClient) : base(httpClient) { }

    /// <summary>POST api/v1/Stock/entry — record a stock entry (restock).</summary>
    public async Task<StockMovementModel?> RecordEntryAsync(StockMovementCreateModel model)
    {
        // Force type to ENTRY
        model.Type = "ENTRY";
        return await PostAsync<StockMovementCreateModel, StockMovementModel>("api/v1/Stock/entry", model);
    }

    /// <summary>POST api/v1/Stock/exit — record a stock exit (dispatch).</summary>
    public async Task<StockMovementModel?> RecordExitAsync(StockMovementCreateModel model)
    {
        model.Type = "EXIT";
        return await PostAsync<StockMovementCreateModel, StockMovementModel>("api/v1/Stock/exit", model);
    }

    /// <summary>GET api/v1/Stock/level/{equipmentId} — current stock quantity.</summary>
    public async Task<int> GetStockLevelAsync(int equipmentId)
    {
        var result = await GetAsync<int>($"api/v1/Stock/level/{equipmentId}");
        return result;
    }

    /// <summary>GET api/v1/Stock/history/{equipmentId} — movement history for one equipment.</summary>
    public async Task<List<StockMovementModel>> GetHistoryAsync(int equipmentId)
    {
        var result = await GetAsync<List<StockMovementModel>>($"api/v1/Stock/history/{equipmentId}");
        return result ?? new List<StockMovementModel>();
    }

    /// <summary>GET api/v1/Stock/alerts/low-stock — all equipment below threshold.</summary>
    public async Task<List<AlertModel>> GetLowStockAlertsAsync()
    {
        var result = await GetAsync<List<AlertModel>>("api/v1/Stock/alerts/low-stock");
        return result ?? new List<AlertModel>();
    }

    /// <summary>
    /// GET api/v1/Stock/history — movements with filters and pagination.
    /// </summary>
    public async Task<PagedResult<StockMovementModel>> GetFilteredHistoryAsync(StockMovementFilter filter)
    {
        var query = BuildQuery(
            ("equipmentId", filter.EquipmentId?.ToString()),
            ("type", filter.Type),
            ("dateFrom", filter.DateFrom?.ToString("o")), // ISO 8601
            ("dateTo", filter.DateTo?.ToString("o")),
            ("page", filter.Page.ToString()),
            ("pageSize", filter.PageSize.ToString())
        );

        var result = await GetAsync<PagedResult<StockMovementModel>>($"api/v1/Stock/history{query}");
        return result ?? new PagedResult<StockMovementModel>();
    }

    /// <summary>PUT api/v1/Stock/{id} — update a movement.</summary>
    public async Task<bool> UpdateAsync(int id, StockMovementCreateModel model)
    {
        return await PutAsync<StockMovementCreateModel>($"api/v1/Stock/{id}", model);
    }

    /// <summary>DELETE api/v1/Stock/{id} — delete a movement.</summary>
    public async Task<bool> DeleteAsync(int id)
    {
        return await DeleteAsync($"api/v1/Stock/{id}");
    }

    // ─── Helper ──────────────────────────────────────────────────────────────
    private static string BuildQuery(params (string Key, string? Value)[] parameters)
    {
        var parts = parameters
            .Where(p => p.Value != null)
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}");
        var qs = string.Join("&", parts);
        return qs.Length > 0 ? "?" + qs : string.Empty;
    }
}
