using AutoMapper;
using Karpaty.Api.DBCommunication.Reposetories;
using Karpaty.Api.DBCommunication.Reposetories.Interfaces;
using Karpaty.Api.DTOs.Place;
using Karpaty.Api.Models;
using Karpaty.Api.Services.Interfaces;

namespace Karpaty.Api.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IPlaceReposetory _placeRepository;
        private readonly IMapper _mapper;

        public PlaceService(IPlaceReposetory placeRepository, IMapper mapper)
        {
            _placeRepository = placeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Place>> GetAllPlacesAsync()
        {
            return await _placeRepository.GetAllAsync();
        }

        public async Task<Place> GetPlaceByIdAsync(string id)
        {
            return await _placeRepository.GetByIdAsync(id);
        }

        public async Task<Place> CreatePlaceAsync(CreatePlaceDTO dto)
        {
            var newPlace = _mapper.Map<Place>(dto);

            await _placeRepository.CreateAsync(newPlace);

            return newPlace;
        }

        public async Task<Place> UpdatePlaceAsync(string id, UpdatePlaceDTO dto)
        {
            var existingPlace = await _placeRepository.GetByIdAsync(id);
            if (existingPlace == null) return null;

            if (dto.description != null)
                existingPlace.description = dto.description;

            if (dto.imageUrl != null)
                existingPlace.imageUrl = dto.imageUrl;

            await _placeRepository.UpdateAsync(existingPlace);

            return existingPlace;
        }

        public async Task<bool> DeletePlaceAsync(string id)
        {
            var existingPlace = await _placeRepository.GetByIdAsync(id);
            if (existingPlace == null) return false;

            await _placeRepository.DeleteAsync(id);
            return true;
        }
    }
}

