using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

/// <summary>
/// Wraps all Equipment-related API calls.
/// Endpoints: GET/POST/PUT/DELETE api/v1/Equipment
/// </summary>
public class EquipmentService : ApiClientBase
{
    public EquipmentService(HttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// GET api/v1/Equipment?categoryId=&condition=&search=&page=&pageSize=
    /// </summary>
    public async Task<PagedResult<EquipmentListItemModel>> GetAllAsync(
        int? familleId = null,
        int? categoryId = null,
        string? condition = null,
        string? search = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = BuildQuery(
            ("familleId", familleId?.ToString()),
            ("categoryId", categoryId?.ToString()),
            ("condition", condition),
            ("search", search),
            ("page", page.ToString()),
            ("pageSize", pageSize.ToString())
        );

        var result = await GetAsync<PagedResult<EquipmentListItemModel>>($"api/v1/Equipment{query}");
        return result ?? new PagedResult<EquipmentListItemModel>();
    }

    /// <summary>GET api/v1/Equipment/{id}</summary>
    public Task<EquipmentDetailModel?> GetByIdAsync(int id)
        => GetAsync<EquipmentDetailModel>($"api/v1/Equipment/{id}");

    /// <summary>POST api/v1/Equipment</summary>
    public async Task<bool> CreateAsync(EquipmentEditModel model)
    {
        var result = await PostAsync<EquipmentEditModel, EquipmentDetailModel>("api/v1/Equipment", model);
        return result != null;
    }

    /// <summary>PUT api/v1/Equipment/{id}</summary>
    public Task<bool> UpdateAsync(int id, EquipmentEditModel model)
        => PutAsync($"api/v1/Equipment/{id}", model);

    /// <summary>DELETE api/v1/Equipment/{id}</summary>
    public Task<bool> DeleteAsync(int id)
        => DeleteAsync($"api/v1/Equipment/{id}");

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
