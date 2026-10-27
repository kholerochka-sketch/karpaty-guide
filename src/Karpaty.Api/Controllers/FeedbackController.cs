using Karpaty.Api.Models;
using Karpaty.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Karpaty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly FeedbackService _feedbackService;
    private readonly IConfiguration _configuration;

    public FeedbackController(FeedbackService feedbackService, IConfiguration configuration)
    {
        _feedbackService = feedbackService;
        _configuration = configuration;
    }

    // B2-3 та B2-8: Збереження відгуку (201 Created) + обмеження частоти запитів (429 Too Many Requests)
    [HttpPost]
    [EnableRateLimiting("FeedbackPolicy")]
    public async Task<IActionResult> Create([FromBody] Feedback feedback)
    {
        var created = await _feedbackService.CreateAsync(feedback);
        return Created($"/api/feedback/{created.Id}", created);
    }

    // B2-4: Перегляд усіх відгуків лише з правильним заголовком X-Admin-Key
    [HttpGet]
    public async Task<IActionResult> GetAll([FromHeader(Name = "X-Admin-Key")] string? adminKey)
    {
        var expectedKey = _configuration["AdminKey"];

        if (string.IsNullOrWhiteSpace(adminKey) || adminKey != expectedKey)
        {
            return Unauthorized(new
            {
                status = 401,
                error = "Доступ заборонено. Вкажіть валідний заголовок X-Admin-Key."
            });
        }

        var feedbacks = await _feedbackService.GetAllAsync();
        return Ok(feedbacks);
    }
}