using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Catalogue;
using Microsoft.AspNetCore.Authorization;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FamilleController : ControllerBase
{
    private readonly IFamilleRepository _familleRepository;
    private readonly IMapper _mapper;

    public FamilleController(IFamilleRepository familleRepository, IMapper mapper)
    {
        _familleRepository = familleRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FamilleDto>>> GetAll()
    {
        var entities = await _familleRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<FamilleDto>>(entities));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FamilleDto>> GetById(int id)
    {
        var entity = await _familleRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        return Ok(_mapper.Map<FamilleDto>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<FamilleDto>> Create(FamilleDto familleDto)
    {
        var entity = _mapper.Map<Famille>(familleDto);
        await _familleRepository.AddAsync(entity);

        var readDto = _mapper.Map<FamilleDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, readDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, FamilleDto familleDto)
    {
        var existing = await _familleRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        _mapper.Map(familleDto, existing);
        await _familleRepository.UpdateAsync(existing);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _familleRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _familleRepository.DeleteAsync(id);
        return NoContent();
    }
}
