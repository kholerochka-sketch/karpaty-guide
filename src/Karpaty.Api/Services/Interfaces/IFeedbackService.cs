using Karpaty.Api.DTOs.Feedback;
using Karpaty.Api.Models;

namespace Karpaty.Api.Services.Interfaces;

public interface IFeedbackService
{
    Task<List<Feedback>> GetAllAsync();
    Task<Feedback?> GetByIdAsync(string id);
    Task<List<Feedback>> GetByPlaceIdAsync(string placeId);
    Task<Feedback> CreateAsync(CreateFeedbackDTO dto);
    Task<Feedback?> UpdateAsync(string id, UpdateFeedbackDTO dto);
    Task<bool> DeleteAsync(string id);
}