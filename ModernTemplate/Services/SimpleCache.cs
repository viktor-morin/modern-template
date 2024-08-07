using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace ModernTemplate.Services;

public sealed class SimpleCache
{
    private readonly IMemoryCache _cache;

    public SimpleCache(IMemoryCache cache)
    {
        _cache = cache;

        var t = _cache.GetOrCreateAsync("key", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            return Task.FromResult("value");
        });
    }
}
