using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Repositories;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _repo;
    private readonly IMapper           _mapper;

    public ClientsController(IClientRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientReadDto>>> GetAll()
        => Ok(_mapper.Map<List<ClientReadDto>>(await _repo.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientReadDto>> GetById(int id)
    {
        var client = await _repo.GetByIdAsync(id);
        return client == null ? NotFound() : Ok(_mapper.Map<ClientReadDto>(client));
    }

    [HttpPost]
    public async Task<ActionResult<ClientReadDto>> Create(CreateClientDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing != null)
            return Conflict(new { message = $"Un client avec l'email '{dto.Email}' existe déjà." });

        var client = _mapper.Map<StockManager.Core.Domain.Entities.Commercial.Client>(dto);
        client.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(client);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, _mapper.Map<ClientReadDto>(client));
    }
}
