using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Core.Application.Services;

/// <summary>
/// LeadService — gère le pipeline de demandes de devis commerciaux.
/// Implémente RG-07 (numérotation automatique DEVIS-AAAA-NNNN).
/// </summary>
public class LeadService : ILeadService
{
    private readonly ILeadRepository   _leadRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IUnitOfWork       _uow;

    public LeadService(ILeadRepository leadRepo, IClientRepository clientRepo, IUnitOfWork uow)
    {
        _leadRepo   = leadRepo;
        _clientRepo = clientRepo;
        _uow        = uow;
    }

    // ── RG-07: Create Lead with auto-numbered devis ───────────────────────────

    public async Task<Lead> CreateLeadAsync(CreateLeadDto dto)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            // Upsert client: match on email
            var client = await _clientRepo.GetByEmailAsync(dto.EmailClient);
            if (client == null)
            {
                client = new Client
                {
                    Nom       = dto.NomClient,
                    Prenom    = dto.PrenomClient,
                    Email     = dto.EmailClient,
                    Telephone = dto.Telephone,
                    Societe   = dto.Societe,
                    CreatedAt = DateTime.UtcNow
                };
                await _clientRepo.AddAsync(client);
                await _uow.SaveChangesAsync(); // need client.Id before creating Lead
            }

            // RG-07: Generate unique numero DEVIS-AAAA-NNNN
            int year  = DateTime.UtcNow.Year;
            int count = await _leadRepo.CountByYearAsync(year);
            string numero = $"DEVIS-{year}-{(count + 1):D4}";

            var lead = new Lead
            {
                ClientId          = client.Id,
                NumeroDevis       = numero,
                Statut            = "NOUVEAU",
                DateSoumission    = DateTime.UtcNow,
                Commentaire       = dto.Commentaire,
                SourceAcquisition = dto.SourceAcquisition,
                Lignes = dto.Lignes.Select(l => new LigneLead
                {
                    ArticleId        = l.ArticleId,
                    QuantiteDemandee = l.QuantiteDemandee,
                    Commentaire      = l.Commentaire
                }).ToList()
            };

            await _leadRepo.AddAsync(lead);
            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();

            // Re-fetch fully loaded
            return (await _leadRepo.GetByIdAsync(lead.Id))!;
        }
        catch
        {
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }

    // ── Workflow: Update statut ───────────────────────────────────────────────

    public async Task<Lead> UpdateStatutAsync(int leadId, UpdateLeadStatutDto dto)
    {
        var lead = await _leadRepo.GetByIdAsync(leadId)
            ?? throw new Exception($"Lead {leadId} introuvable.");

        // Validate transition
        ValidateStatutTransition(lead.Statut, dto.Statut);

        lead.Statut = dto.Statut;
        if (dto.CommercialId.HasValue) lead.CommercialId = dto.CommercialId;
        if (dto.Commentaire != null)   lead.Commentaire  = dto.Commentaire;

        if (lead.DateTraitement == null && dto.Statut != "NOUVEAU")
            lead.DateTraitement = DateTime.UtcNow;

        await _leadRepo.UpdateAsync(lead);
        await _uow.SaveChangesAsync();

        return lead;
    }

    public Task<Lead?>  GetByIdAsync(int id)         => _leadRepo.GetByIdAsync(id);
    public Task<List<Lead>> GetAllAsync()            => _leadRepo.GetAllAsync();
    public Task<List<Lead>> GetByStatutAsync(string s) => _leadRepo.GetByStatutAsync(s);

    // ── Private helpers ───────────────────────────────────────────────────────

    private static void ValidateStatutTransition(string current, string next)
    {
        var allowed = new Dictionary<string, string[]>
        {
            ["NOUVEAU"]      = ["EN_COURS", "REFUSE"],
            ["EN_COURS"]     = ["DEVIS_ENVOYE", "REFUSE"],
            ["DEVIS_ENVOYE"] = ["ACCEPTE", "REFUSE"],
            ["ACCEPTE"]      = [],
            ["REFUSE"]       = []
        };

        if (!allowed.TryGetValue(current, out var nexts) || !nexts.Contains(next))
            throw new Exception($"Transition de statut invalide: {current} → {next}.");
    }
}
