# Redis and Distributed Caching in ASP.NET Core

In-memory caching (`IMemoryCache`) lives inside one server process. When
the app runs on several servers, each has its own cache and they drift
apart. A distributed cache — usually Redis — is shared by all servers, so
a value cached once is visible everywhere.

## Add Redis as the distributed cache

Add the `Microsoft.Extensions.Caching.StackExchangeRedis` package:

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp:";
});
```

Now inject `IDistributedCache` anywhere:

```csharp
public class ProductService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;

    public ProductService(IDistributedCache cache, AppDbContext db)
    {
        _cache = cache;
        _db = db;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        string key = $"product:{id}";

        byte[]? cached = await _cache.GetAsync(key);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<Product>(cached);
        }

        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2),
        };

        await _cache.SetAsync(
            key,
            JsonSerializer.SerializeToUtf8Bytes(product),
            options);

        return product;
    }
}
```

## Absolute vs sliding expiration

- **Absolute expiration** — the entry dies after a fixed time, no matter
  how often it is read. Use it for data that must refresh on a schedule.
- **Sliding expiration** — the entry dies after a period of *no use*. Use
  it for session-like data that stays alive while it is active.

Set both when you can: absolute is the hard ceiling, sliding keeps hot
data alive.

## What to cache — and what not to

- Cache data that is expensive to compute and read often: product lists,
  lookup tables, rendered fragments.
- Never cache per-user secrets or data that changes every request — the
  cache will be stale before it is useful.
- Cache invalidation is the hard part: delete the key when the underlying
  data changes (`await _cache.RemoveAsync(key)`), or the cache will serve
  old data until it expires.

## Redis beyond caching

Redis is also a message broker (pub/sub), a queue, a session store and a
rate-limit counter. `IDistributedCache` only exposes the cache part; for
the rest, use `StackExchange.Redis` directly with `ConnectionMultiplexer`.

## Rules to remember

- The cache is a performance aid, not storage: Redis can evict entries
  under memory pressure, so code must always handle a cache miss.
- `IDistributedCache` stores bytes — serialize with `System.Text.Json`
  and keep the payload small.
- In development with no Redis server, swap the registration for
  `AddDistributedMemoryCache()` and the code does not change.
