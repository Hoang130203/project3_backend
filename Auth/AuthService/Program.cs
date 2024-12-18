//using AuthService.Application.Filters;
using AuthService.Application.Services;
using AuthService.Application.Interfaces.Services;
using AuthService.Application;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Data.Extensions;

using JwtConfiguration;
using Microsoft.AspNetCore.Rewrite;
using AuthService;
using AuthService.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
var Conf = builder.Configuration;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Cho phép tất cả nguồn
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IEncryptor, Encryptor>();
builder.Services.AddControllers(options=>
    {
        //options.Filters.Add(typeof(RequirePermissionAttribute));  
    }
);
builder.Services
     .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);
builder.Services.AddJwt(Conf);

builder.WebHost.ConfigureKestrel(options =>
{
    // Cấu hình HTTP API (REST) trên cổng 8080
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });

    // Cấu hình gRPC trên cổng 5001
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; // HTTP/2 cho gRPC
    });
});



builder.Services.AddGrpc();


var app = builder.Build();
//grpc
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<ConnectionGrpcService>();
app.UseCors("AllowAllOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
app.UseRouting();
app.MapCarter();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

