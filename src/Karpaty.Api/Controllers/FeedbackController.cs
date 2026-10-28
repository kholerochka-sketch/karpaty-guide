using Karpaty.Api.DTOs.Feedback;
using Karpaty.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karpaty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    private readonly IConfiguration _configuration;

    public FeedbackController(IFeedbackService feedbackService, IConfiguration configuration)
    {
        _feedbackService = feedbackService;
        _configuration = configuration;
    }

    // GET /api/feedback (доступний з X-Admin-Key)
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

    // GET /api/feedback/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback == null)
            return NotFound(new { status = 404, error = $"Відгук з ID '{id}' не знайдено." });

        return Ok(feedback);
    }

    // GET /api/feedback/place/{placeId} — всі відгуки до конкретного місця
    [HttpGet("place/{placeId}")]
    public async Task<IActionResult> GetByPlaceId(string placeId)
    {
        var feedbacks = await _feedbackService.GetByPlaceIdAsync(placeId);
        return Ok(feedbacks);
    }

    // POST /api/feedback
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackDTO dto)
    {
        var created = await _feedbackService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/feedback/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateFeedbackDTO dto)
    {
        var updated = await _feedbackService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(new { status = 404, error = $"Відгук з ID '{id}' не знайдено." });

        return Ok(updated);
    }

    // DELETE /api/feedback/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _feedbackService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { status = 404, error = $"Відгук з ID '{id}' не знайдено." });

        return NoContent();
    }
}