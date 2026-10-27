using System.Threading.RateLimiting;
using Karpaty.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування MongoDB (з обмеженням часу очікування, щоб при падінні БД швидше спрацьовував 500)
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27017";
    var settings = MongoClientSettings.FromConnectionString(connectionString);
    settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    return new MongoClient(settings);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDb:DatabaseName"] ?? "karpaty_db";
    return client.GetDatabase(dbName);
});

// 2. Реєстрація сервісів Backend 2
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<TipService>();

// 3. B2-7: Налаштування CORS (дозволено запити з фронтенду)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. B2-8 (Бонус): Rate Limiting для захисту POST /api/feedback від спаму
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            status = 429,
            error = "Забагато запитів. Спробуйте пізніше."
        }, cancellationToken: token);
    };

    options.AddPolicy("FeedbackPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3, // Максимум 3 відгуки
                Window = TimeSpan.FromMinutes(1), // За 1 хвилину
                QueueLimit = 0
            }));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. B2-6: Глобальна обробка помилок (повертає зрозумілий JSON і статус 500)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        if (exceptionFeature?.Error != null)
        {
            logger.LogError(exceptionFeature.Error, "Необроблена помилка під час виконання запиту.");
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = 500,
            error = "Внутрішня помилка сервера. Сервіс або база даних тимчасово недоступні.",
            timestamp = DateTime.UtcNow
        });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();