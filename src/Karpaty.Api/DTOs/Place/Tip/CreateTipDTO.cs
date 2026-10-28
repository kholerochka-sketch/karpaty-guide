using System.ComponentModel.DataAnnotations;

namespace Karpaty.Api.DTOs.Tip;

public class CreateTipDTO
{
    public string? PlaceId { get; set; }

    [Required(ErrorMessage = "Заголовок поради є обов'язковим.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Заголовок має містити від 3 до 100 символів.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Категорія є обов'язковою.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Текст поради є обов'язковим.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Текст поради має містити від 10 до 1000 символів.")]
    public string Content { get; set; } = string.Empty;
}