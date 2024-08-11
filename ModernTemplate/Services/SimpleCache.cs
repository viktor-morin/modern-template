using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace ModernTemplate.Services;

public sealed class SimpleCache
{
    private readonly HybridCache _cache;

    public SimpleCache(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<string> GetAsync<T>(string key)
    {
        return await _cache.GetOrCreateAsync("key", async cancel =>
        {
            await Task.Delay(1);
            var resturnValue = "value";
            return resturnValue;
        });
    }
}
