# Background Services in ASP.NET Core

A background service is code that runs outside any HTTP request: cleaning
up old data, sending queued emails, polling an external API, or processing
jobs from a queue. ASP.NET Core runs them through the generic host with
`IHostedService` and `BackgroundService`.

## A simple timed worker

```csharp
public class CleanupService : BackgroundService
{
    private readonly ILogger<CleanupService> _logger;

    public CleanupService(ILogger<CleanupService> logger) =>
        _logger = logger;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cleaning up old records...");
            // await _repository.DeleteOldRecordsAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

Register it in `Program.cs`:

```csharp
builder.Services.AddHostedService<CleanupService>();
```

## Rules for writing workers

- **Always respect the `CancellationToken`.** Pass it to `Task.Delay`
  and to every async call. When the app shuts down, the token is
  cancelled and `ExecuteAsync` should finish quickly and cleanly.
- **Never use `Thread.Sleep` or a bare `while (true)` loop** — they block
  a thread and ignore shutdown.
- **Catch exceptions inside the loop.** An unhandled exception kills the
  worker silently, and the host keeps running as if nothing happened:

```csharp
while (!stoppingToken.IsCancellationRequested)
{
    try
    {
        await DoWorkAsync(stoppingToken);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Background work failed; retrying.");
    }

    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
}
```

## Scoped services inside a worker

A `BackgroundService` is a singleton, but `DbContext` and repositories
are scoped. Do not inject them directly — create a scope per unit of work:

```csharp
public class EmailWorker : BackgroundService
{
    private readonly IServiceProvider _services;

    public EmailWorker(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            await sender.SendPendingAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

## When to use a background service — and when not to

- Use it for recurring maintenance, queue consumers, cache warmers and
  scheduled sync jobs that belong to the web app itself.
- Do not use it for heavy, long-running or business-critical jobs: use a
  real queue (Hangfire, Azure Functions, or a worker service project)
  with retries, persistence and monitoring. A background service has no
  retries and no dashboard — if the app restarts, in-memory progress is
  lost.
