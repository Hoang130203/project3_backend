using ApiGateway;
using ApiGateway.Handlers;
using ApiGateway.Proto;
using ApiGateway.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddMemoryCache();

// Add configuration
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // Example: 100MB (104857600 bytes)
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for Social Media Application"
    });

    // Add JWT authentication configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Add our custom document filter
    //c.DocumentFilter<SwaggerDocumentFilter>();
});
// Add gRPC client
builder.Services.AddGrpcClient<AuthServiceGrpc.AuthServiceGrpcClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:AuthServiceUrl"]!);
});

// Register services
builder.Services.AddScoped<IAuthGrpcClient, AuthGrpcClientService>();
builder.Services.AddScoped<SimpleAuthHandler>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Add YARP reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        if (handler is SocketsHttpHandler socketsHandler)
        {
            socketsHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true // Bỏ qua xác thực
            };
            socketsHandler.EnableMultipleHttp2Connections = true;
            socketsHandler.KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests;
        }
    });
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
    });
});
// Add cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Tích hợp các microservices Swagger JSON
        c.SwaggerEndpoint("http://localhost:4040/swagger/v1/swagger.json", "Auth Service");
        c.SwaggerEndpoint("http://localhost:4060/swagger/v1/swagger.json", "Post Service");
        c.SwaggerEndpoint("http://localhost:4050/swagger/v1/swagger.json", "File Service");
        c.SwaggerEndpoint("http://localhost:4080/swagger/v1/swagger.json", "Profile Service");


        c.RoutePrefix = "swagger"; // Đặt đường dẫn Swagger UI tại /swagger
    });
}
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
});
app.UseCors();

app.UseRouting();

// Use SimpleAuthHandler middleware
app.UseMiddleware<SimpleAuthHandler>();

// Map reverse proxy routes
app.MapReverseProxy();


app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();