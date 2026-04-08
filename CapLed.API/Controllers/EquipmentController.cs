using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;

using Microsoft.AspNetCore.Authorization;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public EquipmentController(IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// List equipment with filtering and pagination.
    /// Used by WPF Back-office.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EquipmentListItemDto>>> GetAll(
        [FromQuery] int? familleId,
        [FromQuery] int? categoryId, 
        [FromQuery] EquipmentCondition? condition,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (entities, totalCount) = await _equipmentRepository.GetAllAsync(
            familleId, categoryId, condition, isPublished: null, search, page, pageSize);
        
        var dtos = _mapper.Map<IEnumerable<EquipmentListItemDto>>(entities);
        
        return Ok(new PagedResultDto<EquipmentListItemDto>(dtos, totalCount, page, pageSize));
    }

    /// <summary>
    /// Get detailed equipment information.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentReadDto>> GetById(int id)
    {
        var entity = await _equipmentRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var dto = _mapper.Map<EquipmentReadDto>(entity);
        return Ok(dto);
    }

    /// <summary>
    /// Create a new equipment item.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EquipmentReadDto>> Create([FromBody] EquipmentCreateDto createDto)
    {
        var entity = _mapper.Map<Equipment>(createDto);
        entity.CreatedAt = DateTime.UtcNow;

        await _equipmentRepository.AddAsync(entity);

        var readDto = _mapper.Map<EquipmentReadDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, readDto);
    }

    /// <summary>
    /// Update an existing equipment item.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpdateDto updateDto)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        _mapper.Map(updateDto, existing);
        await _equipmentRepository.UpdateAsync(existing);
        
        return NoContent();
    }

    /// <summary>
    /// Delete an equipment item.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _equipmentRepository.DeleteAsync(id);
        return NoContent();
    }
}
