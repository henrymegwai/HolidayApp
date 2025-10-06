using FluentValidation;
using HolidayApp.Application.Common.Behaviors;
using HolidayApp.Application.Common.Decorators;
using HolidayApp.Application.Common.Models.Dtos;
using HolidayApp.Application.Features.Country.Commands;
using HolidayApp.Application.Features.Holiday.Commands;
using HolidayApp.Application.Features.Holiday.Queries.GetLastCelebratedHolidays;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceRegistration).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // Register cache decorators
        services.AddCacheDecorator<GetLastCelebratedHolidaysQuery, List<LastCelebratedHolidayDto>>(
            request => $"last_holidays_{request.CountryCode}",
            TimeSpan.FromHours(1));
            
        // Register cache invalidation decorators
        services.AddCacheInvalidationDecorator<LoadHolidaysCommand, LoadHolidaysResult>(
            request => [$"last_holidays_{request.CountryCode}"]);
            
        // Register cache read-write decorators
        services.AddCacheReadWriteDecorator<LoadCountriesCommand, LoadCountriesResult>(
            request => "all_countries",
            (request, response) => "all_countries",
            TimeSpan.FromHours(24));
    }

    private static void AddCacheDecorator<TRequest, TResponse>(this IServiceCollection services,
        Func<TRequest, string> cacheKeyGenerator,
        TimeSpan cacheDuration)
    {
        services.AddScoped<ICacheDecorator<TRequest, TResponse>>(provider =>
        {
            var cache = provider.GetRequiredService<IMemoryCache>();
            var logger = provider.GetRequiredService<ILogger<CacheDecorator<TRequest, TResponse>>>();
            
            return new CacheDecorator<TRequest, TResponse>(
                cache,
                logger,
                cacheKeyGenerator,
                cacheDuration);
        });
    }

    private static void AddCacheInvalidationDecorator<TRequest, TResponse>(this IServiceCollection services,
        Func<TRequest, string[]> cacheKeyGenerator)
    {
        services.AddScoped<ICacheInvalidationDecorator<TRequest, TResponse>>(provider =>
        {
            var cache = provider.GetRequiredService<IMemoryCache>();
            var logger = provider.GetRequiredService<ILogger<CacheInvalidationDecorator<TRequest, TResponse>>>();
            
            return new CacheInvalidationDecorator<TRequest, TResponse>(
                cache,
                logger,
                cacheKeyGenerator);
        });
    }

    private static void AddCacheReadWriteDecorator<TRequest, TResponse>(this IServiceCollection services,
        Func<TRequest, string> cacheKeyGenerator,
        Func<TRequest, TResponse, string> cacheKeyFromResponseGenerator,
        TimeSpan cacheDuration)
    {
        services.AddScoped<ICacheReadWriteDecorator<TRequest, TResponse>>(provider =>
        {
            var cache = provider.GetRequiredService<IMemoryCache>();
            var logger = provider.GetRequiredService<ILogger<CacheReadWriteDecorator<TRequest, TResponse>>>();
            
            return new CacheReadWriteDecorator<TRequest, TResponse>(
                cache,
                logger,
                cacheKeyGenerator,
                cacheKeyFromResponseGenerator,
                cacheDuration);
        });
    }
}