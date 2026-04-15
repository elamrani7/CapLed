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

    /// <summary>
    /// Upload images for an equipment item.
    /// </summary>
    [HttpPost("{id}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImages(int id, [FromForm] IFormFileCollection files)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing == null) return NotFound("Article introuvable.");

        if (files == null || files.Count == 0) return BadRequest("Aucun fichier reçu.");

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "equipments");
        bool isFirstPhoto = !existing.Photos.Any();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName.Replace(" ", "_")}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                existing.Photos.Add(new Photo
                {
                    EquipmentId = id,
                    Url = $"/images/equipments/{fileName}",
                    IsPrimary = isFirstPhoto
                });
                
                isFirstPhoto = false;
            }
        }

        await _equipmentRepository.UpdateAsync(existing);
        
        var readDto = _mapper.Map<EquipmentReadDto>(existing);
        return Ok(readDto);
    }

    /// <summary>
    /// Delete an image.
    /// </summary>
    [HttpDelete("{id}/images/{photoId}")]
    public async Task<IActionResult> DeleteImage(int id, int photoId)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing == null) return NotFound("Article introuvable.");

        var photo = existing.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null) return NotFound("Image introuvable.");

        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.Url.TrimStart('/'));
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        existing.Photos.Remove(photo);
        
        if (photo.IsPrimary && existing.Photos.Any())
        {
            existing.Photos.First().IsPrimary = true;
        }

        await _equipmentRepository.UpdateAsync(existing);
        var readDto = _mapper.Map<EquipmentReadDto>(existing);
        return Ok(readDto);
    }

    /// <summary>
    /// Set an image as primary.
    /// </summary>
    [HttpPut("{id}/images/{photoId}/primary")]
    public async Task<IActionResult> SetPrimaryImage(int id, int photoId)
    {
        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing == null) return NotFound("Article introuvable.");

        var photo = existing.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null) return NotFound("Image introuvable.");

        foreach (var p in existing.Photos)
        {
            p.IsPrimary = (p.Id == photoId);
        }

        await _equipmentRepository.UpdateAsync(existing);
        var readDto = _mapper.Map<EquipmentReadDto>(existing);
        return Ok(readDto);
    }
}
