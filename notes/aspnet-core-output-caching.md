# Output Caching in ASP.NET Core

Output caching stores a rendered response and serves it again for
identical requests, so the endpoint, the database and the serializers do
not run at all. It is the simplest performance win for endpoints whose
answers change rarely.

## The basics

```csharp
// Program.cs
builder.Services.AddOutputCache();

// Cache this endpoint's response for 60 seconds.
app.MapGet("/products", async (AppDbContext db) =>
        await db.Products.ToListAsync())
    .CacheOutput(p => p.Expire(TimeSpan.FromSeconds(60)));
```

The first request runs the endpoint and stores the result; the next
identical request within 60 seconds gets the stored response with a
`200 OK` and an `Age` header.

## Policies and varying

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("Products", b => b
        .Expire(TimeSpan.FromMinutes(5))
        .SetVaryByQuery("category")   // separate cache per category value
        .SetVaryByHeader("Accept-Language"));
});

app.MapGet("/products", ...).CacheOutput("Products");
```

- `SetVaryByQuery` / `SetVaryByHeader` / `SetVaryByRouteValue` keep a
  separate cached copy per value, so `?category=books` never serves the
  `?category=toys` response.
- `Tag("products")` lets you evict a whole group later:
  `await cacheStore.EvictByTagAsync("products", default)` after a product
  changes.

## Output caching vs response caching

- **Response caching** (`[ResponseCache]`) only sets cache *headers* —
  the browser or a CDN may cache, but the server still runs the endpoint
  on every request that reaches it.
- **Output caching** stores the response on the *server* (in memory by
  default, Redis via a store) and short-circuits the endpoint entirely.
- Use response caching when a CDN or browser cache is the goal; use
  output caching when the server itself must skip the work.

## What not to cache

- Anything user-specific or authenticated — a cached response could leak
  one user's data to another. Vary by header or skip caching for those
  endpoints entirely.
- Endpoints behind a POST/PUT/DELETE — output caching applies to safe
  GET/HEAD requests; writes must always run.
- Data that changes on every request (a live counter, a "just now"
  timestamp). Cache the stable parts and fetch the volatile bits
  client-side instead.
