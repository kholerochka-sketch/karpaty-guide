using Karpaty.Api.DTOs.Tip;
using Karpaty.Api.Models;

namespace Karpaty.Api.Services.Interfaces;

public interface ITipService
{
    Task<List<Tip>> GetAllAsync();
    Task<Tip?> GetByIdAsync(string id);
    Task<List<Tip>> GetByPlaceIdAsync(string placeId);
    Task<Tip> CreateAsync(CreateTipDTO dto);
    Task<Tip?> UpdateAsync(string id, UpdateTipDTO dto);
    Task<bool> DeleteAsync(string id);
}