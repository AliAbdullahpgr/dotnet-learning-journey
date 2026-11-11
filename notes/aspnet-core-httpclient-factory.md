# HttpClientFactory in ASP.NET Core

`new HttpClient()` looks harmless, but creating one per request leaks
sockets: each instance holds its connection open until the OS times it
out, and under load the app runs out of ports. `IHttpClientFactory`
solves this by pooling the underlying handlers while letting you create
clients freely.

## Registering and using it

```csharp
// Program.cs
builder.Services.AddHttpClient();

// In a service — inject the factory, create a client per use.
public class WeatherService
{
    private readonly IHttpClientFactory _factory;
    public WeatherService(IHttpClientFactory factory) => _factory = factory;

    public async Task<string> GetAsync(string city)
    {
        var client = _factory.CreateClient();
        return await client.GetStringAsync(
            $"https://api.example.com/weather?city={city}");
    }
}
```

Clients are cheap to create; the expensive `HttpMessageHandler` is pooled
and recycled (default lifetime: 2 minutes), so DNS changes are picked up
without restarting the app.

## Named and typed clients

```csharp
// Named client — base address and headers in one place.
builder.Services.AddHttpClient("github", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "dotnet-learning");
});

var client = _factory.CreateClient("github");
var repos = await client.GetStringAsync("users/Daimx1214/repos");

// Typed client — the HttpClient is injected straight into the class.
builder.Services.AddHttpClient<GitHubService>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
});

public class GitHubService
{
    public GitHubService(HttpClient client) => _client = client;
    // Use _client directly — no factory needed here.
}
```

Typed clients are the cleanest option: the class declares what it needs
and the configuration lives in `Program.cs`.

## Rules and pitfalls

- **Never** `new HttpClient()` per request, and **never** share one
  static `HttpClient` forever — the first exhausts sockets, the second
  caches DNS forever and misses server moves.
- Do not dispose the `HttpClient` you get from the factory; the factory
  owns the lifetime of the underlying handler.
- Add resilience (retries, timeouts, circuit breaker) as a delegating
  handler — Polly's `AddPolicyHandler` plugs straight into
  `AddHttpClient` — instead of writing retry loops in every service.
- Set a timeout: the default is 100 seconds, which is far too long for
  most APIs. Configure it per client in `Program.cs`.
