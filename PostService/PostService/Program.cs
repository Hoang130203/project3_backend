using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using PostService.Infrastructure;
using PostService.Interfaces;
using PostService.Services;
using PostService.Protos;
using ProfileService.Proto;
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
ConventionRegistry.Register("Guid string convention",
    new ConventionPack { new StringObjectIdIdGeneratorConvention() }, _ => true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Cho phép tất cả nguồn
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPostRepository, PostRepository>();
//builder.Services.AddHttpClient<PostModerationService>();
builder.Services.AddScoped<IPostModerationService, PostModerationService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IPostUpdateService, PostUpdateService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
// Cấu hình MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbClient>();
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2; // Kích hoạt HTTP/2
    });
});
// Add gRPC client
builder.Services.AddGrpcClient<ConnectionServiceGrpc.ConnectionServiceGrpcClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:AuthServiceUrl"]!);
});
builder.Services.AddGrpcClient<ProfileServiceGrpc.ProfileServiceGrpcClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:ProfileServiceUrl"]!);
});
// Register services
builder.Services.AddScoped<IConnectionGrpcClient, ConnectionGrpcClientService>();
builder.Services.AddScoped<IProfileGrpcClient, ProfileGrpcClientService>();
var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // Add this line
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
