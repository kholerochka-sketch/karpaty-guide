using System.ComponentModel.DataAnnotations;

namespace Karpaty.Api.DTOs.Feedback;

public class CreateFeedbackDTO
{
    public string? PlaceId { get; set; }

    [Required(ErrorMessage = "Ім'я є обов'язковим.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я повинно містити від 2 до 50 символів.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Некоректний формат email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Оцінка є обов'язковою.")]
    [Range(1, 5, ErrorMessage = "Оцінка повинна бути від 1 до 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Повідомлення є обов'язковим.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Повідомлення повинно містити від 10 до 1000 символів.")]
    public string Message { get; set; } = string.Empty;
}