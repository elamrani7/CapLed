using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

public class CrmApiClient : ApiClientBase
{
    public CrmApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public async Task<PagedResult<LeadModel>> GetLeadsAsync(int page, int pageSize, string? search = null, string? statut = null)
    {
        var url = $"v2/leads?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(search)) url += $"&search={search}";
        if (!string.IsNullOrEmpty(statut)) url += $"&statut={statut}";

        // Appel silencieux : 401/403 = session non active, pas de popup
        var result = await GetAsyncSilent<PagedResult<LeadModel>>(url);
        return result ?? new PagedResult<LeadModel>();
    }

    public async Task UpdateLeadStatusAsync(int leadId, string statut)
    {
        await PutAsync($"v2/leads/{leadId}/statut", new UpdateLeadStatusModel { Statut = statut });
    }

    /// <summary>Crée un BC depuis un Lead ACCEPTE via le workflow ERP.</summary>
    public async Task<BonCommandeModel?> CreateBonCommandeFromLeadAsync(int leadId)
    {
        return await PostAsync<object, BonCommandeModel>($"api/Orders/bc/from-lead/{leadId}", new { });
    }
}
