using Karpaty.Api.DTOs.Feedback;
using Karpaty.Api.Models;
using Karpaty.Api.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Karpaty.Api.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IMongoCollection<Feedback> _feedbackCollection;

    public FeedbackService(IMongoDatabase database)
    {
        _feedbackCollection = database.GetCollection<Feedback>("feedback");
    }

    public async Task<List<Feedback>> GetAllAsync()
    {
        return await _feedbackCollection
            .Find(_ => true)
            .SortByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Feedback?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _feedbackCollection
            .Find(f => f.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Feedback>> GetByPlaceIdAsync(string placeId)
    {
        return await _feedbackCollection
            .Find(f => f.PlaceId == placeId)
            .SortByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Feedback> CreateAsync(CreateFeedbackDTO dto)
    {
        var feedback = new Feedback
        {
            Id = null,
            PlaceId = dto.PlaceId,
            Name = dto.Name,
            Email = dto.Email,
            Rating = dto.Rating,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow
        };

        await _feedbackCollection.InsertOneAsync(feedback);
        return feedback;
    }

    public async Task<Feedback?> UpdateAsync(string id, UpdateFeedbackDTO dto)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        var existing = await GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.PlaceId = dto.PlaceId;
        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.Rating = dto.Rating;
        existing.Message = dto.Message;

        await _feedbackCollection.ReplaceOneAsync(f => f.Id == id, existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await _feedbackCollection.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }
}