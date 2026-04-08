using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Services;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/leads")]
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly IMapper      _mapper;

    public LeadsController(ILeadService leadService, IMapper mapper)
    {
        _leadService = leadService;
        _mapper      = mapper;
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
        return lead == null ? NotFound() : Ok(_mapper.Map<LeadReadDto>(lead));
    }

    /// <summary>
    /// Crée un nouveau lead / demande de devis.
    /// Génère automatiquement le numéro DEVIS-AAAA-NNNN (RG-07).
    /// Upserte le client par email.
    /// </summary>
    [HttpPost]
    [AllowAnonymous] // accessible depuis le site vitrine sans authentification
    public async Task<ActionResult<LeadReadDto>> Create(CreateLeadDto dto)
    {
        try
        {
            var lead = await _leadService.CreateLeadAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = lead.Id }, _mapper.Map<LeadReadDto>(lead));
        }
        catch (Exception ex)
        {
            return BadRequest(new { errors = new { Error = new[] { ex.Message } } });
        }
    }

    /// <summary>
    /// Met à jour le statut d'un lead (workflow: NOUVEAU → EN_COURS → DEVIS_ENVOYE → ACCEPTE/REFUSE).
    /// </summary>
    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "ADMIN,COMMERCIAL")]
    public async Task<ActionResult<LeadReadDto>> UpdateStatut(int id, UpdateLeadStatutDto dto)
    {
        try
        {
            var lead = await _leadService.UpdateStatutAsync(id, dto);
            return Ok(_mapper.Map<LeadReadDto>(lead));
        }
        catch (Exception ex)
        {
            return BadRequest(new { errors = new { Error = new[] { ex.Message } } });
        }
    }
}
