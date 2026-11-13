# Distributed Caching in ASP.NET Core (ASP.NET Core)

`IDistributedCache` is the abstraction ASP.NET Core provides for a cache shared across servers. Your code talks to the interface; the backing store (in-memory, SQL Server, or Redis) is chosen in configuration.

## Why distributed instead of in-memory

`IMemoryCache` lives inside one process. In a load-balanced app with two servers, a value cached on server A is invisible to server B, so each server does its own database work. A distributed cache is shared: one write is visible to every server.

## The IDistributedCache interface

It works with byte arrays, so anything you cache must be serialized first.

```csharp
public interface IDistributedCache
{
    byte[]? Get(string key);
    Task<byte[]?> GetAsync(string key, CancellationToken token = default);
    void Set(string key, byte[] value, DistributedCacheEntryOptions options);
    Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
        CancellationToken token = default);
    void Refresh(string key);           // resets the sliding expiration
    Task RefreshAsync(string key, CancellationToken token = default);
    void Remove(string key);
    Task RemoveAsync(string key, CancellationToken token = default);
}
```

`DistributedCacheEntryOptions` sets the lifetime:

```csharp
var options = new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
    SlidingExpiration = TimeSpan.FromMinutes(5) // extends on each access
};
```

## SQL Server distributed cache

The SQL Server provider stores entries in a table it creates for you. It is a good fit when you already run SQL Server and do not want another service to operate.

```bash
dotnet tool install --global dotnet-sql-cache
dotnet sql-cache create "Server=.;Database=AppDb;Trusted_Connection=True;" dbo CacheTable
```

```csharp
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("Default");
    options.SchemaName = "dbo";
    options.TableName = "CacheTable";
});
```

Trade-off: it is slower than Redis and adds load to your database, but it needs no new infrastructure.

## Cache-aside pattern

The application, not the cache, owns the data. On a miss, read from the database, store the result in the cache, then return it.

```csharp
public async Task<Product?> GetProductAsync(int id, CancellationToken ct)
{
    string key = $"product:{id}";
    byte[]? cached = await _cache.GetAsync(key, ct);
    if (cached is not null)
        return JsonSerializer.Deserialize<Product>(cached);

    Product? product = await _db.Products.FindAsync(id, ct);
    if (product is null) return null;

    await _cache.SetAsync(key,
        JsonSerializer.SerializeToUtf8Bytes(product),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        }, ct);
    return product;
}
```

## Notes

- Cache keys need a naming convention (`product:42`, `user:7:profile`) so entries never collide and can be invalidated by prefix.
- Always set an expiration. A cache with no expiry is a memory leak with extra steps.
- Cache invalidation is the hard part: remove or overwrite the key when the underlying data changes, or accept a short TTL and stale reads.
- For very hot data, combine both layers: `IMemoryCache` in front of `IDistributedCache`, so repeated reads never leave the process.
- The Redis provider (`AddStackExchangeRedisCache`) uses the same interface — switching providers is a one-line configuration change, which is the whole point of the abstraction.
