using HolidayApp.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Common.Decorators;

public class CacheDecorator<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CacheDecorator<TRequest, TResponse>> logger,
    Func<TRequest, string> cacheKeyGenerator,
    TimeSpan cacheDuration)
    : ICacheDecorator<TRequest, TResponse>
{
    public async Task<Response<TResponse>> HandleAsync(
        TRequest request, 
        Func<TRequest, CancellationToken, Task<Response<TResponse>>> next, 
        CancellationToken cancellationToken)
    {
        var cacheKey = cacheKeyGenerator(request);
        
        if (cache.TryGetValue(cacheKey, out TResponse? cached) && cached != null)
        {
            logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
            return new Response<TResponse>(true, cached, "Data retrieved from cache");
        }

        logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
        
        // Execute the next handler
        var result = await next(request, cancellationToken);
        
        // Cache the result if successful
        if (result.Status && result.Data != null)
        {
            cache.Set(cacheKey, result.Data, cacheDuration);
            logger.LogDebug("Cached result for key: {CacheKey}", cacheKey);
        }
        
        return result;
    }
}
