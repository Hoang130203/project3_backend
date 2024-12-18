using MessageService.Models;
using MessageService;
using MessageService.Hubs;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ProfileService.Proto;
using MessageService.Interfaces;
using PostService.Services;
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
ConventionRegistry.Register("Guid string convention",
    new ConventionPack { new StringObjectIdIdGeneratorConvention() }, _ => true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddGrpcClient<ProfileServiceGrpc.ProfileServiceGrpcClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:ProfileServiceUrl"]!);
});
builder.Services.AddScoped<IProfileGrpcClient, ProfileGrpcClientService>();

builder.Services.AddSignalR();

// Cấu hình dịch vụ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Chỉ cho phép từ frontend
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Hỗ trợ cookie hoặc xác thực
    });
});

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapHub<ChatHub>("/chat");
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.Run();

