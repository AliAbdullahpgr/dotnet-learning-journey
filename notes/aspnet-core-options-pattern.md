# The Options Pattern in ASP.NET Core

The Options pattern binds a section of configuration (appsettings.json,
environment variables, secrets) to a strongly typed class, so the rest of
the code reads `options.Value.SmtpHost` instead of magic strings like
`configuration["Email:SmtpHost"]`.

## Define the options class

```csharp
public class EmailSettings
{
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; } = 587;
    public string FromAddress { get; set; } = "";
}
```

```json
{
  "Email": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "FromAddress": "noreply@example.com"
  }
}
```

## Register and inject

```csharp
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));
```

```csharp
public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public void Send(string to, string body)
    {
        Console.WriteLine($"Sending via {_settings.SmtpHost} from {_settings.FromAddress}");
    }
}
```

## The three interfaces

- **`IOptions<T>`** — a singleton snapshot taken at startup. Use it when
  the settings never change while the app runs.
- **`IOptionsSnapshot<T>`** — scoped: re-reads configuration on every
  request. Use it when settings may change without restarting the app.
- **`IOptionsMonitor<T>`** — singleton with live reload: `CurrentValue`
  always reflects the latest configuration, and `OnChange` lets you react
  when a value changes. Use it for long-lived services such as background
  workers.

## Validation

Bad configuration should fail at startup, not at 2 a.m. in production:

```csharp
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("Email"))
    .Validate(settings =>
        !string.IsNullOrWhiteSpace(settings.SmtpHost) &&
        settings.SmtpPort > 0,
        "Email settings are missing or invalid.")
    .ValidateOnStart();
```

## Rules to remember

- Options classes should be plain classes with simple properties — no
  logic, no constructors with parameters.
- Prefer `IOptionsSnapshot<T>` or `IOptionsMonitor<T>` for settings an
  operator might change (timeouts, feature flags); `IOptions<T>` is fine
  for values that are fixed at deploy time.
- Never store secrets in appsettings.json — use user secrets in
  development and a secret store in production. The Options pattern reads
  from any configuration provider, so the code does not change.
