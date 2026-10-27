using Karpaty.Api.Models;
using Karpaty.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Karpaty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipsController : ControllerBase
{
    private readonly TipService _tipService;

    public TipsController(TipService tipService)
    {
        _tipService = tipService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tips = await _tipService.GetAllAsync();
        return Ok(tips);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Tip tip)
    {
        var created = await _tipService.CreateAsync(tip);
        return Created($"/api/tips/{created.Id}", created);
    }
}