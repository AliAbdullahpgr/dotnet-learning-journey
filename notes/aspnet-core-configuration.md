# ASP.NET Core Configuration

ASP.NET Core builds one `IConfiguration` from several sources, layered in a fixed order. Later sources override earlier ones.

## Default sources (lowest to highest priority)

1. `appsettings.json`
2. `appsettings.{Environment}.json` (e.g. Development, Production)
3. User secrets (Development only)
4. Environment variables
5. Command-line arguments

## appsettings.json example

```json
{
  "Logging": {
    "LogLevel": { "Default": "Information" }
  },
  "ApiSettings": {
    "BaseUrl": "https://api.example.com",
    "TimeoutSeconds": 30
  }
}
```

## Reading values

```csharp
// Direct read
string? baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

// Strongly typed with the options pattern
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
```

Inject `IOptions<ApiSettings>` (or `IOptionsSnapshot<T>` for reloadable values) into a service to use the settings.

## Key points

- Never put real secrets in appsettings.json committed to git; use user secrets locally and environment variables or a vault in production.
- Environment-specific files let Development and Production differ without code changes.
- `IOptionsMonitor<T>` can react when configuration changes at runtime.
