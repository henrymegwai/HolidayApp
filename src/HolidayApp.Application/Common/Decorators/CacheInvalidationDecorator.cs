using HolidayApp.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Common.Decorators;

public class CacheInvalidationDecorator<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CacheInvalidationDecorator<TRequest, TResponse>> logger,
    Func<TRequest, string[]> cacheKeyGenerator)
    : ICacheInvalidationDecorator<TRequest, TResponse>
{
    public async Task<Response<TResponse>> HandleAsync(
        TRequest request, 
        Func<TRequest, CancellationToken, Task<Response<TResponse>>> next, 
        CancellationToken cancellationToken)
    {
        // Execute the command
        var result = await next(request, cancellationToken);

        if (!result.Status) return result;
        
        var cacheKeys = cacheKeyGenerator(request);
        foreach (var key in cacheKeys)
        {
            cache.Remove(key);
            logger.LogDebug("Cache invalidated for key: {CacheKey}", key);
        }

        return result;
    }
}
