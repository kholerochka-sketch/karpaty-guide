using Karpaty.Api.DTOs.Tip;
using Karpaty.Api.Models;
using Karpaty.Api.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Karpaty.Api.Services;

public class TipService : ITipService
{
    private readonly IMongoCollection<Tip> _tipsCollection;

    public TipService(IMongoDatabase database)
    {
        _tipsCollection = database.GetCollection<Tip>("tips");
    }

    public async Task<List<Tip>> GetAllAsync()
    {
        await SeedInitialTipsAsync();
        return await _tipsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Tip?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _tipsCollection
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Tip>> GetByPlaceIdAsync(string placeId)
    {
        return await _tipsCollection
            .Find(t => t.PlaceId == placeId)
            .ToListAsync();
    }

    public async Task<Tip> CreateAsync(CreateTipDTO dto)
    {
        var tip = new Tip
        {
            Id = null,
            PlaceId = dto.PlaceId,
            Title = dto.Title,
            Category = dto.Category,
            Content = dto.Content
        };

        await _tipsCollection.InsertOneAsync(tip);
        return tip;
    }

    public async Task<Tip?> UpdateAsync(string id, UpdateTipDTO dto)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        var existing = await GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.PlaceId = dto.PlaceId;
        existing.Title = dto.Title;
        existing.Category = dto.Category;
        existing.Content = dto.Content;

        await _tipsCollection.ReplaceOneAsync(t => t.Id == id, existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await _tipsCollection.DeleteOneAsync(t => t.Id == id);
        return result.DeletedCount > 0;
    }

    private async Task SeedInitialTipsAsync()
    {
        var count = await _tipsCollection.CountDocumentsAsync(_ => true);
        if (count > 0) return;

        var initialTips = new List<Tip>
        {
            new()
            {
                PlaceId = null,
                Title = "Тришаровий принцип одягу",
                Category = "Спорядження",
                Content = "Використовуйте термобілизну для відведення вологи, фліс для утеплення та мембранну куртку для захисту від вітру й дощу. Уникайте бавовняних речей."
            },
            new()
            {
                PlaceId = null,
                Title = "Взуття та трекінгові палиці",
                Category = "Спорядження",
                Content = "Обирайте розношені трекінгові черевики з фіксацією гомілкостопу та підошвою Vibram. Трекінгові палиці зменшують навантаження на коліна під час спуску на 25–30%."
            },
            new()
            {
                PlaceId = null,
                Title = "Реєстрація маршруту у рятувальників",
                Category = "Безпека",
                Content = "Перед виходом у гори обов'язково зареєструйте свій маршрут у гірських пошуково-рятувальних підрозділах ДСНС або через застосунок «Порятунок у горах»."
            },
            new()
            {
                PlaceId = null,
                Title = "Запас води, їжі та ліхтарик",
                Category = "Безпека",
                Content = "Навіть у одноденний похід беріть налобний ліхтарик із запасними батарейками, заряджений павербанк, аптечку, свисток та аварійний перекус (горіхи, батончики)."
            },
            new()
            {
                PlaceId = null,
                Title = "Ранній вихід та грозова небезпека",
                Category = "Погода",
                Content = "У Карпатах грози найчастіше трапляються після 14:00. Виходьте на маршрут рано-вранці, щоб пройти відкриті хребти та вершини до обіду."
            },
            new()
            {
                PlaceId = null,
                Title = "Дотримання маркованих стежок",
                Category = "Поведінка в горах",
                Content = "Не сходьте з маркованих маршрутів і не намагайтеся «зрізати» шлях через жереп (альпійську сосну) — це небезпечно та забирає в рази більше сил і часу."
            },
            new()
            {
                PlaceId = null,
                Title = "Принцип Leave No Trace (Не залишай слідів)",
                Category = "Прибирання за собою",
                Content = "Усе сміття (включно з вологими серветками, обгортками та бляшанками) забирайте з собою до найближчого контейнера в населеному пункті. Не розводьте багаття на високогір'ї."
            }
        };

        await _tipsCollection.InsertManyAsync(initialTips);
    }
}