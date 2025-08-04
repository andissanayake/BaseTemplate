using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Common.Behaviors;

/// <summary>
/// Interface for cacheable queries
/// </summary>
public interface ICacheableQuery
{
    /// <summary>
    /// Cache key for the query
    /// </summary>
    string CacheKey { get; }
    
    /// <summary>
    /// Cache expiration time
    /// </summary>
    TimeSpan CacheExpiration { get; }
}

/// <summary>
/// Caching behavior that wraps query handlers with caching functionality
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only cache queries that implement ICacheableQuery
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        var cacheKey = cacheableQuery.CacheKey;
        
        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse))
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
        
        // Execute the handler
        var response = await next();
        
        // Cache the response if successful
        if (response is Result<object> result && ResultCodeMapper.IsSuccess(result.Code))
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheableQuery.CacheExpiration,
                Priority = CacheItemPriority.Normal
            };
            
            _cache.Set(cacheKey, response, cacheOptions);
            _logger.LogDebug("Cached response for key: {CacheKey}, Expiration: {Expiration}", 
                cacheKey, cacheableQuery.CacheExpiration);
        }

        return response;
    }
}