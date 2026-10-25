using Karpaty.Api;
using Karpaty.Api.DBCommunication.Reposetories;
using Karpaty.Api.DBCommunication.Reposetories.Interfaces;
using Karpaty.Api.Services;
using Karpaty.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Karpaty.Api.DBCommunication.Reposetories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Реєструємо клієнт та базу даних MongoDB
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

// Реєструємо ваші репозиторії
builder.Services.AddScoped<IPlaceReposetory, PlaceReposetory>();
builder.Services.AddSingleton<IPlaceReposetory, PlaceReposetory>();

// Реєстрація сервісу (це те, через що падає помилка)
builder.Services.AddSingleton<IPlaceService, PlaceService>();
// Реєструємо MongoClient як Singleton (створюється один раз на всю програму)
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();


