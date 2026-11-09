# Logging in ASP.NET Core

Logging records what an application is doing while it runs, so you can trace a request, find a bug after the fact, and spot errors in production.

## Key points

- Inject `ILogger<T>` into a controller or service; the generic type names the category, which is usually the class name.
- Log levels, from lowest to highest severity, are `Trace`, `Debug`, `Information`, `Warning`, `Error`, and `Critical`.
- Set the minimum level per category in `appsettings.json` under `Logging:LogLevel`, for example a quiet default with a louder level for your own code.
- Write structured messages with placeholders, such as `LogInformation("Order {OrderId} placed", orderId)`; the value is stored as a searchable field, not just as text.
- Use `LogError` with the exception as the first argument so the stack trace is included: `LogError(ex, "Failed to load order {OrderId}", orderId)`.
- Providers decide where logs go: the console, a file, Application Insights, or another sink. ASP.NET Core configures the console and debug providers by default.
- Use scopes (`BeginScope`) to attach the same context, such as a request or order id, to every log line inside a block.
- Never log passwords, tokens, or other secrets.

## Example

```csharp
app.MapGet("/orders/{id:int}", (int id, ILogger<Program> logger) =>
{
    logger.LogInformation("Loading order {OrderId}", id);
    return Results.Ok(new { OrderId = id });
});
// A request to /orders/42 logs: Loading order 42
// (with OrderId stored as a structured field).
```
