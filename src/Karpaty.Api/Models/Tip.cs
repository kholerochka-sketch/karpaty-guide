using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Karpaty.Api.Models;

public class Tip
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required(ErrorMessage = "Заголовок поради є обов'язковим.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Заголовок має містити від 3 до 100 символів.")]
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Категорія є обов'язковою (спорядження, безпека, погода, поведінка, екологія).")]
    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Текст поради є обов'язковим.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Текст поради має містити від 10 до 1000 символів.")]
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;
}