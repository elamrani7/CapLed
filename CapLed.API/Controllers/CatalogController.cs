using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IContactRequestRepository _contactRepository;
    private readonly IMapper _mapper;

    public CatalogController(
        IEquipmentRepository equipmentRepository, 
        IContactRequestRepository contactRepository,
        IMapper mapper)
    {
        _equipmentRepository = equipmentRepository;
        _contactRepository = contactRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Search and filter catalog items with pagination.
    /// Used by React Front-office.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EquipmentCatalogItemDto>>> GetCatalog(
        [FromQuery] int? familleId,
        [FromQuery] int? categoryId, 
        [FromQuery] EquipmentCondition? condition,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        // Public catalog only shows Published AND Visible items
        var (entities, totalCount) = await _equipmentRepository.GetAllAsync(
            familleId, categoryId, condition, isPublished: true, search, page, pageSize);
        
        // Ensure we filter for VisibleSite in memory if repo doesn't do it yet
        var filteredEntities = entities.Where(e => e.VisibleSite).ToList();
        
        var dtos = _mapper.Map<IEnumerable<EquipmentCatalogItemDto>>(filteredEntities);
        
        return Ok(new PagedResultDto<EquipmentCatalogItemDto>(dtos, filteredEntities.Count, page, pageSize));
    }

    /// <summary>
    /// Get details for a specific catalog item.
    /// Used by React Front-office.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentCatalogDetailDto>> GetDetails(int id)
    {
        var entity = await _equipmentRepository.GetByIdAsync(id);
        
        // Ensure item is published and exists
        if (entity == null || !entity.IsPublished) return NotFound();

        var dto = _mapper.Map<EquipmentCatalogDetailDto>(entity);
        return Ok(dto);
    }

    /// <summary>
    /// Submit a contact request for equipment items.
    /// Used by React Front-office.
    /// </summary>
    [HttpPost("contact-requests")]
    public async Task<IActionResult> SubmitContactRequest(ContactRequestCreateDto requestDto)
    {
        // Validation is handled automatically by [ApiController] and DTO annotations
        var entity = _mapper.Map<ContactRequest>(requestDto);
        entity.CreatedAt = DateTime.UtcNow;
        entity.Status = ContactStatus.NEW;

        await _contactRepository.AddAsync(entity);

        return Ok(new { Message = "Votre demande a bien été envoyée." });
    }
}
