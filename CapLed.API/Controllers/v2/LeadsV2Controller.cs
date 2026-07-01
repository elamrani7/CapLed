using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.API.Controllers.V2;

/// <summary>
/// v2/leads — Paginated Lead endpoint consumed by the WPF desktop CRM.
/// Maps Lead domain data to the shape expected by WPF's LeadModel / LigneLeadModel.
/// </summary>
[ApiController]
[Route("v2/leads")]
[Authorize]
public class LeadsV2Controller : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly IMapper      _mapper;

    public LeadsV2Controller(ILeadService leadService, IMapper mapper)
    {
        _leadService = leadService;
        _mapper      = mapper;
    }

    /// <summary>Paginated list of leads for WPF CRM.</summary>
    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        [FromQuery] string? search = null,
        [FromQuery] string? statut = null)
    {
        List<Lead> leads;

        if (!string.IsNullOrEmpty(statut))
            leads = await _leadService.GetByStatutAsync(statut);
        else
            leads = await _leadService.GetAllAsync();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(search))
        {
            var s = search.ToLowerInvariant();
            leads = leads.Where(l =>
                (l.Client?.Nom ?? "").ToLower().Contains(s) ||
                (l.Client?.Email ?? "").ToLower().Contains(s) ||
                (l.NumeroDevis ?? "").ToLower().Contains(s)
            ).ToList();
        }

        var totalCount = leads.Count;

        // Apply pagination
        var pagedLeads = leads
            .OrderByDescending(l => l.DateSoumission)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Map to shape expected by WPF LeadModel
        var items = pagedLeads.Select(l => new
        {
            l.Id,
            l.Statut,
            NomContact       = l.Client != null ? $"{l.Client.Nom} {l.Client.Prenom}".Trim() : "Inconnu",
            EmailContact     = l.Client?.Email ?? "",
            TelephoneContact = l.Client?.Telephone,
            MessageClient    = l.MessageClient ?? l.Commentaire,
            DateCreation     = l.DateSoumission,
            Client = l.Client != null ? new
            {
                l.Client.Id,
                RaisonSociale = l.Client.Societe ?? l.Client.Nom,
                l.Client.Email,
                l.Client.Telephone,
                Siret = (string?)null
            } : null,
            CommercialNom = (string?)null,
            Lignes = l.Lignes?.Select(li => new
            {
                li.Id,
                li.ArticleId,
                ArticleName      = li.Article?.Name ?? "Article",
                ArticleReference = li.Article?.Reference ?? "-",
                Quantite         = li.QuantiteDemandee,
                PrixUnitairePropose = (decimal?)null
            }).ToList()
        }).ToList();

        return Ok(new
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    /// <summary>Update lead status (PUT for WPF compatibility).</summary>
    [HttpPut("{id:int}/statut")]
    [Authorize(Roles = "ADMIN,COMMERCIAL")]
    public async Task<ActionResult> UpdateStatut(int id, [FromBody] UpdateLeadStatutDto dto)
    {
        try
        {
            var lead = await _leadService.UpdateStatutAsync(id, dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
