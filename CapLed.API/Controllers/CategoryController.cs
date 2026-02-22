using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var entities = await _categoryRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<CategoryDto>>(entities));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        return Ok(_mapper.Map<CategoryDto>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryDto categoryDto)
    {
        var entity = _mapper.Map<Category>(categoryDto);
        await _categoryRepository.AddAsync(entity);

        var readDto = _mapper.Map<CategoryDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, readDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryDto categoryDto)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        _mapper.Map(categoryDto, existing);
        await _categoryRepository.UpdateAsync(existing);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _categoryRepository.DeleteAsync(id);
        return NoContent();
    }
}
