using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
    {
        var entities = await _userRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<UserReadDto>>(entities));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDto>> GetById(int id)
    {
        var entity = await _userRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        return Ok(_mapper.Map<UserReadDto>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<UserReadDto>> Create(UserCreateDto createDto)
    {
        var entity = _mapper.Map<User>(createDto);
        // Password hashing would go here in a real app
        await _userRepository.AddAsync(entity);

        var readDto = _mapper.Map<UserReadDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, readDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserUpdateDto updateDto)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        _mapper.Map(updateDto, existing);
        await _userRepository.UpdateAsync(existing);
        
        return NoContent();
    }
}
