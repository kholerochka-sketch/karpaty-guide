using Karpaty.Api.DTOs.Tip;
using Karpaty.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karpaty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipsController : ControllerBase
{
    private readonly ITipService _tipService;

    public TipsController(ITipService tipService)
    {
        _tipService = tipService;
    }

    // GET /api/tips
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tips = await _tipService.GetAllAsync();
        return Ok(tips);
    }

    // GET /api/tips/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var tip = await _tipService.GetByIdAsync(id);
        if (tip == null)
            return NotFound(new { status = 404, error = $"Пораду з ID '{id}' не знайдено." });

        return Ok(tip);
    }

    // GET /api/tips/place/{placeId} — поради для конкретного обраного місця
    [HttpGet("place/{placeId}")]
    public async Task<IActionResult> GetByPlaceId(string placeId)
    {
        var tips = await _tipService.GetByPlaceIdAsync(placeId);
        return Ok(tips);
    }

    // POST /api/tips
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTipDTO dto)
    {
        var created = await _tipService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/tips/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTipDTO dto)
    {
        var updated = await _tipService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(new { status = 404, error = $"Пораду з ID '{id}' не знайдено." });

        return Ok(updated);
    }

    // DELETE /api/tips/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _tipService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { status = 404, error = $"Пораду з ID '{id}' не знайдено." });

        return NoContent();
    }
}