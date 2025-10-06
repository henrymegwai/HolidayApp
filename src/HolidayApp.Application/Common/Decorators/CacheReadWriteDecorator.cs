using HolidayApp.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Common.Decorators;

public class CacheReadWriteDecorator<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CacheReadWriteDecorator<TRequest, TResponse>> logger,
    Func<TRequest, string> cacheKeyGenerator,
    Func<TRequest, TResponse, string> cacheKeyFromResponseGenerator,
    TimeSpan cacheDuration)
    : ICacheReadWriteDecorator<TRequest, TResponse>
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

        if (!result.Status || result.Data == null) return result;
        
        var finalCacheKey = cacheKeyFromResponseGenerator(request, result.Data);
        cache.Set(finalCacheKey, result.Data, cacheDuration);
        logger.LogDebug("Cached result for key: {CacheKey}", finalCacheKey);

        return result;
    }
}
