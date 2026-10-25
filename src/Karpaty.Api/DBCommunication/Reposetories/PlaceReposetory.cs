using AutoMapper;
using Karpaty.Api.DBCommunication.Documents;
using Karpaty.Api.DBCommunication.Reposetories.Interfaces;
using Karpaty.Api.Models;
using MongoDB.Driver;

namespace Karpaty.Api.DBCommunication.Reposetories
{
    public class PlaceRepository : IPlaceReposetory
    {
        private readonly IMongoCollection<PlaceDocument> _collection;
        private readonly IMapper _mapper;

        public PlaceRepository(IMongoDatabase database, IMapper mapper)
        {
            _collection = database.GetCollection<PlaceDocument>("places");
            _mapper = mapper;
        }

        public async Task CreateAsync(Place place)
        {
            var document = _mapper.Map<PlaceDocument>(place);

            await _collection.InsertOneAsync(document);

            place.id = document.id;
        }

        public async Task<List<Place>> GetAllAsync()
        {
            var documents = await _collection.Find(_ => true).ToListAsync();
            return _mapper.Map<List<Place>>(documents);
        }

        public async Task<Place?> GetByIdAsync(string id)
        {
            var document = await _collection.Find(x => x.id == id).FirstOrDefaultAsync();
            return document is null ? null : _mapper.Map<Place>(document);
        }

        public async Task UpdateAsync(Place place)
        {
            var document = _mapper.Map<PlaceDocument>(place);
            await _collection.ReplaceOneAsync(x => x.id == place.id, document);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.id == id);
        }
    }
}
