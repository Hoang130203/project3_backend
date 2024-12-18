using FileService.Models;
using FileService.Service;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection("FileService"));
builder.Services.AddScoped<IFileService, HybridFileService>();
builder.Services.AddScoped<ILocalFileService, FileServiceImpl>();
builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();
builder.Services.Configure<GoogleDriveOptions>(
    builder.Configuration.GetSection("GoogleDrive"));

//builder.Services.AddGrpc();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.

var storagePath = builder.Configuration["FileService:StoragePath"];
if (string.IsNullOrEmpty(storagePath))
{
    storagePath = "uploads"; // Default value
}

// Ensure directory exists
var fullPath = Path.Combine(Directory.GetCurrentDirectory(), storagePath);
Directory.CreateDirectory(fullPath);

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(fullPath),
    RequestPath = builder.Configuration["FileService:BaseUrl"] ?? "/files"
});
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//app.MapGrpcService<FileServiceGrpc>();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
