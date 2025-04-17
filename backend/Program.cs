using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Add CORS with more permissive settings for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
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

// Use CORS before other middleware
app.UseCors("AllowAll");

// Remove HTTPS redirection
// app.UseHttpsRedirection();

app.UseAuthorization();

// Log all incoming requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Ensure controllers are mapped
app.MapControllers();

// Configure Kestrel to use HTTP only
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

// Log all registered endpoints
var logger = app.Services.GetRequiredService<ILogger<Program>>();
foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<EndpointDataSource>>().SelectMany(e => e.Endpoints))
{
    logger.LogInformation($"Registered endpoint: {endpoint.DisplayName}");
}

app.Run(); 