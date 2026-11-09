# Health Checks in ASP.NET Core

Health checks expose a small endpoint that reports whether an application and the things it depends on are working, so a load balancer, container platform, or monitoring tool can decide what to do.

## Key points

- Register checks with `builder.Services.AddHealthChecks()` and expose them with `app.MapHealthChecks("/health")`; the endpoint returns 200 when healthy and 503 when not.
- A check implements `IHealthCheck` and returns `HealthCheckResult.Healthy()`, `.Degraded()`, or `.Unhealthy()`, with an optional description.
- The built-in `AddCheck` overload takes a name and a function, which is enough for simple checks such as pinging a database.
- Tag checks (`tags: new[] { "ready" }`) and map different endpoints per tag, for example `/health/live` for liveness and `/health/ready` for readiness.
- A liveness check asks "is the process alive?" and is cheap; a readiness check asks "can it serve traffic?" and may test the database or a downstream service.
- Keep checks fast and read-only, and give them a timeout so a slow dependency does not hang the endpoint.
- Do not expose internal details in a public health endpoint; the default response is just the word Healthy or Unhealthy.

## Example

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("The app is running."));

var app = builder.Build();
app.MapHealthChecks("/health");
// GET /health returns 200 with the body "Healthy".
```
