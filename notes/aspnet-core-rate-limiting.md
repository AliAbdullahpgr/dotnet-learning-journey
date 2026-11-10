# Rate Limiting in ASP.NET Core

Rate limiting protects an API from abuse and from honest mistakes: one
client sending too many requests cannot take the service down for
everyone else. ASP.NET Core 7+ ships a built-in rate limiter — no package
needed.

## Register a policy

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;              // 100 requests
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddSlidingWindowLimiter("sliding", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.SegmentsPerWindow = 5;
    });

    options.AddTokenBucketLimiter("api", limiterOptions =>
    {
        limiterOptions.TokenLimit = 100;
        limiterOptions.TokensPerPeriod = 10;
        limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
```

Enable it in the pipeline and apply the policy to an endpoint:

```csharp
var app = builder.Build();
app.UseRateLimiter();

app.MapGet("/api/products", () => new[] { "a", "b" })
   .RequireRateLimiting("fixed");
```

Or apply it to a whole controller with an attribute:

```csharp
[EnableRateLimiting("api")]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase { }
```

## Which limiter to pick

- **Fixed window** — simple: N requests per minute. Easy to explain, but
  a burst at the window edge can allow 2N requests in a short span.
- **Sliding window** — smoother than fixed window; the limit is spread
  across the window. The best default for most APIs.
- **Token bucket** — allows short bursts (spend saved tokens) while
  keeping the average rate. Good for APIs where occasional bursts are
  normal.
- **Concurrency limiter** — limits how many requests run at the same
  time, not how many arrive. Use it to protect a slow downstream service.

## Per-user limits

By default the limiter counts per endpoint, not per user. For per-user
limits, partition by the authenticated user id (or by client IP for
anonymous endpoints):

```csharp
options.AddPolicy("per-user", context =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.User.Identity?.Name ?? "anonymous",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
        }));
```

## Rules to remember

- Return **429 Too Many Requests** with a `Retry-After` header, so
  clients back off instead of retrying immediately.
- Limit login and password-reset endpoints strictly — they are the first
  target of brute-force attacks.
- Rate limiting is not authentication: a limiter slows an attacker down,
  it does not identify them.
- For multiple servers, an in-memory limiter counts per server. Use a
  distributed limiter (Redis) when the API runs on more than one machine.
