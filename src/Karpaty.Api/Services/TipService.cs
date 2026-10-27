using Karpaty.Api.Models;
using MongoDB.Driver;

namespace Karpaty.Api.Services;

public class TipService
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

    public async Task<Tip> CreateAsync(Tip tip)
    {
        tip.Id = null;
        await _tipsCollection.InsertOneAsync(tip);
        return tip;
    }

    private async Task SeedInitialTipsAsync()
    {
        var count = await _tipsCollection.CountDocumentsAsync(_ => true);
        if (count > 0) return;

        var initialTips = new List<Tip>
        {
            new()
            {
                Title = "Тришаровий принцип одягу",
                Category = "Спорядження",
                Content = "Використовуйте термобілизну для відведення вологи, фліс для утеплення та мембранну куртку для захисту від вітру й дощу. Уникайте бавовняних речей."
            },
            new()
            {
                Title = "Взуття та трекінгові палиці",
                Category = "Спорядження",
                Content = "Обирайте розношені трекінгові черевики з фіксацією гомілкостопу та підошвою Vibram. Трекінгові палиці зменшують навантаження на коліна під час спуску на 25–30%."
            },
            new()
            {
                Title = "Реєстрація маршруту у рятувальників",
                Category = "Безпека",
                Content = "Перед виходом у гори обов'язково зареєструйте свій маршрут у гірських пошуково-рятувальних підрозділах ДСНС або через застосунок «Порятунок у горах»."
            },
            new()
            {
                Title = "Запас води, їжі та ліхтарик",
                Category = "Безпека",
                Content = "Навіть у одноденний похід беріть налобний ліхтарик із запасними батарейками, заряджений павербанк, аптечку, свисток та аварійний перекус (горіхи, батончики)."
            },
            new()
            {
                Title = "Ранній вихід та грозова небезпека",
                Category = "Погода",
                Content = "У Карпатах грози найчастіше трапляються після 14:00. Виходьте на маршрут рано-вранці, щоб пройти відкриті хребти та вершини до обіду."
            },
            new()
            {
                Title = "Дотримання маркованих стежок",
                Category = "Поведінка в горах",
                Content = "Не сходьте з маркованих маршрутів і не намагайтеся «зрізати» шлях через жереп (альпійську сосну) — це небезпечно та забирає в рази більше сил і часу."
            },
            new()
            {
                Title = "Принцип Leave No Trace (Не залишай слідів)",
                Category = "Прибирання за собою",
                Content = "Усе сміття (включно з вологими серветками, обгортками та бляшанками) забирайте з собою до найближчого контейнера в населеному пункті. Не розводьте багаття на високогір'ї."
            }
        };

        await _tipsCollection.InsertManyAsync(initialTips);
    }
}