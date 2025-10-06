using HolidayApp.Api.Middleware;
using HolidayApp.Application;
using HolidayApp.Infrastructure;
using HolidayApp.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "Holiday API", Version = "v1" });
});

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMemoryCache();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

// Validate container configuration at startup
builder.Services.AddOptions<ServiceProviderOptions>()
    .Configure(options => options.ValidateOnBuild = true);

var app = builder.Build();

// Global exception handler - must be first
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Holiday API");
  c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});
app.ApplyMigrations();

// Health check endpoint
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
