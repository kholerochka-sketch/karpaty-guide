using Karpaty.Api;
using Karpaty.Api.DBCommunication.Reposetories;
using Karpaty.Api.DBCommunication.Reposetories.Interfaces;
using Karpaty.Api.Services;
using Karpaty.Api.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1. Єдине налаштування MongoDB (підтримує і MongoDBSettings, і MongoDb)
builder.Services.Configure<MongoDBSettings>(options =>
{
    builder.Configuration.GetSection("MongoDBSettings").Bind(options);

    if (string.IsNullOrWhiteSpace(options.ConnectionString))
        options.ConnectionString = builder.Configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27017";

    if (string.IsNullOrWhiteSpace(options.DatabaseName))
        options.DatabaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "karpaty_db";
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 2. Реєструємо IMongoClient один раз як Singleton
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
    mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    return new MongoClient(mongoSettings);
});

// 3. Реєструємо IMongoDatabase через вже створений IMongoClient
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// 4. Реєстрація репозиторіїв та сервісів Backend 1 (Places)
builder.Services.AddSingleton<IPlaceReposetory, PlaceReposetory>();
builder.Services.AddSingleton<IPlaceService, PlaceService>();

// 5. Реєстрація сервісів Backend 2 (Feedback + Tips) через інтерфейси
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ITipService, TipService>();

// 6. Налаштування CORS (B2-7)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 7. Глобальна обробка помилок (B2-6: повертає JSON і код 500)
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();