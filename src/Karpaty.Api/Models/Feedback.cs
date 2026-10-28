using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Karpaty.Api.Models;

public class Feedback
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // Прив'язка до конкретного місця (Place.id)
    [BsonElement("placeId")]
    public string? PlaceId { get; set; }

    [Required(ErrorMessage = "Ім'я є обов'язковим.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я повинно містити від 2 до 50 символів.")]
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Некоректний формат email.")]
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Оцінка є обов'язковою.")]
    [Range(1, 5, ErrorMessage = "Оцінка повинна бути від 1 до 5.")]
    [BsonElement("rating")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Повідомлення є обов'язковим.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Повідомлення повинно містити від 10 до 1000 символів.")]
    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}