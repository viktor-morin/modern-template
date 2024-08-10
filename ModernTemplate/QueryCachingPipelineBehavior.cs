using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ModernTemplate;

internal sealed class QueryCachingPipelineBehavior<TRequest, TResponse>(
    IMemoryCache cacheService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
       if (await cacheService.TryGetValue<TResponse>(
            request.,
            out var cachedResult))
        {
            return cachedResult;
        }

        string requestName = typeof(TRequest).Name;
        if (cachedResult is not null)
        {
            logger.LogInformation("Cache hit for {RequestName}", requestName);

            return cachedResult;
        }

        logger.LogInformation("Cache miss for {RequestName}", requestName);

        TResponse result = await next();

        if (result.IsSuccess)
        {
            await cacheService.SetAsync(
                request.CacheKey,
                result,
                request.Expiration,
                cancellationToken);
        }

        return result;
    }
}

public interface ICachedQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}

//https://www.youtube.com/watch?v=LOEYZRE72wE