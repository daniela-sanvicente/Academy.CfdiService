using System.Text.Json;
using Academy.CfdiService.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Academy.CfdiService.Infrastructure.Caching;

public class DistributedCacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IDistributedCache _cache;

    public DistributedCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cached))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(cached, SerializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (value is null)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(value, SerializerOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await _cache.SetStringAsync(key, payload, options, cancellationToken);
    }
}
