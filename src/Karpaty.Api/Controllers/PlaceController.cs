using Karpaty.Api.DTOs;
using Karpaty.Api.DTOs.Place;
using Karpaty.Api.Models;
using Karpaty.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karpaty.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Place>>> GetAll()
        {
            var places = await _placeService.GetAllPlacesAsync();
            return Ok(places);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Place>> GetById(string id)
        {
            var place = await _placeService.GetPlaceByIdAsync(id);

            if (place == null)
                return NotFound($"Місце з ID={id} не знайдено.");

            return Ok(place);
        }

        [HttpPost]
        public async Task<ActionResult<Place>> Create([FromBody] CreatePlaceDTO dto)
        {
            try
            {
                var createdPlace = await _placeService.CreatePlaceAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = createdPlace.id }, createdPlace);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] UpdatePlaceDTO patchDto)
        {
            var updatedPlace = await _placeService.UpdatePlaceAsync(id, patchDto);

            if (updatedPlace == null)
                return NotFound($"Місце з ID={id} не знайдено.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var isDeleted = await _placeService.DeletePlaceAsync(id);

            if (!isDeleted)
                return NotFound($"Місце з ID={id} не знайдено.");

            return NoContent();
        }
    }
}