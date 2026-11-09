# Caching in ASP.NET Core

Caching stores the result of expensive work so repeated requests can be served faster. The trade-off is staleness: cached data can be out of date, so every cache entry needs a lifetime that matches how often the data changes.

## Key points

- **In-memory cache (`IMemoryCache`):** stored in the process memory. Fast and simple, but lost on restart and not shared between servers — best for single-server apps.
- **Distributed cache (`IDistributedCache`):** shared across servers, backed by Redis, SQL Server, or NCache. Values are serialized (usually to bytes or JSON), so it works in multi-server deployments.
- **Response caching:** `[ResponseCache]` plus the Response Caching Middleware caches whole HTTP responses based on headers and query keys.
- **Output caching:** ASP.NET Core 7+ can cache the rendered output of endpoints per route and policy.
- **Expiration:** absolute expiration removes an entry after a fixed time; sliding expiration extends the lifetime on each access. Entries can also be evicted under memory pressure according to their priority.

## Example

```csharp
app.MapGet("/products/{id}", async (int id, IMemoryCache cache, AppDbContext db) =>
{
    return await cache.GetOrCreateAsync($"product-{id}", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return await db.Products.FindAsync(id);
    });
});
```

The first request for a product hits the database and stores the result; later requests within five minutes are served from the cache.
