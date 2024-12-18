using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Configurations;
using NotificationService.Hubs;
using NotificationService.Interfaces;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
// Add Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
// Add RabbitMQ
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));


// Add Services
builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


// Add Channels
builder.Services.AddScoped<INotificationChannel, WebSocketChannel>();
//builder.Services.AddScoped<INotificationChannel, EmailChannel>();
//builder.Services.AddScoped<INotificationChannel, SmsChannel>();

// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();
//builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
             .AllowAnyOrigin() // Cho phép mọi origin
            .AllowAnyMethod()
            .AllowAnyHeader());
           
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapHub<NotificationHub>("/notificationHub");
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
