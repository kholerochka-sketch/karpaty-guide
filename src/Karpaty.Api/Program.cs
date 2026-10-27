using System.Threading.RateLimiting;
using Karpaty.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration["MongoDbSettings:ConnectionString"]
        ?? "mongodb://localhost:27017";
    var settings = MongoClientSettings.FromConnectionString(connectionString);
    settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    return new MongoClient(settings);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDbSettings:DatabaseName"] ?? "Karpatyyy";
    return client.GetDatabase(dbName);
});

// 2. Реєстрація сервісів та репозиторіїв
builder.Services.AddScoped<Karpaty.Api.DBCommunication.Reposetories.Interfaces.IPlaceReposetory, Karpaty.Api.DBCommunication.Reposetories.PlaceReposetory>();
builder.Services.AddScoped<Karpaty.Api.Services.Interfaces.IPlaceService, PlaceService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<TipService>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 3. Налаштування CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Rate Limiter для відгуків
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("FeedbackPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Конвеєр обробки запитів (Порядок має значення!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); //  Дозволяє .NET роздавати твої HTML, CSS, JS із папки wwwroot

app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html"); //   Автоматично відкриває сайт, коли заходиш 

app.Run();