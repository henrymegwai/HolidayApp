using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Infrastructure.ExternalServices;
using HolidayApp.Infrastructure.ExternalServices.Configuration;
using HolidayApp.Infrastructure.Persistence;
using HolidayApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace HolidayApp.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabaseSetUp(services, configuration);
        services.AddScoped<IHolidayRepository, HolidayRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        // HttpClient with Polly policies
        services.AddHttpClient<INagerDateApiClient, NagerDateApiClient>()
            .AddPolicyHandler((serviceProvider, request) => GetRetryPolicy(serviceProvider))
            .AddPolicyHandler((serviceProvider, request) => GetCircuitBreakerPolicy(serviceProvider));
        services.AddNagerDateApiConfiguration(configuration);
    }
    
    private static void AddDatabaseSetUp(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("ConnectionStrings:Database");
        
        if(string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .EnableDetailedErrors());
    }
    
    private static void AddNagerDateApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<NagerDateApiConfiguration>()
            .Bind(configuration.GetSection(nameof(NagerDateApiConfiguration)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, _) =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<NagerDateApiClient>>();
                    logger.LogWarning(
                        "Retry {RetryCount} for Nager API after {DelaySeconds}s due to: {StatusCode}",
                        retryCount, timespan.TotalSeconds, outcome.Result?.StatusCode);
                });
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IServiceProvider serviceProvider)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (_, duration) =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<NagerDateApiClient>>();
                    logger.LogError("Circuit breaker opened for Nager Date Api for {DurationSeconds}s", duration.TotalSeconds);
                },
                onReset: () =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<NagerDateApiClient>>();
                    logger.LogInformation("Circuit breaker reset for Nager Date Api");
                });
    }
}