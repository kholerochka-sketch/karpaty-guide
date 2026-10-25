using Microsoft.AspNetCore.Mvc;
using Karpaty.Api.Models;
using Karpaty.Api.Services.Interfaces;
namespace Karpaty.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _eventService;

        public PlaceController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Place>>> GetAll()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Place>> GetById(string id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);

            if (eventItem == null)
                return NotFound($"Подію з ID={id} не знайдено.");

            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<ActionResult<Place>> Create([FromBody] CreateEventDTO dto)
        {
            try
            {
                var createdEvent = await _eventService.CreateEventAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] UpdateEventDTO patchDto)
        {
            var updatedEvent = await _eventService.UpdateEventAsync(id, patchDto);

            if (updatedEvent == null)
                return NotFound($"Подію з ID={id} не знайдено.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var isDeleted = await _eventService.DeleteEventAsync(id);

            if (!isDeleted)
                return NotFound($"Подію з ID={id} не знайдено.");

            return NoContent();
        }
    }
}
