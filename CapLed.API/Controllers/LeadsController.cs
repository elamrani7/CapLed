using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Exceptions;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/leads")]
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly IMapper      _mapper;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(ILeadService leadService, IMapper mapper, ILogger<LeadsController> logger)
    {
        _leadService = leadService;
        _mapper      = mapper;
        _logger      = logger;
    }

    /// <summary>Liste tous les leads (tous statuts).</summary>
    [HttpGet]
    public async Task<ActionResult<List<LeadReadDto>>> GetAll()
        => Ok(_mapper.Map<List<LeadReadDto>>(await _leadService.GetAllAsync()));

    /// <summary>Liste des leads filtrés par statut.</summary>
    [HttpGet("statut/{statut}")]
    public async Task<ActionResult<List<LeadReadDto>>> GetByStatut(string statut)
        => Ok(_mapper.Map<List<LeadReadDto>>(await _leadService.GetByStatutAsync(statut)));

    /// <summary>Détail d'un lead par ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeadReadDto>> GetById(int id)
    {
        var lead = await _leadService.GetByIdAsync(id);
        if (lead == null) throw new NotFoundException("LEAD_NOT_FOUND", $"Le lead {id} est introuvable.");
        return Ok(_mapper.Map<LeadReadDto>(lead));
    }

    /// <summary>
    /// Crée un nouveau lead / demande de devis.
    /// Génère automatiquement le numéro DEVIS-AAAA-NNNN (RG-07).
    /// Upserte le client par email.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LeadReadDto>> Create(CreateLeadDto dto)
    {
        _logger.LogInformation("Lead submission: Client={Client}, Source={Source}, Lignes={Count}",
            dto.EmailClient, dto.SourceAcquisition, dto.Lignes?.Count ?? 0);

        var lead = await _leadService.CreateLeadAsync(dto);
        _logger.LogInformation("Lead created: ID={Id}, Numero={Numero}", lead.Id, lead.NumeroDevis);
        return CreatedAtAction(nameof(GetById), new { id = lead.Id }, _mapper.Map<LeadReadDto>(lead));
    }

    /// <summary>
    /// Met à jour le statut d'un lead (workflow: NOUVEAU → EN_COURS → DEVIS_ENVOYE → ACCEPTE/REFUSE).
    /// </summary>
    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "ADMIN,COMMERCIAL")]
    public async Task<ActionResult<LeadReadDto>> UpdateStatut(int id, UpdateLeadStatutDto dto)
    {
        var lead = await _leadService.UpdateStatutAsync(id, dto);
        return Ok(_mapper.Map<LeadReadDto>(lead));
    }
}
