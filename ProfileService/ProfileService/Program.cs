using ProfileService.Services;
using BuildingBlocks.Messaging.MassTransit;
using ProfileService.EventHandlers.Integration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using ProfileService.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProfileService.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MassTransit;
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
builder.Services.AddGrpc();
builder.Services.AddMassTransit(config =>
{
    // Add all consumers in the assembly
    config.AddConsumer<CreateUserEventHandler>();
    config.AddConsumer<CreateGroupEventHandler>();
    config.AddConsumer<UpdateGroupEventHandler>();
    config.SetKebabCaseEndpointNameFormatter();

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["MessageBroker:Host"], host =>
        {
            host.Username(builder.Configuration["MessageBroker:UserName"]);
            host.Password(builder.Configuration["MessageBroker:Password"]);
        });

        // Configure endpoints for consumers
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("RedisSettings:ConnectionString").Value;
    options.InstanceName = builder.Configuration.GetSection("RedisSettings:InstanceName").Value;
});
// Cấu hình MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbClient>();


//repository
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.Decorate<IProfileRepository, CachedProfileRepository>();
builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration["RedisSettings:ConnectionString"]!, name: "redis");
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP API (REST) on port 8080
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    // gRPC on port 5001
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
app.MapGrpcService<ProfileGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
app.Run();
