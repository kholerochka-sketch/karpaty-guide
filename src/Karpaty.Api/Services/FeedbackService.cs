using Karpaty.Api.Models;
using MongoDB.Driver;

namespace Karpaty.Api.Services;

public class FeedbackService
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

    public async Task<Feedback> CreateAsync(Feedback feedback)
    {
        feedback.Id = null;
        feedback.CreatedAt = DateTime.UtcNow;
        await _feedbackCollection.InsertOneAsync(feedback);
        return feedback;
    }
}